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
            Position = 0,
            ParameterSetName = "FromFrame",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Frame to search within")]
        public PupFrame Frame { get; set; }

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

        [Parameter(HelpMessage = "Return only elements that are visible (have layout dimensions and not hidden by CSS)")]
        public SwitchParameter Visible { get; set; }

        private bool _staleElements;

        protected override void ProcessRecord()
        {
            if (_staleElements) return;

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
                        results = Await(pageService.FindElementsByTextAsync(searchText, exactMatch, Selector));

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
                                ? Await(pageService.FindElementByXPathAsync(Selector, WaitForLoad.IsPresent, Timeout))
                                : Await(pageService.FindElementBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout));
                            if (singleElement != null) results.Add(singleElement);
                        }
                        else
                        {
                            results = XPath.IsPresent
                                ? Await(pageService.FindElementsByXPathAsync(Selector, WaitForLoad.IsPresent, Timeout))
                                : Await(pageService.FindElementsBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout));
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
                        results = Await(elementService.FindElementsByTextAsync(searchText, exactMatch, Selector));

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
                                ? Await(elementService.FindElementByXPathAsync(Selector))
                                : Await(elementService.FindElementBySelectorAsync(Selector));
                            if (childElement != null)
                            {
                                results.Add(childElement);
                            }
                        }
                        else
                        {
                            var childElements = XPath.IsPresent
                                ? Await(elementService.FindElementsByXPathAsync(Selector))
                                : Await(elementService.FindElementsBySelectorAsync(Selector));
                            results.AddRange(childElements);
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Either -Selector, -Text, or -TextContains must be specified.");
                    }
                }
                else if (ParameterSetName == "FromFrame")
                {
                    var frameService = ServiceFactory.CreateFrameService(Frame);

                    if (isTextSearch)
                    {
                        bool exactMatch = !string.IsNullOrEmpty(Text);
                        string searchText = exactMatch ? Text : TextContains;
                        results = Await(frameService.FindElementsByTextAsync(searchText, exactMatch, Selector));

                        if (First.IsPresent && results.Count > 0)
                        {
                            results = new List<PupElement> { results[0] };
                        }
                    }
                    else if (!string.IsNullOrEmpty(Selector))
                    {
                        if (First.IsPresent)
                        {
                            var frameElement = XPath.IsPresent
                                ? Await(frameService.FindElementByXPathAsync(Selector, WaitForLoad.IsPresent, Timeout))
                                : Await(frameService.FindElementBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout));
                            if (frameElement != null)
                            {
                                results.Add(frameElement);
                            }
                        }
                        else
                        {
                            results = XPath.IsPresent
                                ? Await(frameService.FindElementsByXPathAsync(Selector, WaitForLoad.IsPresent, Timeout))
                                : Await(frameService.FindElementsBySelectorAsync(Selector, WaitForLoad.IsPresent, Timeout));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Either -Selector, -Text, or -TextContains must be specified.");
                    }
                }

                if (Visible.IsPresent && results.Count > 0)
                {
                    results = results.FindAll(el => el.IsVisible);
                }

                WriteObject(results.ToArray(), true);
            }
            catch (Exception ex) when (IsStaleElementException(ex))
            {
                _staleElements = true;
                WriteWarning("Page has navigated — remaining elements are no longer valid. Skipping.");
            }
            catch (PipelineStoppedException) { throw; }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "FindElementsFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}