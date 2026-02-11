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

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                var attributeValue = elementService.GetElementAttributeAsync(Name).GetAwaiter().GetResult();

                WriteObject(attributeValue);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementAttributeError", ErrorCategory.ReadError, Element));
            }
        }
    }
}