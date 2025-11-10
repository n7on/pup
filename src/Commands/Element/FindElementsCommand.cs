using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Find, "PupElements")]
    [OutputType(typeof(PupElement[]))]
    public class FindPageElementsCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "CSS selector to find elements", Mandatory = true)]
        public string Selector { get; set; }

        [Parameter(HelpMessage = "Wait for elements to load before returning")]
        public SwitchParameter WaitForLoad { get; set; }

        [Parameter(HelpMessage = "Timeout in milliseconds to wait for elements to appear (default: 5000)")]
        public int Timeout { get; set; } = 5000;

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var elements = pageService.FindElementsBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout).GetAwaiter().GetResult();
                
                WriteObject(elements.ToArray(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "FindElementsFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}