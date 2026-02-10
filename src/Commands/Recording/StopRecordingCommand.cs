using System;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsLifecycle.Stop, "PupRecording")]
    [OutputType(typeof(void))]
    public class StopRecordingCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Page to stop recording on")]
        public PupPage Page { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (!Page.RecordingActive)
                {
                    WriteWarning("Recording is not active on this page.");
                    return;
                }

                var recordingService = ServiceFactory.CreateRecordingService(Page);
                recordingService.StopRecordingAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StopRecordingFailed", ErrorCategory.OperationStopped, Page));
            }
        }
    }
}
