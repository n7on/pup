using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Services;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupPageScroll")]
    [OutputType(typeof(void))]
    public class InvokePageScrollCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to scroll")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Horizontal scroll position (pixels from left)")]
        public double X { get; set; } = 0;

        [Parameter(HelpMessage = "Vertical scroll position (pixels from top)")]
        public double Y { get; set; } = 0;

        [Parameter(HelpMessage = "Use smooth scrolling")]
        public SwitchParameter Smooth { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                pageService.ScrollToAsync(X, Y, Smooth.IsPresent).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageScrollError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
