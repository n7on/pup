using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Element
{
    [Cmdlet(VerbsCommon.Set, "ElementAttribute")]
    [OutputType(typeof(void))]
    public class SetElementAttributeCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to set attribute on")]
        public PBElement Element { get; set; }

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