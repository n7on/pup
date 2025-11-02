using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Find, "PageElement")]
    [OutputType(typeof(PBElement))]
    public class FindPageElementCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "CSS selector to find the element", Mandatory = true)]
        public string Selector { get; set; }

        [Parameter(HelpMessage = "Wait for page to load completely before returning")]
        public SwitchParameter WaitForLoad { get; set; }
        [Parameter(
            HelpMessage = "Timeout in milliseconds to wait for element to appear (default: 5000)")]
        public int Timeout { get; set; } = 5000;
        protected override void ProcessRecord()
        {
            try
            {
                Page = ResolvePageOrThrow();
                var element = PageService.FindElementBySelector(Page, Selector, WaitForLoad.IsPresent, Timeout);
                WriteObject(element);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "FindPageElementFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}