using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Set, "PupPageViewport")]
    [OutputType(typeof(void))]
    public class SetPageViewportCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PupPage Page { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public int Width { get; set; }

        [Parameter(Mandatory = true, Position = 2)]
        public int Height { get; set; }

        [Parameter()]
        public double DeviceScaleFactor { get; set; } = 1;

        [Parameter()]
        public SwitchParameter IsMobile { get; set; }

        [Parameter()]
        public SwitchParameter HasTouch { get; set; }

        [Parameter()]
        public SwitchParameter IsLandscape { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                pageService.SetViewportAsync(
                    Width,
                    Height,
                    DeviceScaleFactor,
                    IsMobile.IsPresent,
                    HasTouch.IsPresent,
                    IsLandscape.IsPresent
                ).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetPageViewportFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
