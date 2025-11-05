using System;
using System.Management.Automation;
using PowerBrowser.Transport;
using PowerBrowser.Services;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageBack")]
    [OutputType(typeof(void))]
    public class InvokePageBackCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Wait for page to load after navigation")]
        public SwitchParameter WaitForLoad { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Page = ResolvePageOrThrow();

                var pageService = ServiceFactory.CreatePageService(SessionState);
                Page = pageService.NavigateBackAsync(Page, WaitForLoad.IsPresent).GetAwaiter().GetResult();

                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageBackError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}   