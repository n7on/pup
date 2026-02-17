using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.New, "PupPage")]
    [OutputType(typeof(PupPage))]
    public class NewPageCommand : BrowserBaseCommand
    {

        [Parameter(HelpMessage = "Custom name for the page (if not specified, will be Page1, Page2, etc.)")]
        public string Name { get; set; } = string.Empty;

        [Parameter(HelpMessage = "URL to navigate to when creating the page")]
        public string Url { get; set; } = "about:blank";

        [Parameter(HelpMessage = "Wait for page to load completely before returning")]
        public SwitchParameter WaitForLoad { get; set; }

        [Parameter(HelpMessage = "Page width (if not specified, viewport auto-resizes with the browser window)")]
        public int? Width { get; set; }

        [Parameter(HelpMessage = "Page height (if not specified, viewport auto-resizes with the browser window)")]
        public int? Height { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var browser = ResolveBrowserOrThrow();

                var browserService = ServiceFactory.CreateBrowserService(browser, SessionState);
                var browserPage = browserService.CreatePageAsync(
                    Name,
                    Width,
                    Height,
                    Url,
                    WaitForLoad.IsPresent
                ).GetAwaiter().GetResult();
                WriteObject(browserPage);
            }            
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewPageFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
