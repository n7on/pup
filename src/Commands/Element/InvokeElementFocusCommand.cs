using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupElementFocus")]
    [OutputType(typeof(void))]
    public class InvokeElementFocusCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "Element",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to focus")]
        public PupElement Element { get; set; }

        private bool _staleElements;

        protected override void ProcessRecord()
        {
            if (_staleElements) return;

            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                elementService.FocusElementAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex) when (IsStaleElementException(ex))
            {
                _staleElements = true;
                WriteWarning("Page has navigated — remaining elements are no longer valid. Skipping.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementFocusError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}