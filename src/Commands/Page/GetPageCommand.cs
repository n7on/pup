using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPage")]
    [OutputType(typeof(PupPage))]
    public class GetPageCommand : BrowserBaseCommand
    {

        protected override void ProcessRecord()
        {
            try
            {
                Browser = ResolveBrowserOrThrow();
                var pageService = ServiceFactory.CreateBrowserService(Browser, SessionState);
                var pages = pageService.GetPagesAsync().GetAwaiter().GetResult();

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