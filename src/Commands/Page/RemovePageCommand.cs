using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Remove, "PupPage")]
    [OutputType(typeof(PupPage))]
    public class RemovePageCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to close")]
        public PupPage Page { get; set; }
        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                pageService.RemovePageAsync().GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemovePageFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}