using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Get, "PupElementValue")]
    [OutputType(typeof(object))]
    public class GetElementValueCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to read value from")]
        public PupElement Element { get; set; }

        private bool _staleElements;

        protected override void ProcessRecord()
        {
            if (_staleElements) return;

            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                var value = Await(elementService.GetElementValueAsync());
                WriteObject(value);
            }
            catch (Exception ex) when (IsStaleElementException(ex))
            {
                _staleElements = true;
                WriteWarning("Page has navigated — remaining elements are no longer valid. Skipping.");
            }
            catch (PipelineStoppedException) { throw; }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementValueError", ErrorCategory.ReadError, Element));
            }
        }
    }
}
