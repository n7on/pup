using System;
using System.Management.Automation;

namespace PowerBrowser.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Stop, "Browser")]
    [OutputType(typeof(string))]
    public class StopBrowserCommand : BrowserBaseCommand
    {

        [Parameter(HelpMessage = "Force stop without confirmation")]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Browser = ResolveBrowserOrThrow();
                BrowserService.StopBrowser(Browser);

                WriteVerbose($"Browser '{Browser.BrowserType}' stopped successfully!");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StopBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}