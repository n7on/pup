using System;
using System.Management.Automation;
using Pup.Commands.Base;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Stop, "PupBrowser")]
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
                BrowserService.StopBrowser();

                WriteVerbose($"Browser '{Browser.BrowserType}' stopped successfully!");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StopBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}