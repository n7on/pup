using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Find, "PupElement")]
    [OutputType(typeof(PupElement))]
    public class FindPageElementCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PupPage Page { get; set; }

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
                var pageService = ServiceFactory.CreatePageService(Page);
                var element = pageService.FindElementBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout).GetAwaiter().GetResult();
                WriteObject(element);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "FindPageElementFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}