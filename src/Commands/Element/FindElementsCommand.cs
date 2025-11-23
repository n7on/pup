using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Find, "PupElements")]
    [OutputType(typeof(PupElement[]))]
    public class FindElementsCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "FromPage",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Page to search within")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "FromElement", 
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Parent element to search within")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "CSS selector or XPath expression to find elements", 
            Mandatory = true)]
        public string Selector { get; set; }

        [Parameter(HelpMessage = "Use XPath expression instead of CSS selector")]
        public SwitchParameter XPath { get; set; }

        [Parameter(HelpMessage = "Wait for elements to load before returning")]
        public SwitchParameter WaitForLoad { get; set; }

        [Parameter(HelpMessage = "Timeout in milliseconds to wait for elements to appear (default: 5000)")]
        public int Timeout { get; set; } = 5000;

        [Parameter(HelpMessage = "Return only the first element found")]
        public SwitchParameter First { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                List<PupElement> results = new List<PupElement>();

                if (ParameterSetName == "FromPage")
                {
                    // Find elements within a page
                    var pageService = ServiceFactory.CreatePageService(Page);
                    if (First.IsPresent)
                    {
                        var singleElement = XPath.IsPresent
                            ? pageService.FindElementByXPathAsync(Selector, WaitForLoad.IsPresent, Timeout).GetAwaiter().GetResult()
                            : pageService.FindElementBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout).GetAwaiter().GetResult();
                        if (singleElement != null) results.Add(singleElement);
                    }
                    else
                    {
                        results = XPath.IsPresent
                            ? pageService.FindElementsByXPathAsync(Selector, WaitForLoad.IsPresent, Timeout).GetAwaiter().GetResult()
                            : pageService.FindElementsBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout).GetAwaiter().GetResult();
                    }
                }
                else if (ParameterSetName == "FromElement")
                {
                    // Find elements within a parent element using ElementService
                    var elementService = new ElementService(Element);
                    
                    if (First.IsPresent)
                    {
                        var childElement = XPath.IsPresent
                            ? elementService.FindElementByXPathAsync(Selector).GetAwaiter().GetResult()
                            : elementService.FindElementBySelectorAsync(Selector).GetAwaiter().GetResult();
                        if (childElement != null)
                        {
                            results.Add(childElement);
                        }
                    }
                    else
                    {
                        var childElements = XPath.IsPresent
                            ? elementService.FindElementsByXPathAsync(Selector).GetAwaiter().GetResult()
                            : elementService.FindElementsBySelectorAsync(Selector).GetAwaiter().GetResult();
                        results.AddRange(childElements);
                    }
                }

                WriteObject(results.ToArray(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "FindElementsFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}