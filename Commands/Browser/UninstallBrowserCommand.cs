using System;
using System.Management.Automation;

namespace PowerBrowser.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Uninstall, "Browser")]
    [OutputType(typeof(string))]
    public class UninstallBrowserCommand : BrowserBaseCommand
    {
        [Parameter(HelpMessage = "Remove without confirmation")]
        public SwitchParameter Force { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Browser = ResolveBrowserOrThrow();
                BrowserService.RemoveBrowser(Browser);

                WriteVerbose($"Removed browser: {Browser.BrowserType} from path: {Browser.Path}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "UninstallBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
