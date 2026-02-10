using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsCommon.Get, "PupRecording")]
    [OutputType(typeof(PupRecordingEvent[]))]
    public class GetRecordingCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "Page to get recorded events from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Include wait events based on actual timing between actions")]
        public SwitchParameter IncludeWaits { get; set; }

        [Parameter(HelpMessage = "Minimum wait to include (waits shorter than this are skipped, default 100ms)")]
        [ValidateRange(0, 60000)]
        public int WaitThreshold { get; set; } = 100;

        protected override void ProcessRecord()
        {
            try
            {
                List<PupRecordingEvent> events;
                lock (Page.RecordingLock)
                {
                    events = new List<PupRecordingEvent>(Page.RecordingEvents);
                }

                if (IncludeWaits.IsPresent)
                {
                    events = InsertWaitEvents(events, WaitThreshold);
                }

                WriteObject(events.ToArray(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetRecordingFailed", ErrorCategory.ReadError, Page));
            }
        }

        private List<PupRecordingEvent> InsertWaitEvents(List<PupRecordingEvent> events, int threshold)
        {
            if (events.Count == 0) return events;

            var result = new List<PupRecordingEvent>();
            PupRecordingEvent previous = null;

            foreach (var evt in events)
            {
                if (previous != null)
                {
                    var elapsed = (int)(evt.Timestamp - previous.Timestamp).TotalMilliseconds;
                    if (elapsed >= threshold)
                    {
                        result.Add(new PupRecordingEvent
                        {
                            Type = "wait",
                            Timestamp = previous.Timestamp,
                            Value = elapsed.ToString()
                        });
                    }
                }
                result.Add(evt);
                previous = evt;
            }

            return result;
        }
    }
}
