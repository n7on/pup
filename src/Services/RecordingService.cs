using System;
using System.Collections.Generic;
using System.Text;
using Pup.Transport;

namespace Pup.Services
{
    public interface IRecordingService
    {
        string ConvertToScript(PupRecording recording, RecordingConvertOptions options);
    }

    public class RecordingConvertOptions
    {
        public string PageVariable { get; set; } = "$page";
        public string BrowserVariable { get; set; } = "$browser";
        public bool IncludeSetup { get; set; }
        public bool IncludeTeardown { get; set; }
    }

    public class RecordingService : IRecordingService
    {
        public string ConvertToScript(PupRecording recording, RecordingConvertOptions options)
        {
            var sb = new StringBuilder();

            // Header comment
            sb.AppendLine($"# Pup Recording: {recording.Title ?? "Untitled"}");
            sb.AppendLine($"# Converted from Chrome DevTools Recorder");
            sb.AppendLine($"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine();

            // Setup
            if (options.IncludeSetup)
            {
                sb.AppendLine("# Setup");
                sb.AppendLine($"{options.BrowserVariable} = Start-PupBrowser -Headless");
                sb.AppendLine($"{options.PageVariable} = New-PupPage -Browser {options.BrowserVariable}");
                sb.AppendLine();
            }

            // Steps
            sb.AppendLine("# Recording Steps");
            foreach (var step in recording.Steps)
            {
                var stepCode = GenerateStepCode(step, options);
                if (!string.IsNullOrEmpty(stepCode))
                {
                    sb.AppendLine(stepCode);
                }
            }

            // Teardown
            if (options.IncludeTeardown)
            {
                sb.AppendLine();
                sb.AppendLine("# Teardown");
                sb.AppendLine($"Remove-PupPage -Page {options.PageVariable}");
                sb.AppendLine($"Stop-PupBrowser -Browser {options.BrowserVariable}");
            }

            return sb.ToString();
        }

        private string GenerateStepCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            switch (step.Type?.ToLowerInvariant())
            {
                case "setviewport":
                    return GenerateViewportCode(step, options);

                case "navigate":
                    return GenerateNavigateCode(step, options);

                case "click":
                    return GenerateClickCode(step, options, doubleClick: false);

                case "doubleclick":
                    return GenerateClickCode(step, options, doubleClick: true);

                case "change":
                    return GenerateChangeCode(step, options);

                case "keydown":
                    return GenerateKeyCode(step, options);

                case "keyup":
                    return null; // Usually paired with keydown, skip

                case "scroll":
                    return GenerateScrollCode(step, options);

                case "hover":
                    return GenerateHoverCode(step, options);

                case "waitforelement":
                    return GenerateWaitCode(step, options);

                default:
                    return $"# Unsupported step type: {step.Type}";
            }
        }

        private string GenerateViewportCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var sb = new StringBuilder();
            sb.Append($"Set-PupPageViewport -Page {options.PageVariable}");
            sb.Append($" -Width {step.Width ?? 1920}");
            sb.Append($" -Height {step.Height ?? 1080}");

            if (step.DeviceScaleFactor.HasValue && step.DeviceScaleFactor != 1)
                sb.Append($" -DeviceScaleFactor {step.DeviceScaleFactor}");
            if (step.IsMobile == true)
                sb.Append(" -IsMobile");
            if (step.HasTouch == true)
                sb.Append(" -HasTouch");
            if (step.IsLandscape == true)
                sb.Append(" -IsLandscape");

            return sb.ToString();
        }

        private string GenerateNavigateCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var url = EscapeString(step.Url);
            return $"Move-PupPage -Page {options.PageVariable} -Url \"{url}\" -WaitForLoad";
        }

        private string GenerateClickCode(PupRecordingStep step, RecordingConvertOptions options, bool doubleClick)
        {
            var selector = GetBestSelector(step.Selectors);
            if (selector == null) return "# Click: No selector found";

            var sb = new StringBuilder();
            sb.AppendLine($"$element = Find-PupElements -Page {options.PageVariable} -Selector \"{EscapeString(selector)}\" -First");

            if (doubleClick)
                sb.Append("$element | Invoke-PupElementClick -DoubleClick");
            else
                sb.Append("$element | Invoke-PupElementClick");

            return sb.ToString();
        }

        private string GenerateChangeCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var selector = GetBestSelector(step.Selectors);
            if (selector == null) return "# Change: No selector found";

            var value = EscapeString(step.Value ?? "");
            var sb = new StringBuilder();
            sb.AppendLine($"$element = Find-PupElements -Page {options.PageVariable} -Selector \"{EscapeString(selector)}\" -First");
            sb.Append($"Set-PupElement -Element $element -Text \"{value}\" -Clear");

            return sb.ToString();
        }

        private string GenerateKeyCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var key = step.Key ?? "";
            return $"Send-PupKey -Page {options.PageVariable} -Key \"{EscapeString(key)}\"";
        }

        private string GenerateScrollCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var selector = GetBestSelector(step.Selectors);
            if (selector != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"$element = Find-PupElements -Page {options.PageVariable} -Selector \"{EscapeString(selector)}\" -First");
                sb.Append("$element | Invoke-PupElementScroll");
                return sb.ToString();
            }

            // Page scroll via JavaScript
            var x = step.X ?? 0;
            var y = step.Y ?? 0;
            return $"Invoke-PupPageScript -Page {options.PageVariable} -Script \"() => window.scrollTo({x}, {y})\" -AsVoid";
        }

        private string GenerateHoverCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var selector = GetBestSelector(step.Selectors);
            if (selector == null) return "# Hover: No selector found";

            var sb = new StringBuilder();
            sb.AppendLine($"$element = Find-PupElements -Page {options.PageVariable} -Selector \"{EscapeString(selector)}\" -First");
            sb.Append("$element | Invoke-PupElementHover");

            return sb.ToString();
        }

        private string GenerateWaitCode(PupRecordingStep step, RecordingConvertOptions options)
        {
            var selector = GetBestSelector(step.Selectors);
            if (selector == null) return "# Wait: No selector found";

            var sb = new StringBuilder();
            sb.Append($"Wait-PupElement -Page {options.PageVariable} -Selector \"{EscapeString(selector)}\"");

            if (step.Visible == true)
                sb.Append(" -Visible");
            if (step.Timeout.HasValue)
                sb.Append($" -Timeout {step.Timeout}");

            return sb.ToString();
        }

        private string GetBestSelector(List<List<string>> selectors)
        {
            if (selectors == null || selectors.Count == 0)
                return null;

            // Prefer CSS selectors over XPath, avoid pierce/aria selectors
            foreach (var selectorGroup in selectors)
            {
                if (selectorGroup == null || selectorGroup.Count == 0)
                    continue;

                var selector = selectorGroup[0];

                // Skip unsupported selector types
                if (selector.StartsWith("pierce/") || selector.StartsWith("aria/"))
                    continue;

                // Convert xpath selector
                if (selector.StartsWith("xpath/"))
                    continue;

                return selector;
            }

            // Fallback: try first available selector
            foreach (var selectorGroup in selectors)
            {
                if (selectorGroup != null && selectorGroup.Count > 0)
                {
                    var selector = selectorGroup[0];
                    if (selector.StartsWith("xpath/"))
                        return selector.Substring(6); // Remove "xpath/" prefix
                    return selector;
                }
            }

            return null;
        }

        private string EscapeString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "`\"")
                .Replace("$", "`$")
                .Replace("`", "``");
        }
    }
}
