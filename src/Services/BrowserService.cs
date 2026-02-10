using System;
using PuppeteerSharp;
using System.IO;
using System.Linq;
using Pup.Transport;
using System.Management.Automation;
using System.Collections.Generic;
using Pup.Common;
using System.Threading.Tasks;
using System.Text.Json;

namespace Pup.Services
{
    public interface IBrowserService
    {
        Task<PupPage> CreatePageAsync(string name, int width, int height, string url, bool waitForLoad);
        Task<List<PupPage>> GetPagesAsync();
        bool RemoveBrowser();
        bool StopBrowser();
    }

    public class BrowserService : SupportedBrowserService, IBrowserService
    {
        private readonly PupBrowser _browser;

        public BrowserService(PupBrowser browser, SessionState sessionState) : base(sessionState)
        {
            _browser = browser;
        }

        public async Task<List<PupPage>> GetPagesAsync()
        {
            var pages = await _browser.Browser.PagesAsync().ConfigureAwait(false);

            var pupPages = new List<PupPage>();
            foreach (var page in pages)
            {
                var title = await page.GetTitleAsync().ConfigureAwait(false);
                var pupPage = new PupPage(page, title);
                await InitializePageCaptureAsync(pupPage).ConfigureAwait(false);
                pupPages.Add(pupPage);
            }

            return pupPages;
        }

        public async Task<PupPage> CreatePageAsync(string name, int width, int height, string url, bool waitForLoad)
        {
            var pages = await _browser.Browser.PagesAsync().ConfigureAwait(false);
            string pageName = string.IsNullOrEmpty(name) ? $"Page{pages.Length + 1}" : name;

            var page = await _browser.Browser.NewPageAsync().ConfigureAwait(false);
            var pupPage = new PupPage(page, string.Empty);

            // Apply stealth mode - inject before any page content loads
            await ApplyStealthModeAsync(page).ConfigureAwait(false);
            await ApplyWebSocketTrackerAsync(page).ConfigureAwait(false);

            // Set viewport size
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = width,
                Height = height
            }).ConfigureAwait(false);

            await InitializePageCaptureAsync(pupPage).ConfigureAwait(false);

            // Navigate to URL if specified
            if (!string.IsNullOrEmpty(url) && url != "about:blank")
            {
                if (waitForLoad)
                {
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                    }).ConfigureAwait(false);
                }
                else
                {
                    await page.GoToAsync(url).ConfigureAwait(false);
                }
            }

            var title = await page.GetTitleAsync().ConfigureAwait(false);
            pupPage.Title = title;
            pupPage.Url = page.Url;
            return pupPage;
        }

        public bool RemoveBrowser()
        {
            if (Directory.Exists(_browser.Path))
            {
                Directory.Delete(_browser.Path, true);
                return true;
            }
            return false;
        }

        public bool StopBrowser()
        {

            if (!_browser.Running)
            {
                return false;
            }

            _browser.Browser.CloseAsync().GetAwaiter().GetResult();

            _sessionStateService.Remove(_browser.BrowserType.ToString());

            return true;
        }

        private async Task InitializePageCaptureAsync(PupPage pupPage)
        {
            if (pupPage == null || pupPage.CaptureInitialized)
            {
                return;
            }

            // Console capture
            pupPage.Page.Console += (sender, e) =>
            {
                try
                {
                    var text = e.Message.Text;

                    // Check for recording event
                    if (text != null && text.StartsWith("__PUP_RECORDING__:"))
                    {
                        if (pupPage.RecordingActive)
                        {
                            var json = text.Substring("__PUP_RECORDING__:".Length);
                            var recordingEvent = ParseRecordingEvent(json);
                            if (recordingEvent != null)
                            {
                                lock (pupPage.RecordingLock)
                                {
                                    pupPage.RecordingEvents.Add(recordingEvent);
                                }
                            }
                        }
                        return; // Don't add to regular console entries
                    }

                    var entry = new PupConsoleEntry
                    {
                        Type = e.Message.Type.ToString(),
                        Text = text,
                        Url = e.Message.Location?.URL,
                        LineNumber = e.Message.Location?.LineNumber,
                        ColumnNumber = e.Message.Location?.ColumnNumber,
                        Timestamp = DateTime.UtcNow
                    };

                    lock (pupPage.ConsoleLock)
                    {
                        pupPage.ConsoleEntries.Add(entry);
                    }
                }
                catch
                {
                    // ignore console parsing issues
                }
            };

            // Network capture via CDP
            try
            {
                var session = await pupPage.Page.CreateCDPSessionAsync().ConfigureAwait(false);
                pupPage.NetworkSession = session;
                session.MessageReceived += (sender, e) =>
                {
                    try
                    {
                        switch (e.MessageID)
                        {
                            case "Network.requestWillBeSent":
                                HandleRequestWillBeSent(pupPage, e.MessageData);
                                break;
                            case "Network.responseReceived":
                                HandleResponseReceived(pupPage, e.MessageData);
                                break;
                            case "Network.loadingFinished":
                                HandleLoadingFinished(pupPage, e.MessageData);
                                break;
                            case "Network.loadingFailed":
                                HandleLoadingFailed(pupPage, e.MessageData);
                                break;
                            case "Network.webSocketCreated":
                                HandleWebSocketCreated(pupPage, e.MessageData);
                                break;
                            case "Network.webSocketFrameSent":
                                HandleWebSocketFrameSent(pupPage, e.MessageData);
                                break;
                            case "Network.webSocketFrameReceived":
                                HandleWebSocketFrameReceived(pupPage, e.MessageData);
                                break;
                            case "Network.webSocketClosed":
                                HandleWebSocketClosed(pupPage, e.MessageData);
                                break;
                        }
                    }
                    catch
                    {
                        // swallow event parsing errors
                    }
                };
                await session.SendAsync("Network.enable").ConfigureAwait(false);
            }
            catch
            {
                // ignore network capture init failures
            }

            pupPage.CaptureInitialized = true;
        }

        private static void HandleRequestWillBeSent(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();
            var request = data.GetProperty("request");
            var entry = new PupNetworkEntry
            {
                RequestId = requestId,
                Url = request.GetProperty("url").GetString(),
                Method = request.GetProperty("method").GetString(),
                ResourceType = data.TryGetProperty("type", out var t) ? t.GetString() : null,
                StartTime = DateTime.UtcNow,
                RequestHeaders = ToHeaderDictionary(request.GetProperty("headers"))
            };

            lock (pupPage.NetworkLock)
            {
                pupPage.NetworkMap[requestId] = entry;
                pupPage.NetworkEntries.Add(entry);
            }
        }

        private static void HandleResponseReceived(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();
            var response = data.GetProperty("response");

            lock (pupPage.NetworkLock)
            {
                if (!pupPage.NetworkMap.TryGetValue(requestId, out var entry))
                {
                    entry = new PupNetworkEntry { RequestId = requestId, StartTime = DateTime.UtcNow };
                    pupPage.NetworkMap[requestId] = entry;
                    pupPage.NetworkEntries.Add(entry);
                }

                entry.Status = (int)response.GetProperty("status").GetDouble();
                entry.StatusText = response.GetProperty("statusText").GetString();
                entry.MimeType = response.TryGetProperty("mimeType", out var mt) ? mt.GetString() : null;
                entry.ResponseHeaders = ToHeaderDictionary(response.GetProperty("headers"));
                entry.FromDiskCache = response.TryGetProperty("fromDiskCache", out var fdc) && fdc.GetBoolean();
                entry.FromServiceWorker = response.TryGetProperty("fromServiceWorker", out var fsw) && fsw.GetBoolean();
                entry.RemoteAddress = response.TryGetProperty("remoteIPAddress", out var ip) ? ip.GetString() : null;

                // Capture security details for HTTPS requests
                if (response.TryGetProperty("securityDetails", out var securityDetails) &&
                    securityDetails.ValueKind == JsonValueKind.Object)
                {
                    entry.SecurityDetails = ParseSecurityDetails(securityDetails);
                }
            }
        }

        private static void HandleLoadingFinished(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();

            lock (pupPage.NetworkLock)
            {
                if (pupPage.NetworkMap.TryGetValue(requestId, out var entry))
                {
                    entry.EncodedDataLength = data.TryGetProperty("encodedDataLength", out var len) ? len.GetInt64() : (long?)null;
                    entry.EndTime = DateTime.UtcNow;
                }
            }
        }

        private static void HandleLoadingFailed(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();
            lock (pupPage.NetworkLock)
            {
                if (pupPage.NetworkMap.TryGetValue(requestId, out var entry))
                {
                    entry.ErrorText = data.TryGetProperty("errorText", out var err) ? err.GetString() : null;
                    entry.EndTime = DateTime.UtcNow;
                }
            }
        }

        private static PupSecurityDetails ParseSecurityDetails(JsonElement securityDetails)
        {
            var details = new PupSecurityDetails
            {
                Protocol = securityDetails.TryGetProperty("protocol", out var proto) ? proto.GetString() : null,
                KeyExchange = securityDetails.TryGetProperty("keyExchange", out var kx) ? kx.GetString() : null,
                KeyExchangeGroup = securityDetails.TryGetProperty("keyExchangeGroup", out var kxg) ? kxg.GetString() : null,
                Cipher = securityDetails.TryGetProperty("cipher", out var cipher) ? cipher.GetString() : null,
                Mac = securityDetails.TryGetProperty("mac", out var mac) ? mac.GetString() : null,
                SubjectName = securityDetails.TryGetProperty("subjectName", out var subj) ? subj.GetString() : null,
                Issuer = securityDetails.TryGetProperty("issuer", out var issuer) ? issuer.GetString() : null,
                CertificateTransparencyCompliance = securityDetails.TryGetProperty("certificateTransparencyCompliance", out var ctc) ? ctc.GetString() : null
            };

            // Parse validFrom/validTo (Unix epoch seconds)
            if (securityDetails.TryGetProperty("validFrom", out var validFrom))
            {
                details.ValidFrom = DateTimeOffset.FromUnixTimeSeconds((long)validFrom.GetDouble()).UtcDateTime;
            }
            if (securityDetails.TryGetProperty("validTo", out var validTo))
            {
                details.ValidTo = DateTimeOffset.FromUnixTimeSeconds((long)validTo.GetDouble()).UtcDateTime;
            }

            // Parse SAN list
            if (securityDetails.TryGetProperty("sanList", out var sanList) && sanList.ValueKind == JsonValueKind.Array)
            {
                foreach (var san in sanList.EnumerateArray())
                {
                    if (san.ValueKind == JsonValueKind.String)
                    {
                        details.SanList.Add(san.GetString());
                    }
                }
            }

            // Parse Signed Certificate Timestamp list
            if (securityDetails.TryGetProperty("signedCertificateTimestampList", out var sctList) && sctList.ValueKind == JsonValueKind.Array)
            {
                foreach (var sct in sctList.EnumerateArray())
                {
                    if (sct.ValueKind == JsonValueKind.Object)
                    {
                        var timestamp = new PupSignedCertificateTimestamp
                        {
                            Status = sct.TryGetProperty("status", out var status) ? status.GetString() : null,
                            Origin = sct.TryGetProperty("origin", out var origin) ? origin.GetString() : null,
                            LogDescription = sct.TryGetProperty("logDescription", out var logDesc) ? logDesc.GetString() : null,
                            LogId = sct.TryGetProperty("logId", out var logId) ? logId.GetString() : null,
                            HashAlgorithm = sct.TryGetProperty("hashAlgorithm", out var hashAlg) ? hashAlg.GetString() : null,
                            SignatureAlgorithm = sct.TryGetProperty("signatureAlgorithm", out var sigAlg) ? sigAlg.GetString() : null,
                            SignatureData = sct.TryGetProperty("signatureData", out var sigData) ? sigData.GetString() : null
                        };

                        if (sct.TryGetProperty("timestamp", out var ts))
                        {
                            timestamp.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds((long)ts.GetDouble()).UtcDateTime;
                        }

                        details.SignedCertificateTimestampList.Add(timestamp);
                    }
                }
            }

            return details;
        }

        private static Dictionary<string, string> ToHeaderDictionary(JsonElement headersElement)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in headersElement.EnumerateObject())
            {
                dict[prop.Name] = prop.Value.GetString();
            }
            return dict;
        }

        private static void HandleWebSocketCreated(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();
            var url = data.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : null;

            var entry = new PupWebSocketEntry
            {
                RequestId = requestId,
                Url = url,
                State = "connecting",
                CreatedTime = DateTime.UtcNow
            };

            lock (pupPage.WebSocketLock)
            {
                pupPage.WebSocketMap[requestId] = entry;
                pupPage.WebSocketEntries.Add(entry);
            }
        }

        private static void HandleWebSocketFrameSent(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();

            lock (pupPage.WebSocketLock)
            {
                if (pupPage.WebSocketMap.TryGetValue(requestId, out var entry))
                {
                    // Mark as open once we see frames
                    if (entry.State == "connecting")
                    {
                        entry.State = "open";
                    }

                    if (data.TryGetProperty("response", out var response))
                    {
                        var frame = new PupWebSocketFrame
                        {
                            Direction = "sent",
                            Timestamp = DateTime.UtcNow,
                            Opcode = response.TryGetProperty("opcode", out var op) ? op.GetInt32() : 1,
                            PayloadData = response.TryGetProperty("payloadData", out var pd) ? pd.GetString() : null
                        };
                        frame.PayloadLength = frame.PayloadData?.Length ?? 0;
                        entry.Frames.Add(frame);
                    }
                }
            }
        }

        private static void HandleWebSocketFrameReceived(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();

            lock (pupPage.WebSocketLock)
            {
                if (pupPage.WebSocketMap.TryGetValue(requestId, out var entry))
                {
                    // Mark as open once we see frames
                    if (entry.State == "connecting")
                    {
                        entry.State = "open";
                    }

                    if (data.TryGetProperty("response", out var response))
                    {
                        var frame = new PupWebSocketFrame
                        {
                            Direction = "received",
                            Timestamp = DateTime.UtcNow,
                            Opcode = response.TryGetProperty("opcode", out var op) ? op.GetInt32() : 1,
                            PayloadData = response.TryGetProperty("payloadData", out var pd) ? pd.GetString() : null
                        };
                        frame.PayloadLength = frame.PayloadData?.Length ?? 0;
                        entry.Frames.Add(frame);
                    }
                }
            }
        }

        private static void HandleWebSocketClosed(PupPage pupPage, JsonElement data)
        {
            var requestId = data.GetProperty("requestId").GetString();

            lock (pupPage.WebSocketLock)
            {
                if (pupPage.WebSocketMap.TryGetValue(requestId, out var entry))
                {
                    entry.State = "closed";
                    entry.ClosedTime = DateTime.UtcNow;
                }
            }
        }

        private static PupRecordingEvent ParseRecordingEvent(string json)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var evt = JsonSerializer.Deserialize<PupRecordingEvent>(json, options);
                if (evt != null && string.IsNullOrEmpty(evt.Timestamp.ToString()) == false)
                {
                    return evt;
                }
                // If timestamp wasn't parsed, set it now
                if (evt != null && evt.Timestamp == default)
                {
                    evt.Timestamp = DateTime.UtcNow;
                }
                return evt;
            }
            catch
            {
                return null;
            }
        }

        private static async Task ApplyStealthModeAsync(IPage page)
        {
            var script = EmbeddedResourceService.LoadScript("stealth.js");

            // Apply to future navigations
            await page.EvaluateExpressionOnNewDocumentAsync(script).ConfigureAwait(false);

            // Also apply to the current page context immediately
            await page.EvaluateExpressionAsync(script).ConfigureAwait(false);
        }

        private static async Task ApplyWebSocketTrackerAsync(IPage page)
        {
            var script = EmbeddedResourceService.LoadScript("websocket-tracker.js");

            // Apply to future navigations
            await page.EvaluateExpressionOnNewDocumentAsync(script).ConfigureAwait(false);

            // Also apply to the current page context immediately
            await page.EvaluateExpressionAsync(script).ConfigureAwait(false);
        }
    }
}
