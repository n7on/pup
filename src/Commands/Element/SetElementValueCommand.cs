using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Set, "PupElementValue")]
    [OutputType(typeof(void))]
    public class SetElementValueCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to set")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Value to set (string/number/bool or array for multi-select)")]
        public object Value { get; set; }

        [Parameter(HelpMessage = "Values for multi-select elements")]
        public object[] Values { get; set; }

        [Parameter(HelpMessage = "Mark checkbox/radio as checked")]
        public SwitchParameter Check { get; set; }

        [Parameter(HelpMessage = "Mark checkbox/radio as unchecked")]
        public SwitchParameter Uncheck { get; set; }

        [Parameter(HelpMessage = "Skip firing input/change events")]
        public SwitchParameter NoEvents { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                object valueToSet = null;
                if (Values != null && Values.Length > 0)
                {
                    valueToSet = Values;
                }
                else if (Check.IsPresent)
                {
                    valueToSet = true;
                }
                else if (Uncheck.IsPresent)
                {
                    valueToSet = false;
                }
                else
                {
                    valueToSet = Value;
                }

                elementService.SetElementFormValueAsync(valueToSet, triggerChange: !NoEvents.IsPresent).GetAwaiter().GetResult();
                WriteVerbose("Element value set successfully");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetElementValueError", ErrorCategory.WriteError, Element));
            }
        }
    }
}
