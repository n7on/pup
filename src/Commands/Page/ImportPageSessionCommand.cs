using System.IO;
using System.Management.Automation;
using System.Text.Json;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsData.Import, "PupPageSession")]
    [OutputType(typeof(void))]
    public class ImportPageSessionCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PupPage Page { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "FilePath")]
        public string FilePath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Session")]
        public PupSession Session { get; set; }

        [Parameter()]
        public SwitchParameter NoCookies { get; set; }

        [Parameter()]
        public SwitchParameter NoLocalStorage { get; set; }

        [Parameter()]
        public SwitchParameter NoSessionStorage { get; set; }

        [Parameter()]
        public SwitchParameter Reload { get; set; }

        protected override void ProcessRecord()
        {
            PupSession session;

            if (Session != null)
            {
                session = Session;
            }
            else
            {
                var json = File.ReadAllText(FilePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                session = JsonSerializer.Deserialize<PupSession>(json, options);
            }

            var service = ServiceFactory.CreatePageService(Page);
            service.ImportSessionAsync(
                session,
                includeCookies: !NoCookies.IsPresent,
                includeLocalStorage: !NoLocalStorage.IsPresent,
                includeSessionStorage: !NoSessionStorage.IsPresent
            ).GetAwaiter().GetResult();

            if (Reload.IsPresent)
            {
                service.ReloadPageAsync(waitForLoad: true).GetAwaiter().GetResult();
            }
        }
    }
}
