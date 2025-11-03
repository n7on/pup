using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Remove, "Page")]
    [OutputType(typeof(PBPage))]
    public class RemovePageCommand : PageBaseCommand
    {
        protected override void ProcessRecord()
        {
            try
            {
                Page = ResolvePageOrThrow();
                PageService.RemovePage(Page);

                WriteVerbose($"Removed browser page: {Page.PageName} (ID: {Page.PageId})");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemovePageFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}