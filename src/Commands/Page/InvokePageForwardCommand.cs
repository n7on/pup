using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageForward")]
    [OutputType(typeof(void))]
    public class InvokePageForwardCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Wait for page to load after navigation")]
        public SwitchParameter WaitForLoad { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Page = ResolvePageOrThrow();

                var pageService = ServiceFactory.CreatePageService(SessionState);
                Page = pageService.NavigateForwardAsync(Page, WaitForLoad.IsPresent).GetAwaiter().GetResult();

                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageForwardError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}