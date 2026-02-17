using System;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupRecording")]
    [OutputType(typeof(void))]
    public class InvokeRecordingCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Page to replay recording on")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Recording events to replay (from Get-PupRecording)")]
        public PupRecordingEvent[] Recording { get; set; }

        [Parameter(HelpMessage = "Fixed delay between actions in milliseconds (default 0)")]
        [ValidateRange(0, 60000)]
        public int Delay { get; set; } = 0;

        protected override void ProcessRecord()
        {
            try
            {
                var recordingService = ServiceFactory.CreateRecordingService(Page);
                recordingService.ReplayEventsAsync(Recording, Delay).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokeRecordingFailed", ErrorCategory.OperationStopped, Page));
            }
        }
    }
}
