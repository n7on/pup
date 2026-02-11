using System;
using System.Management.Automation;
using System.Runtime.InteropServices.ComTypes;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Wait, "PupElement")]
    [OutputType(typeof(PupElement))]
    public class WaitElementCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to wait for element on")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "CSS selector to wait for")]
        public string Selector { get; set; }

        [Parameter(HelpMessage = "Timeout in milliseconds to wait for element (default: 30000)")]
        public int Timeout { get; set; } = 30000;

        [Parameter(HelpMessage = "Polling interval in milliseconds (default: 200)")]
        public int PollingInterval { get; set; } = 200;

        [Parameter(HelpMessage = "Wait for element to be visible (default: false - just present in DOM)")]
        public SwitchParameter Visible { get; set; }

        [Parameter(HelpMessage = "Wait for element to be hidden/removed")]
        public SwitchParameter Hidden { get; set; }

        [Parameter(HelpMessage = "Wait for element to be enabled")]
        public SwitchParameter Enabled { get; set; }

        [Parameter(HelpMessage = "Wait for element to be disabled")]
        public SwitchParameter Disabled { get; set; }

        [Parameter(HelpMessage = "Wait until element's text contains this value")]
        public string TextContains { get; set; }

        [Parameter(HelpMessage = "Wait until an attribute equals the specified value")]
        public string AttributeValue { get; set; }

        [Parameter(HelpMessage = "Name of the attribute to check when using -AttributeValue")]
        public string AttributeName { get; set; }

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
                    pageService.WaitForElementConditionAsync(Selector, "hidden", null, null, null, Timeout, PollingInterval).GetAwaiter().GetResult();
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
                        pageService.WaitForElementConditionAsync(Selector, "visible", null, null, null, Timeout, PollingInterval).GetAwaiter().GetResult();
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' is now visible");
                    }
                    else if (Enabled.IsPresent)
                    {
                        pageService.WaitForElementConditionAsync(Selector, "enabled", null, null, null, Timeout, PollingInterval).GetAwaiter().GetResult();
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' is now enabled");
                    }
                    else if (Disabled.IsPresent)
                    {
                        pageService.WaitForElementConditionAsync(Selector, "disabled", null, null, null, Timeout, PollingInterval).GetAwaiter().GetResult();
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' is now disabled");
                    }
                    else if (!string.IsNullOrEmpty(TextContains))
                    {
                        pageService.WaitForElementConditionAsync(Selector, "textContains", TextContains, null, null, Timeout, PollingInterval).GetAwaiter().GetResult();
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' contains text '{TextContains}'");
                    }
                    else if (!string.IsNullOrEmpty(AttributeName))
                    {
                        pageService.WaitForElementConditionAsync(Selector, "attributeEquals", null, AttributeName, AttributeValue, Timeout, PollingInterval).GetAwaiter().GetResult();
                        element = pageService.FindElementBySelectorAsync(Selector, waitForLoad: false, Timeout).GetAwaiter().GetResult();
                        WriteVerbose($"Element with selector '{Selector}' has attribute '{AttributeName}' equal to '{AttributeValue}'");
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
