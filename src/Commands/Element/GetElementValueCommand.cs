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

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                var value = elementService.GetElementValueAsync().GetAwaiter().GetResult();
                WriteObject(value);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementValueError", ErrorCategory.ReadError, Element));
            }
        }
    }
}
