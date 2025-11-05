using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "ElementFocus")]
    [OutputType(typeof(void))]
    public class InvokeElementFocusCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "Element",
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to focus")]
        public PBElement Element { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "Selector",
            Mandatory = true,
            HelpMessage = "CSS selector of element to focus")]
        public string Selector { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService();

                switch (ParameterSetName)
                {
                    case "Element":
                        elementService.FocusElementAsync(Element).GetAwaiter().GetResult();
                        WriteVerbose($"Focused on element");
                        break;

                    case "Selector":
                        var page = ResolvePageOrThrow();
                        elementService.FocusElementBySelectorAsync(page, Selector).GetAwaiter().GetResult();
                        WriteVerbose($"Focused on element with selector '{Selector}'");
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementFocusError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}