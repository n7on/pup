using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pup.Transport;

namespace Pup.Services
{
    public interface IRecordingService
    {
        Task StartRecordingAsync(RecordingOptions options);
        Task StopRecordingAsync();
        string ConvertEventsToScript(List<PupRecordingEvent> events, RecordingConvertOptions options);
    }

    public class RecordingConvertOptions
    {
        public string PageVariable { get; set; } = "$page";
        public string BrowserVariable { get; set; } = "$browser";
        public bool IncludeSetup { get; set; }
        public bool IncludeTeardown { get; set; }
        public string StartUrl { get; set; }
        /// <summary>Minimum delay between actions in milliseconds (0 = no delay)</summary>
        public int DelayMin { get; set; } = 0;
        /// <summary>Maximum delay between actions in milliseconds (0 = use DelayMin as fixed delay)</summary>
        public int DelayMax { get; set; } = 0;
        /// <summary>Preserve actual timing from recording (uses timestamps between events)</summary>
        public bool Realtime { get; set; }
        /// <summary>Minimum wait threshold in milliseconds - waits shorter than this are skipped (default 100ms)</summary>
        public int RealtimeThreshold { get; set; } = 100;
    }

    public class RecordingOptions
    {
        public bool IncludeScroll { get; set; }
        public bool IncludeHover { get; set; }
    }

    public class RecordingService : IRecordingService
    {
        private readonly PupPage _page;

        public RecordingService(PupPage page)
        {
            _page = page;
        }

        public async Task StartRecordingAsync(RecordingOptions options)
        {
            if (_page == null)
                throw new InvalidOperationException("RecordingService requires a page for live recording.");

            _page.RecordingActive = true;

            // Capture the starting URL as a "start" event
            var startUrl = await _page.Page.EvaluateExpressionAsync<string>("location.href").ConfigureAwait(false);
            lock (_page.RecordingLock)
            {
                _page.RecordingEvents.Add(new PupRecordingEvent
                {
                    Type = "start",
                    Url = startUrl,
                    Timestamp = DateTime.Now
                });
            }

            var script = GenerateRecordingScript(options);

            // Apply to current page context
            await _page.Page.EvaluateExpressionAsync(script).ConfigureAwait(false);

            // Apply to future navigations
            await _page.Page.EvaluateExpressionOnNewDocumentAsync(script).ConfigureAwait(false);
        }

        public async Task StopRecordingAsync()
        {
            if (_page == null)
                throw new InvalidOperationException("RecordingService requires a page for live recording.");

            // Flush any pending input before stopping, so the final value is captured
            await _page.Page.EvaluateExpressionAsync(@"
                if (window.__pup_flush_pending_input) {
                    window.__pup_flush_pending_input();
                }
            ").ConfigureAwait(false);

            _page.RecordingActive = false;
        }

        private string GenerateRecordingScript(RecordingOptions options)
        {
            var script = EmbeddedResourceService.LoadScript("recording.js");

            // Replace placeholders with actual values
            var includeScroll = options?.IncludeScroll == true ? "true" : "false";
            var includeHover = options?.IncludeHover == true ? "true" : "false";

            script = script.Replace("{{INCLUDE_SCROLL}}", includeScroll);
            script = script.Replace("{{INCLUDE_HOVER}}", includeHover);

            return script;
        }

        public string ConvertEventsToScript(List<PupRecordingEvent> events, RecordingConvertOptions options)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"# Pup Recording");
            sb.AppendLine($"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"# Events: {events.Count}");
            sb.AppendLine();

            if (options.IncludeSetup)
            {
                sb.AppendLine("# Setup");
                sb.AppendLine($"{options.BrowserVariable} = Start-PupBrowser");

                // Get URL from start event, or use provided StartUrl as fallback
                var startUrl = options.StartUrl;
                if (string.IsNullOrEmpty(startUrl))
                {
                    var startEvent = events.FirstOrDefault(e => e.Type == "start");
                    if (startEvent != null)
                    {
                        startUrl = startEvent.Url;
                    }
                }

                if (!string.IsNullOrEmpty(startUrl))
                {
                    var url = EscapeString(startUrl);
                    sb.AppendLine($"{options.PageVariable} = New-PupPage -Browser {options.BrowserVariable} -Url \"{url}\" -WaitForLoad");
                }
                else
                {
                    sb.AppendLine($"{options.PageVariable} = New-PupPage -Browser {options.BrowserVariable}");
                }
                sb.AppendLine();
            }

            // Process events with deduplication and filtering
            var filteredEvents = FilterAndDeduplicateEvents(events);

            // Process events with optional delays
            var hasDelay = options.DelayMin > 0 || options.DelayMax > 0;
            PupRecordingEvent previousEvt = null;
            bool firstStartSkipped = false;
            foreach (var evt in filteredEvents)
            {
                // Skip the first start event if it was used for setup
                if (evt.Type == "start" && options.IncludeSetup && !firstStartSkipped)
                {
                    firstStartSkipped = true;
                    continue;
                }

                var code = GenerateEventCode(evt, options);
                if (!string.IsNullOrEmpty(code))
                {
                    // Add timing between actions
                    if (previousEvt != null)
                    {
                        if (options.Realtime)
                        {
                            // Use actual time difference from recording
                            var elapsed = (int)(evt.Timestamp - previousEvt.Timestamp).TotalMilliseconds;
                            if (elapsed >= options.RealtimeThreshold)
                            {
                                sb.AppendLine($"Start-Sleep -Milliseconds {elapsed}");
                            }
                        }
                        else if (hasDelay)
                        {
                            // Use configured delay
                            if (options.DelayMax > options.DelayMin)
                            {
                                sb.AppendLine($"Start-Sleep -Milliseconds (Get-Random -Minimum {options.DelayMin} -Maximum {options.DelayMax})");
                            }
                            else
                            {
                                sb.AppendLine($"Start-Sleep -Milliseconds {options.DelayMin}");
                            }
                        }
                    }
                    sb.AppendLine(code);
                    previousEvt = evt;
                }
            }

            if (options.IncludeTeardown)
            {
                sb.AppendLine();
                sb.AppendLine("# Teardown");
                sb.AppendLine($"Remove-PupPage -Page {options.PageVariable}");
                sb.AppendLine($"Stop-PupBrowser -Browser {options.BrowserVariable}");
            }

            return sb.ToString();
        }

        private List<PupRecordingEvent> FilterAndDeduplicateEvents(List<PupRecordingEvent> events)
        {
            var result = new List<PupRecordingEvent>();
            PupRecordingEvent previous = null;

            for (int i = 0; i < events.Count; i++)
            {
                var current = events[i];
                var next = i + 1 < events.Count ? events[i + 1] : null;

                // Skip duplicate consecutive events (same type, selector, value)
                if (previous != null && IsDuplicateEvent(previous, current))
                {
                    continue;
                }

                // Skip scroll events immediately before navigation events
                // (user scrolled to see a link, then clicked - scroll isn't needed)
                if (current.Type == "scroll" && next != null && IsNavigationEvent(next))
                {
                    continue;
                }

                // Skip navigate events immediately after a click on a link
                // (the click already handles the navigation with -WaitForLoad)
                if (current.Type == "navigate" && previous != null &&
                    previous.Type == "click" &&
                    string.Equals(previous.TagName, "A", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                result.Add(current);
                previous = current;
            }

            return result;
        }

        private bool IsDuplicateEvent(PupRecordingEvent a, PupRecordingEvent b)
        {
            if (a.Type != b.Type) return false;
            if (a.Selector != b.Selector) return false;

            // For input events, also check value
            if (a.Type == "input" || a.Type == "change")
            {
                return a.Value == b.Value;
            }

            // For scroll events, check position
            if (a.Type == "scroll")
            {
                return a.ScrollX == b.ScrollX && a.ScrollY == b.ScrollY;
            }

            return true;
        }

        private bool IsNavigationEvent(PupRecordingEvent evt)
        {
            var type = evt.Type?.ToLowerInvariant();
            return type == "navigate" || type == "back" || type == "forward" ||
                   (type == "click" && string.Equals(evt.TagName, "A", StringComparison.OrdinalIgnoreCase));
        }

        private string GenerateEventCode(PupRecordingEvent evt, RecordingConvertOptions options)
        {
            switch (evt.Type?.ToLowerInvariant())
            {
                case "start":
                    // Start events (after the first) become navigation commands
                    return GenerateStartEventCode(evt, options);
                case "click":
                    return GenerateClickEventCode(evt, options);
                case "change":
                case "input":
                    return GenerateInputEventCode(evt, options);
                case "keydown":
                    return GenerateKeyEventCode(evt, options);
                case "navigate":
                    return GenerateNavigateEventCode(evt, options);
                case "back":
                    return $"Invoke-PupPageBack -Page {options.PageVariable} -WaitForLoad";
                case "forward":
                    return $"Invoke-PupPageForward -Page {options.PageVariable} -WaitForLoad";
                case "scroll":
                    return GenerateScrollEventCode(evt, options);
                case "wait":
                    return GenerateWaitEventCode(evt);
                default:
                    return $"# Unknown event type: {evt.Type}";
            }
        }

        private string GenerateClickEventCode(PupRecordingEvent evt, RecordingConvertOptions options, bool waitForLoad = false)
        {
            if (string.IsNullOrEmpty(evt.Selector)) return "# Click: No selector";

            var selector = EscapeSingleQuoteString(evt.Selector);
            var clickSwitch = evt.ClickCount > 1 ? " -DoubleClick" : "";
            // Add -WaitForLoad for links since they typically navigate
            var waitSwitch = waitForLoad || string.Equals(evt.TagName, "A", StringComparison.OrdinalIgnoreCase) ? " -WaitForLoad" : "";
            return $"Find-PupElements -Page {options.PageVariable} -Selector '{selector}' -First | Invoke-PupElementClick{clickSwitch}{waitSwitch}";
        }

        private string GenerateInputEventCode(PupRecordingEvent evt, RecordingConvertOptions options)
        {
            if (string.IsNullOrEmpty(evt.Selector)) return "# Input: No selector";

            var selector = EscapeSingleQuoteString(evt.Selector);
            var value = EscapeString(evt.Value ?? "");

            // Handle checkboxes and radios
            if (evt.InputType == "checkbox" || evt.InputType == "radio")
            {
                var checkSwitch = evt.Value == "true" ? "-Check" : "-Uncheck";
                return $"Find-PupElements -Page {options.PageVariable} -Selector '{selector}' -First | Set-PupElementValue {checkSwitch}";
            }

            return $"Find-PupElements -Page {options.PageVariable} -Selector '{selector}' -First | Set-PupElement -Text \"{value}\" -Clear";
        }

        private string GenerateKeyEventCode(PupRecordingEvent evt, RecordingConvertOptions options)
        {
            var key = EscapeString(evt.Key ?? "");

            // Build modifier string if any
            var modifiers = "";
            if (evt.Modifiers != null && evt.Modifiers.Length > 0)
            {
                modifiers = $" -Modifiers {string.Join(",", evt.Modifiers)}";
            }

            return $"Send-PupKey -Page {options.PageVariable} -Key \"{key}\"{modifiers}";
        }

        private string GenerateNavigateEventCode(PupRecordingEvent evt, RecordingConvertOptions options)
        {
            var url = EscapeString(evt.Url ?? "");
            return $"Move-PupPage -Page {options.PageVariable} -Url \"{url}\" -WaitForLoad";
        }

        private string GenerateStartEventCode(PupRecordingEvent evt, RecordingConvertOptions options)
        {
            if (string.IsNullOrEmpty(evt.Url))
                return null;

            var url = EscapeString(evt.Url);
            return $"Move-PupPage -Page {options.PageVariable} -Url \"{url}\" -WaitForLoad";
        }

        private string GenerateWaitEventCode(PupRecordingEvent evt)
        {
            if (int.TryParse(evt.Value, out var ms))
            {
                return $"Start-Sleep -Milliseconds {ms}";
            }
            return $"# Wait: Invalid duration '{evt.Value}'";
        }

        private string GenerateScrollEventCode(PupRecordingEvent evt, RecordingConvertOptions options)
        {
            var x = evt.ScrollX ?? 0;
            var y = evt.ScrollY ?? 0;
            // Use invariant culture for decimal separator (always period, not comma)
            var xStr = x.ToString(CultureInfo.InvariantCulture);
            var yStr = y.ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(evt.Selector))
            {
                // Element scroll - use JavaScript since there's no dedicated element scroll-to command
                // Use double quotes for JS string, escape any quotes in selector
                var selector = evt.Selector.Replace("'", "\\'");
                return $"# Scroll element to ({xStr}, {yStr})\nInvoke-PupPageScript -Page {options.PageVariable} -Script \"() => document.querySelector('{selector}')?.scrollTo({xStr}, {yStr})\" -AsVoid";
            }
            else
            {
                // Page scroll - use dedicated command
                return $"Invoke-PupPageScroll -Page {options.PageVariable} -X {xStr} -Y {yStr}";
            }
        }

        private string EscapeString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // IMPORTANT: Escape backticks FIRST, before adding new ones
            return value
                .Replace("`", "``")
                .Replace("\\", "\\\\")
                .Replace("\"", "`\"")
                .Replace("$", "`$");
        }

        /// <summary>
        /// Escape for single-quoted PowerShell strings (only single quotes need escaping)
        /// </summary>
        private string EscapeSingleQuoteString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // In single-quoted strings, only single quotes need escaping (by doubling)
            return value.Replace("'", "''");
        }
    }
}
