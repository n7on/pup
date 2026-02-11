using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupElementClick")]
    [OutputType(typeof(void))]
    public class InvokeElementClickCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to click")]
        public PupElement Element { get; set; }

        [Parameter(HelpMessage = "Number of clicks (2 for double-click)")]
        [ValidateRange(1, 3)]
        public int ClickCount { get; set; } = 1;

        [Parameter(HelpMessage = "Perform a double-click")]
        public SwitchParameter DoubleClick { get; set; }

        [Parameter(HelpMessage = "Wait for page to load after click (useful for links)")]
        public SwitchParameter WaitForLoad { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);
                var clicks = DoubleClick.IsPresent ? 2 : ClickCount;
                elementService.ClickElementAsync(clicks, WaitForLoad.IsPresent).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeElementClickFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
