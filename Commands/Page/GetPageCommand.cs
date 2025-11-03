using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "Page")]
    [OutputType(typeof(PBPage))]
    public class GetPageCommand : PSCmdlet
    {

        [Parameter(
            Position = 0,
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PBBrowser Browser { get; set; }
        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(SessionState);
                var pages = Browser != null ? pageService.GetPagesByBrowser(Browser) : pageService.GetPages();

                if (pages.Count == 0)
                {
                    WriteWarning("No browser pages found.");
                    return;
                }

                foreach (var page in pages)
                {
                    WriteObject(page);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}