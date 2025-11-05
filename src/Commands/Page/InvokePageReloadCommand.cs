using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageReload")]
    [OutputType(typeof(void))]
    public class InvokePageReloadCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Wait for page to load after reload")]
        public SwitchParameter WaitForLoad { get; set; }

        [Parameter(HelpMessage = "Ignore cache and force reload from server")]
        public SwitchParameter HardReload { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Page = ResolvePageOrThrow();
                var pageService = ServiceFactory.CreatePageService(SessionState);
                Page = pageService.ReloadPageAsync(Page, WaitForLoad.IsPresent).GetAwaiter().GetResult();
                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageReloadError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}