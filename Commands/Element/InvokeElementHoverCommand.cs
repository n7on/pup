using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "ElementHover")]
    [OutputType(typeof(void))]
    public class InvokeElementHoverCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "Element",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to hover over")]
        public PBElement Element { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "Selector",
            Mandatory = true,
            HelpMessage = "CSS selector of element to hover over")]
        public string Selector { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService();

                switch (ParameterSetName)
                {
                    case "Element":
                        elementService.HoverElementAsync(Element).GetAwaiter().GetResult();
                        WriteVerbose($"Hovered over element");
                        break;

                    case "Selector":
                        var page = ResolvePageOrThrow();
                        elementService.HoverElementBySelectorAsync(page, Selector).GetAwaiter().GetResult();
                        WriteVerbose($"Hovered over element with selector '{Selector}'");
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementHoverError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}