using System.IO;
using System.Management.Automation;
using System.Text.Json;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Session
{
    [Cmdlet(VerbsData.Export, "PupSession")]
    [OutputType(typeof(PupSession))]
    public class ExportSessionCommand : PupBaseCommand
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The page to export session from")]
        public PupPage Page { get; set; }

        [Parameter(Position = 1, HelpMessage = "File path to save the session JSON")]
        public string FilePath { get; set; }

        [Parameter(HelpMessage = "Output the session object even when saving to file")]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            var service = ServiceFactory.CreatePageService(Page);
            var session = service.ExportSessionAsync().GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(FilePath))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(session, options);
                File.WriteAllText(FilePath, json);

                if (PassThru.IsPresent)
                {
                    WriteObject(session);
                }
            }
            else
            {
                WriteObject(session);
            }
        }
    }
}
