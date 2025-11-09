using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Completers;
using Pup.Common;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Start, "Browser")]
    [OutputType(typeof(PupBrowser))]
    public class StartBrowserCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            HelpMessage = "Name of the browser to stop (used when Browser parameter is not provided)",
            Mandatory = true)]
        [ArgumentCompleter(typeof(InstalledBrowserCompleter))]
        public string BrowserType { get; set; }
        [Parameter(HelpMessage = "Run browser in headless mode (no GUI)")]
        public SwitchParameter Headless { get; set; }

        [Parameter(HelpMessage = "Additional arguments to pass to the browser")]
        public string[] Arguments { get; set; }

        [Parameter(HelpMessage = "Window width (default: 1280)")]
        public int Width { get; set; } = 1280;

        [Parameter(HelpMessage = "Window height (default: 720)")]
        public int Height { get; set; } = 720;

        protected override void ProcessRecord()
        {
            try
            {
                BrowserTypeValidator.Validate(BrowserType);
                var supportedBrowserService = ServiceFactory.CreateSupportedBrowserService(SessionState);
                var browser = supportedBrowserService.StartBrowser(
                    BrowserType.ToPBSupportedBrowser(),
                    Headless.IsPresent,
                    Width,
                    Height
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