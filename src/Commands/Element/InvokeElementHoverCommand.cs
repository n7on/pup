using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupElementHover")]
    [OutputType(typeof(void))]
    public class InvokeElementHoverCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "Element",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to hover over")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "Selector",
            Mandatory = true,
            HelpMessage = "CSS selector of element to hover over")]
        public string Selector { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                elementService.HoverElementAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementHoverError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}