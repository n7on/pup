using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupPageReload")]
    [OutputType(typeof(void))]
    public class InvokePageReloadCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to reload")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Wait for page to load after reload")]
        public SwitchParameter WaitForLoad { get; set; }

        [Parameter(HelpMessage = "Ignore cache and force reload from server")]
        public SwitchParameter HardReload { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                Page = pageService.ReloadPageAsync(WaitForLoad.IsPresent).GetAwaiter().GetResult();
                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageReloadError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}