using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupPageForward")]
    [OutputType(typeof(void))]
    public class InvokePageForwardCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to navigate forward")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Wait for page to load after navigation")]
        public SwitchParameter WaitForLoad { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                Page = pageService.NavigateForwardAsync(WaitForLoad.IsPresent).GetAwaiter().GetResult();

                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageForwardError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}