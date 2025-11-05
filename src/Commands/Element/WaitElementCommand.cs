using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Wait, "Element")]
    [OutputType(typeof(PBElement))]
    public class WaitElementCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
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
                var page = ResolvePageOrThrow();
                var elementService = ServiceFactory.CreateElementService();

                if (Hidden.IsPresent)
                {
                    // Wait for element to be hidden/removed
                    elementService.WaitForElementToBeHiddenAsync(page, Selector, Timeout).GetAwaiter().GetResult();
                    WriteVerbose($"Element with selector '{Selector}' is now hidden");
                    
                    if (PassThru.IsPresent)
                    {
                        WriteObject(null); // Element is hidden, so return null
                    }
                }
                else
                {
                    PBElement element;
                    
                    if (Visible.IsPresent)
                    {
                        // Wait for element to be visible, then find it
                        elementService.WaitForElementToBeVisibleAsync(page, Selector, Timeout).GetAwaiter().GetResult();
                        element = elementService.FindElementBySelectorAsync(page, Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' is now visible");
                    }
                    else
                    {
                        // Use FindElementBySelector with waitForLoad=true for DOM presence
                        element = elementService.FindElementBySelectorAsync(page, Selector, waitForLoad: true, Timeout).GetAwaiter().GetResult();
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