using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Completers;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Start, "PupBrowser")]
    [OutputType(typeof(PupBrowser))]
    public class StartBrowserCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            HelpMessage = "Name of the browser to stop (used when Browser parameter is not provided)",
            Mandatory = false)]
        [ArgumentCompleter(typeof(InstalledBrowserCompleter))]
        public string BrowserType { get; set; } = "Chrome";
        [Parameter(HelpMessage = "Run browser in headless mode (no GUI)")]
        public SwitchParameter Headless { get; set; }

        [Parameter(HelpMessage = "Start browser in fullscreen mode (hides browser UI)")]
        public SwitchParameter Fullscreen { get; set; }

        [Parameter(HelpMessage = "Start browser maximized")]
        public SwitchParameter Maximized { get; set; }

        [Parameter(HelpMessage = "Additional arguments to pass to the browser")]
        public string[] Arguments { get; set; }

        [Parameter(HelpMessage = "Proxy server URL (e.g., 127.0.0.1:8080 for Burp Suite)")]
        public string Proxy { get; set; }

        [Parameter(HelpMessage = "Custom User-Agent string. Defaults to a realistic Chrome UA. Use 'none' for the browser's native UA.")]
        public string UserAgent { get; set; }

        [Parameter(HelpMessage = "Viewport width (if not specified, viewport auto-resizes with the browser window)")]
        public int? Width { get; set; }

        [Parameter(HelpMessage = "Viewport height (if not specified, viewport auto-resizes with the browser window)")]
        public int? Height { get; set; }

        [Parameter(HelpMessage = "Force start a new browser even if one is already running")]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                BrowserTypeValidator.Validate(BrowserType);
                var supportedBrowserService = ServiceFactory.CreateSupportedBrowserService();
                
                // Check if browser is already running
                var existingBrowser = supportedBrowserService.GetBrowser(BrowserType.ToPBSupportedBrowser());
                if (existingBrowser != null && existingBrowser.Running && !Force.IsPresent)
                {
                    WriteWarning($"Browser '{BrowserType}' is already running. Use -Force to start a new instance or use Stop-PupBrowser first.");
                    WriteObject(existingBrowser);
                    return;
                }

                // If Force is specified and browser exists, stop it first
                if (existingBrowser != null && existingBrowser.Running && Force.IsPresent)
                {
                    WriteVerbose($"Stopping existing '{BrowserType}' browser instance...");
                    existingBrowser.Browser.CloseAsync().GetAwaiter().GetResult();
                }

                // Build arguments list
                var args = Arguments != null ? new System.Collections.Generic.List<string>(Arguments) : new System.Collections.Generic.List<string>();
                if (Fullscreen.IsPresent)
                {
                    args.Add("--start-fullscreen");
                }
                if (Maximized.IsPresent)
                {
                    args.Add("--start-maximized");
                }

                var browser = supportedBrowserService.StartBrowser(
                    BrowserType.ToPBSupportedBrowser(),
                    Headless.IsPresent,
                    Width,
                    Height,
                    Proxy,
                    UserAgent,
                    args.Count > 0 ? args.ToArray() : null
                );
                WriteObject(browser);
            }
            catch (PipelineStoppedException)
            {
                // Swallow the exception as it indicates the pipeline was stopped intentionally
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StartBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}