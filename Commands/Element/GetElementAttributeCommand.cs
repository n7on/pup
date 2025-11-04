using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Element
{
    [Cmdlet(VerbsCommon.Get, "ElementAttribute")]
    [OutputType(typeof(string))]
    public class GetElementAttributeCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to get attribute from")]
        public PBElement Element { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Name of the attribute to get")]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService();
                var attributeValue = elementService.GetElementAttributeAsync(Element, Name).GetAwaiter().GetResult();
                
                WriteObject(attributeValue);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementAttributeError", ErrorCategory.ReadError, Element));
            }
        }
    }
}