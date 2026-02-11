using System;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsLifecycle.Start, "PupRecording")]
    [OutputType(typeof(void))]
    public class StartRecordingCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Page to record interactions on")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Clear any existing recorded events before starting")]
        public SwitchParameter Clear { get; set; }

        [Parameter(HelpMessage = "Include scroll events (can be verbose)")]
        public SwitchParameter IncludeScroll { get; set; }

        [Parameter(HelpMessage = "Include hover events")]
        public SwitchParameter IncludeHover { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Page.RecordingActive)
                {
                    WriteWarning("Recording is already active on this page.");
                    return;
                }

                var options = new RecordingOptions
                {
                    IncludeScroll = IncludeScroll.IsPresent,
                    IncludeHover = IncludeHover.IsPresent
                };

                if (Clear.IsPresent)
                {
                    lock (Page.RecordingLock)
                    {
                        Page.RecordingEvents.Clear();
                    }
                }

                var recordingService = ServiceFactory.CreateRecordingService(Page);
                recordingService.StartRecordingAsync(options).GetAwaiter().GetResult();

                WriteVerbose("Recording started. Interact with the browser, then use Stop-PupRecording to capture events.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StartRecordingFailed", ErrorCategory.OperationStopped, Page));
            }
        }
    }
}
