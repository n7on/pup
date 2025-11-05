using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Move, "Page")]
    [OutputType(typeof(PBPage))]
    public class MovePageCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "URL to navigate", Mandatory = true)]
        public string Url { get; set; }

        [Parameter(HelpMessage = "Wait for page to load completely before returning")]
        public SwitchParameter WaitForLoad { get; set; }
        protected override void ProcessRecord()
        {
            try
            {
                Page = ResolvePageOrThrow();
                Page = PageService.NavigatePageAsync(Page, Url, WaitForLoad.IsPresent).GetAwaiter().GetResult();

                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "MovePageFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}