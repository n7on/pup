using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Services;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupPageBack")]
    [OutputType(typeof(void))]
    public class InvokePageBackCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to navigate back")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Wait for page to load after navigation")]
        public SwitchParameter WaitForLoad { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                Page = pageService.NavigateBackAsync(WaitForLoad.IsPresent).GetAwaiter().GetResult();

                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageBackError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}   