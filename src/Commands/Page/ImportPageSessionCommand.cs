using System.IO;
using System.Management.Automation;
using System.Text.Json;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsData.Import, "PupPageSession")]
    [OutputType(typeof(void))]
    public class ImportPageSessionCommand : PupBaseCommand
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The page to import session into")]
        public PupPage Page { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "FilePath", HelpMessage = "Path to session JSON file")]
        public string FilePath { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Session", HelpMessage = "Session object to import")]
        public PupSession Session { get; set; }

        [Parameter(HelpMessage = "Skip importing cookies")]
        public SwitchParameter NoCookies { get; set; }

        [Parameter(HelpMessage = "Skip importing local storage")]
        public SwitchParameter NoLocalStorage { get; set; }

        [Parameter(HelpMessage = "Skip importing session storage")]
        public SwitchParameter NoSessionStorage { get; set; }

        [Parameter(HelpMessage = "Reload the page after importing session")]
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
