using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Completers;
using Pup.Common;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Start, "PupBrowser")]
    [OutputType(typeof(PupBrowser))]
    public class StartBrowserCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            HelpMessage = "Name of the browser to stop (used when Browser parameter is not provided)",
            Mandatory = false)]
        [ArgumentCompleter(typeof(InstalledBrowserCompleter))]
        public string BrowserType { get; set; } = "Chrome";
        [Parameter(HelpMessage = "Run browser in headless mode (no GUI)")]
        public SwitchParameter Headless { get; set; }

        [Parameter(HelpMessage = "Additional arguments to pass to the browser")]
        public string[] Arguments { get; set; }

        [Parameter(HelpMessage = "Proxy server URL (e.g., 127.0.0.1:8080 for Burp Suite)")]
        public string Proxy { get; set; }

        [Parameter(HelpMessage = "Custom User-Agent string. Defaults to a realistic Chrome UA. Use 'none' for the browser's native UA.")]
        public string UserAgent { get; set; }

        [Parameter(HelpMessage = "Window width (default: 1280)")]
        public int Width { get; set; } = 1280;

        [Parameter(HelpMessage = "Window height (default: 720)")]
        public int Height { get; set; } = 720;

        [Parameter(HelpMessage = "Force start a new browser even if one is already running")]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                BrowserTypeValidator.Validate(BrowserType);
                var supportedBrowserService = ServiceFactory.CreateSupportedBrowserService(SessionState);
                
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

                var browser = supportedBrowserService.StartBrowser(
                    BrowserType.ToPBSupportedBrowser(),
                    Headless.IsPresent,
                    Width,
                    Height,
                    Proxy,
                    UserAgent,
                    Arguments
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