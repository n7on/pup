using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Set, "PupElementAttribute")]
    [OutputType(typeof(void))]
    public class SetElementAttributeCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to set attribute on")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Name of the attribute to set")]
        public string Name { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            HelpMessage = "Value for the attribute")]
        public string Value { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Element.Element.EvaluateFunctionAsync("(el, name, value) => el.setAttribute(name, value)", Name, Value).GetAwaiter().GetResult();
                
                WriteVerbose($"Set attribute '{Name}' to '{Value}' on element");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetElementAttributeError", ErrorCategory.WriteError, Element));
            }
        }
    }
}