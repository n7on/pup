using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Move, "PupPage")]
    [OutputType(typeof(PupPage))]
    public class MovePageCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "URL to navigate", Mandatory = true)]
        public string Url { get; set; }

        [Parameter(HelpMessage = "Wait for page to load completely before returning")]
        public SwitchParameter WaitForLoad { get; set; }
        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                Page = pageService.NavigatePageAsync(Url, WaitForLoad.IsPresent).GetAwaiter().GetResult();

                WriteObject(Page);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "MovePageFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}