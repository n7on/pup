using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupElementClick")]
    [OutputType(typeof(void))]
    public class InvokeElementClickCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "Element",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to click")]
        public PupElement Element { get; set; }


        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                elementService.ClickElementAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementClickFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}