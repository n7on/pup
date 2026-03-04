using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Get, "PupElementAttribute")]
    [OutputType(typeof(string))]
    public class GetElementAttributeCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to get attribute from")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Name of the attribute to get")]
        public string Name { get; set; }

        private bool _staleElements;

        protected override void ProcessRecord()
        {
            if (_staleElements) return;

            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                var attributeValue = Await(elementService.GetElementAttributeAsync(Name));

                WriteObject(attributeValue);
            }
            catch (Exception ex) when (IsStaleElementException(ex))
            {
                _staleElements = true;
                WriteWarning("Page has navigated — remaining elements are no longer valid. Skipping.");
            }
            catch (PipelineStoppedException) { throw; }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementAttributeError", ErrorCategory.ReadError, Element));
            }
        }
    }
}