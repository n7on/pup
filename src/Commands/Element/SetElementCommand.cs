using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Set, "PupElement")]
    [OutputType(typeof(void))]
    public class SetElementCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to modify")]
        public PupElement Element { get; set; }

        [Parameter(HelpMessage = "Set the text content of the element (clears existing text first unless -Append is used)")]
        public string Text { get; set; }

        [Parameter(HelpMessage = "Set the value property of the element (for form inputs)")]
        public string Value { get; set; }

        [Parameter(HelpMessage = "Set the innerHTML of the element")]
        public string InnerHTML { get; set; }

        [Parameter(HelpMessage = "Clear the element before setting new content")]
        public SwitchParameter Clear { get; set; }

        [Parameter(HelpMessage = "Append text instead of replacing existing content")]
        public SwitchParameter Append { get; set; }

        [Parameter(HelpMessage = "Focus the element after setting values")]
        public SwitchParameter Focus { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                // Clear element content if requested
                if (Clear.IsPresent)
                {
                    // Clear by selecting all and deleting (works for most input types)
                    elementService.FocusElementAsync().GetAwaiter().GetResult();
                    Element.Element.EvaluateFunctionAsync("el => { el.select && el.select(); el.value = ''; }").GetAwaiter().GetResult();
                }

                // Set text content (typing)
                if (!string.IsNullOrEmpty(Text))
                {
                    // Clear existing text first unless -Append is specified
                    if (!Append.IsPresent && !Clear.IsPresent)
                    {
                        elementService.FocusElementAsync().GetAwaiter().GetResult();
                        Element.Element.EvaluateFunctionAsync("el => { if (el.select) el.select(); else el.setSelectionRange(0, el.value.length); }").GetAwaiter().GetResult();
                    }
                    elementService.SetElementTextAsync(Text).GetAwaiter().GetResult();
                }

                // Set value property
                if (!string.IsNullOrEmpty(Value))
                {
                    elementService.SetElementValueAsync(Value).GetAwaiter().GetResult();
                }

                // Set innerHTML
                if (!string.IsNullOrEmpty(InnerHTML))
                {
                    Element.Element.EvaluateFunctionAsync("(el, html) => el.innerHTML = html", InnerHTML).GetAwaiter().GetResult();
                }

                // Focus element if requested
                if (Focus.IsPresent)
                {
                    elementService.FocusElementAsync().GetAwaiter().GetResult();
                }

                WriteVerbose($"Element properties set successfully");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetElementError", ErrorCategory.WriteError, Element));
            }
        }
    }
}