using System;
using System.Management.Automation;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Uninstall, "PupBrowser", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
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
                
                if (ShouldProcess($"Browser: {Browser.BrowserType}", "Uninstall"))
                {
                    BrowserService.RemoveBrowser();
                    WriteVerbose($"Removed browser: {Browser.BrowserType} from path: {Browser.Path}");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "UninstallBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
