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

        private bool _staleElements;

        protected override void ProcessRecord()
        {
            if (_staleElements) return;

            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                elementService.ScrollIntoViewAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (IsStaleElementException(ex))
            {
                _staleElements = true;
                WriteWarning("Page has navigated — remaining elements are no longer valid. Skipping.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementScrollFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
