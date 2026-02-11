using System;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    /// <summary>
    /// Gets a CSS selector for an element. Returns full path by default, or hierarchical selector for similar elements.
    /// Perfect for web scraping workflows - find one element, then get selector to find all similar elements.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "PupElementSelector")]
    [OutputType(typeof(string))]
    public class GetElementSelectorCommand : PupBaseCommand
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The element to get selector for")]
        public PupElement Element { get; set; }

        [Parameter(HelpMessage = "Generate selector for all similar elements at same hierarchical level")]
        public SwitchParameter Similar { get; set; }

        [Parameter(HelpMessage = "Show count of elements this selector would match")]
        public SwitchParameter ShowCount { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = new ElementService(Element);
                string selector;

                if (Similar.IsPresent)
                {
                    // Generate selector for all similar elements at same hierarchical level
                    selector = elementService.GetSimilarElementsSelectorAsync().GetAwaiter().GetResult();
                }
                else
                {
                    // Default: Generate full path selector for this specific element
                    selector = elementService.GetElementSelectorAsync(fullPath: true).GetAwaiter().GetResult();
                }

                if (ShowCount.IsPresent)
                {
                    var count = elementService.CountElementsBySelectorAsync(selector).GetAwaiter().GetResult();
                    WriteInformation(new InformationRecord($"Selector '{selector}' matches {count} element(s)", "ElementCount"));
                }

                WriteObject(selector);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementSelectorFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}