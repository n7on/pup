using System;
using System.IO;
using System.Management.Automation;
using System.Text.Json;
using Pup.Services;
using Pup.Transport;

namespace Pup.Commands.Recording
{
    [Cmdlet(VerbsData.Convert, "PupRecording")]
    [OutputType(typeof(string))]
    public class ConvertRecordingCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "FilePath")]
        public string InputFile { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Json")]
        public string Json { get; set; }

        [Parameter(Position = 1)]
        public string OutputFile { get; set; }

        [Parameter()]
        public string PageVariable { get; set; } = "$page";

        [Parameter()]
        public string BrowserVariable { get; set; } = "$browser";

        [Parameter()]
        public SwitchParameter IncludeSetup { get; set; }

        [Parameter()]
        public SwitchParameter IncludeTeardown { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string jsonContent = !string.IsNullOrEmpty(Json) ? Json : File.ReadAllText(InputFile);

                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var recording = JsonSerializer.Deserialize<PupRecording>(jsonContent, jsonOptions);

                var service = ServiceFactory.CreateRecordingService();
                var options = new RecordingConvertOptions
                {
                    PageVariable = PageVariable,
                    BrowserVariable = BrowserVariable,
                    IncludeSetup = IncludeSetup.IsPresent,
                    IncludeTeardown = IncludeTeardown.IsPresent
                };

                var script = service.ConvertToScript(recording, options);

                if (!string.IsNullOrEmpty(OutputFile))
                {
                    File.WriteAllText(OutputFile, script);
                    WriteObject(OutputFile);
                }
                else
                {
                    WriteObject(script);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ConvertRecordingFailed", ErrorCategory.InvalidData, null));
            }
        }
    }
}
