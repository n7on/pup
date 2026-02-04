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
                    var entry = new PupConsoleEntry
                    {
                        Type = e.Message.Type.ToString(),
                        Text = e.Message.Text,
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

        private static Dictionary<string, string> ToHeaderDictionary(JsonElement headersElement)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in headersElement.EnumerateObject())
            {
                dict[prop.Name] = prop.Value.GetString();
            }
            return dict;
        }

        private static async Task ApplyStealthModeAsync(IPage page)
        {
            // Stealth script to hide automation indicators
            const string stealthScript = @"
                // Remove webdriver property
                Object.defineProperty(navigator, 'webdriver', {
                    get: () => undefined,
                    configurable: true
                });

                // Mock plugins to look like a real browser
                Object.defineProperty(navigator, 'plugins', {
                    get: () => {
                        const plugins = [
                            { name: 'Chrome PDF Plugin', filename: 'internal-pdf-viewer', description: 'Portable Document Format' },
                            { name: 'Chrome PDF Viewer', filename: 'mhjfbmdgcfjbbpaeojofohoefgiehjai', description: '' },
                            { name: 'Native Client', filename: 'internal-nacl-plugin', description: '' }
                        ];
                        plugins.item = (i) => plugins[i] || null;
                        plugins.namedItem = (name) => plugins.find(p => p.name === name) || null;
                        plugins.refresh = () => {};
                        return plugins;
                    },
                    configurable: true
                });

                // Mock languages
                Object.defineProperty(navigator, 'languages', {
                    get: () => ['en-US', 'en'],
                    configurable: true
                });

                // Fix permissions API
                const originalQuery = window.navigator.permissions?.query;
                if (originalQuery) {
                    window.navigator.permissions.query = (parameters) => (
                        parameters.name === 'notifications' ?
                            Promise.resolve({ state: Notification.permission }) :
                            originalQuery(parameters)
                    );
                }

                // Mock chrome runtime to look like a real browser
                if (!window.chrome) {
                    window.chrome = {};
                }
                if (!window.chrome.runtime) {
                    window.chrome.runtime = {
                        connect: () => {},
                        sendMessage: () => {},
                        onMessage: { addListener: () => {} }
                    };
                }

                // Remove automation-related properties from navigator
                const automationProps = ['__webdriver_evaluate', '__selenium_evaluate', '__webdriver_script_function',
                    '__webdriver_script_func', '__webdriver_script_fn', '__fxdriver_evaluate', '__driver_unwrapped',
                    '__webdriver_unwrapped', '__driver_evaluate', '__selenium_unwrapped', '__fxdriver_unwrapped'];
                automationProps.forEach(prop => {
                    delete navigator[prop];
                    delete window[prop];
                });

                // Fix iframe contentWindow access detection
                const originalContentWindow = Object.getOwnPropertyDescriptor(HTMLIFrameElement.prototype, 'contentWindow');
                Object.defineProperty(HTMLIFrameElement.prototype, 'contentWindow', {
                    get: function() {
                        const win = originalContentWindow.get.call(this);
                        if (win) {
                            // Make sure nested iframes also don't expose webdriver
                            try {
                                Object.defineProperty(win.navigator, 'webdriver', {
                                    get: () => undefined,
                                    configurable: true
                                });
                            } catch (e) {}
                        }
                        return win;
                    }
                });

                // Patch toString to hide native code modifications
                const originalToString = Function.prototype.toString;
                Function.prototype.toString = function() {
                    if (this === navigator.webdriver?.get ||
                        this === navigator.plugins?.get ||
                        this === navigator.languages?.get) {
                        return 'function get webdriver() { [native code] }';
                    }
                    return originalToString.call(this);
                };
            ";

            // Apply to future navigations
            await page.EvaluateExpressionOnNewDocumentAsync(stealthScript).ConfigureAwait(false);

            // Also apply to the current page context immediately
            await page.EvaluateExpressionAsync(stealthScript).ConfigureAwait(false);
        }
    }
}
