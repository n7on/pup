using System;
using System.Management.Automation;
using System.Runtime.InteropServices.ComTypes;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Wait, "PupElement")]
    [OutputType(typeof(PupElement))]
    public class WaitElementCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "CSS selector to wait for")]
        public string Selector { get; set; }

        [Parameter(HelpMessage = "Timeout in milliseconds to wait for element (default: 30000)")]
        public int Timeout { get; set; } = 30000;

        [Parameter(HelpMessage = "Wait for element to be visible (default: false - just present in DOM)")]
        public SwitchParameter Visible { get; set; }

        [Parameter(HelpMessage = "Wait for element to be hidden/removed")]
        public SwitchParameter Hidden { get; set; }

        [Parameter(HelpMessage = "Return the element after waiting (default: true)")]
        public SwitchParameter PassThru { get; set; } = true;

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                if (Hidden.IsPresent)
                {
                    // Wait for element to be hidden/removed
                    pageService.WaitForElementToBeHiddenAsync(Selector, Timeout).GetAwaiter().GetResult();
                    WriteVerbose($"Element with selector '{Selector}' is now hidden");
                    
                    if (PassThru.IsPresent)
                    {
                        WriteObject(null); // Element is hidden, so return null
                    }
                }
                else
                {
                    PupElement element;
                    
                    if (Visible.IsPresent)
                    {
                        // Wait for element to be visible, then find it
                        pageService.WaitForElementToBeVisibleAsync(Selector, Timeout).GetAwaiter().GetResult();
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' is now visible");
                    }
                    else
                    {
                        // Use FindElementBySelector with waitForLoad=true for DOM presence
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: true, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' is now present in DOM");
                    }
                    
                    if (PassThru.IsPresent)
                    {
                        WriteObject(element);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "WaitElementError", ErrorCategory.OperationStopped, Selector));
            }
        }
    }
}