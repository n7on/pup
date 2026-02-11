using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Services;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Find, "PupElements")]
    [OutputType(typeof(PupElement[]))]
    public class FindElementsCommand : PupBaseCommand
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
            HelpMessage = "CSS selector or XPath expression to find elements")]
        public string Selector { get; set; }

        [Parameter(HelpMessage = "Use XPath expression instead of CSS selector")]
        public SwitchParameter XPath { get; set; }

        [Parameter(HelpMessage = "Find elements by exact visible text match")]
        public string Text { get; set; }

        [Parameter(HelpMessage = "Find elements containing this text (case-insensitive)")]
        public string TextContains { get; set; }

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

                // Determine if we're doing text-based search
                bool isTextSearch = !string.IsNullOrEmpty(Text) || !string.IsNullOrEmpty(TextContains);

                if (ParameterSetName == "FromPage")
                {
                    var pageService = ServiceFactory.CreatePageService(Page);

                    if (isTextSearch)
                    {
                        // Text-based search
                        bool exactMatch = !string.IsNullOrEmpty(Text);
                        string searchText = exactMatch ? Text : TextContains;
                        results = pageService.FindElementsByTextAsync(searchText, exactMatch, Selector).GetAwaiter().GetResult();

                        if (First.IsPresent && results.Count > 0)
                        {
                            results = new List<PupElement> { results[0] };
                        }
                    }
                    else if (!string.IsNullOrEmpty(Selector))
                    {
                        // CSS/XPath selector search
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
                    else
                    {
                        throw new ArgumentException("Either -Selector, -Text, or -TextContains must be specified.");
                    }
                }
                else if (ParameterSetName == "FromElement")
                {
                    var elementService = new ElementService(Element);

                    if (isTextSearch)
                    {
                        // Text-based search within element
                        bool exactMatch = !string.IsNullOrEmpty(Text);
                        string searchText = exactMatch ? Text : TextContains;
                        results = elementService.FindElementsByTextAsync(searchText, exactMatch, Selector).GetAwaiter().GetResult();

                        if (First.IsPresent && results.Count > 0)
                        {
                            results = new List<PupElement> { results[0] };
                        }
                    }
                    else if (!string.IsNullOrEmpty(Selector))
                    {
                        // CSS/XPath selector search within element
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
                    else
                    {
                        throw new ArgumentException("Either -Selector, -Text, or -TextContains must be specified.");
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