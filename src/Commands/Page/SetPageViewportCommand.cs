using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Set, "PupPageViewport")]
    [OutputType(typeof(void))]
    public class SetPageViewportCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The page to set viewport for")]
        public PupPage Page { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Viewport width in pixels")]
        public int Width { get; set; }

        [Parameter(Mandatory = true, Position = 2, HelpMessage = "Viewport height in pixels")]
        public int Height { get; set; }

        [Parameter(HelpMessage = "Device scale factor (default: 1)")]
        public double DeviceScaleFactor { get; set; } = 1;

        [Parameter(HelpMessage = "Emulate mobile device")]
        public SwitchParameter IsMobile { get; set; }

        [Parameter(HelpMessage = "Enable touch events")]
        public SwitchParameter HasTouch { get; set; }

        [Parameter(HelpMessage = "Use landscape orientation")]
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
