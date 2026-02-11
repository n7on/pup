using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsCommon.Clear, "PupRecording")]
    [OutputType(typeof(void))]
    public class ClearRecordingCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Page to clear recorded events from")]
        public PupPage Page { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                lock (Page.RecordingLock)
                {
                    Page.RecordingEvents.Clear();
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ClearRecordingFailed", ErrorCategory.WriteError, Page));
            }
        }
    }
}
