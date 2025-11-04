using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "ElementClick")]
    [OutputType(typeof(void))]
    public class InvokeElementClickCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "Element",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to click")]
        public PBElement Element { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "Selector", 
            Mandatory = true,
            HelpMessage = "CSS selector of element to click")]
        public string Selector { get; set; }

        [Parameter(
            ParameterSetName = "Coordinates",
            Mandatory = true,
            HelpMessage = "X coordinate to click")]
        public double X { get; set; }

        [Parameter(
            ParameterSetName = "Coordinates",
            Mandatory = true, 
            HelpMessage = "Y coordinate to click")]
        public double Y { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService();

                switch (ParameterSetName)
                {
                    case "Element":
                        elementService.ClickElementAsync(Element).GetAwaiter().GetResult();
                        break;

                    case "Selector":
                        var page = ResolvePageOrThrow();
                        elementService.ClickElementBySelectorAsync(page, Selector).GetAwaiter().GetResult();
                        break;

                    case "Coordinates":
                        var coordPage = ResolvePageOrThrow();
                        elementService.ClickElementByCoordinatesAsync(coordPage, X, Y).GetAwaiter().GetResult();
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementClickFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}