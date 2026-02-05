using System.IO;
using System.Management.Automation;
using System.Text.Json;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsData.Export, "PupPageSession")]
    [OutputType(typeof(PupSession))]
    public class ExportPageSessionCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PupPage Page { get; set; }

        [Parameter(Position = 1)]
        public string FilePath { get; set; }

        [Parameter()]
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
