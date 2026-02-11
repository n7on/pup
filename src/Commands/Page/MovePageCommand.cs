using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Move, "PupPage")]
    [OutputType(typeof(PupPage))]
    public class MovePageCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to navigate")]
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