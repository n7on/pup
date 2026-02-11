using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsData.ConvertTo, "PupScript")]
    [OutputType(typeof(string))]
    public class ConvertToScriptCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Recording events to convert")]
        public PupRecordingEvent[] RecordingEvents { get; set; }

        [Parameter(HelpMessage = "Variable name for page in generated script")]
        public string PageVariable { get; set; } = "$page";

        [Parameter(HelpMessage = "Variable name for browser in generated script")]
        public string BrowserVariable { get; set; } = "$browser";

        [Parameter(HelpMessage = "Include setup code in generated script")]
        public SwitchParameter IncludeSetup { get; set; }

        [Parameter(HelpMessage = "Include teardown code in generated script")]
        public SwitchParameter IncludeTeardown { get; set; }

        [Parameter(HelpMessage = "Override URL for setup code (by default uses URL from recording start)")]
        public string Url { get; set; }

        [Parameter(HelpMessage = "Save output to file")]
        public string OutputFile { get; set; }

        [Parameter(HelpMessage = "Minimum delay between actions in milliseconds")]
        [ValidateRange(0, 60000)]
        public int DelayMin { get; set; } = 0;

        [Parameter(HelpMessage = "Maximum delay between actions in milliseconds (randomizes between min and max)")]
        [ValidateRange(0, 60000)]
        public int DelayMax { get; set; } = 0;

        private List<PupRecordingEvent> _allEvents = new List<PupRecordingEvent>();

        protected override void ProcessRecord()
        {
            if (RecordingEvents != null)
            {
                _allEvents.AddRange(RecordingEvents);
            }
        }

        protected override void EndProcessing()
        {
            try
            {
                var recordingService = new RecordingService(null);
                var options = new RecordingConvertOptions
                {
                    PageVariable = PageVariable,
                    BrowserVariable = BrowserVariable,
                    IncludeSetup = IncludeSetup.IsPresent,
                    IncludeTeardown = IncludeTeardown.IsPresent,
                    StartUrl = Url,
                    DelayMin = DelayMin,
                    DelayMax = DelayMax
                };

                var script = recordingService.ConvertEventsToScript(_allEvents, options);

                if (!string.IsNullOrEmpty(OutputFile))
                {
                    System.IO.File.WriteAllText(OutputFile, script);
                    WriteObject(OutputFile);
                }
                else
                {
                    WriteObject(script);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ConvertToScriptFailed", ErrorCategory.InvalidOperation, null));
            }
        }
    }
}
