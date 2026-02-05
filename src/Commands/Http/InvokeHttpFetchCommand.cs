using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;

namespace Pup.Commands.Http
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupHttpFetch")]
    [OutputType(typeof(PupFetchResponse))]
    public class InvokeHttpFetchCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public PupPage Page { get; set; }

        [Parameter(Mandatory = true, Position = 1)]
        public string Url { get; set; }

        [Parameter()]
        [ValidateSet("GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS")]
        public string Method { get; set; } = "GET";

        [Parameter()]
        public object Body { get; set; }

        [Parameter()]
        public Hashtable Headers { get; set; }

        [Parameter()]
        public string ContentType { get; set; }

        [Parameter()]
        public SwitchParameter AsJson { get; set; }

        [Parameter()]
        public int Timeout { get; set; } = 30000;

        protected override void ProcessRecord()
        {
            var service = ServiceFactory.CreatePageService(Page);

            // Convert Hashtable to Dictionary
            Dictionary<string, string> headerDict = null;
            if (Headers != null)
            {
                headerDict = new Dictionary<string, string>();
                foreach (DictionaryEntry entry in Headers)
                {
                    headerDict[entry.Key.ToString()] = entry.Value?.ToString() ?? "";
                }
            }

            var response = service.FetchAsync(Url, Method, Body, headerDict, ContentType, Timeout, AsJson.IsPresent)
                .GetAwaiter().GetResult();

            WriteObject(response);
        }
    }
}
