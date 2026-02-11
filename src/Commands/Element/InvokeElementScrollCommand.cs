using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupElementScroll")]
    [OutputType(typeof(void))]
    public class InvokeElementScrollCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to scroll into view")]
        public PupElement Element { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                elementService.ScrollIntoViewAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementScrollFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
