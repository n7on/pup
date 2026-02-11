using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Http
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupHttpFetch")]
    [OutputType(typeof(PupFetchResponse))]
    public class InvokeHttpFetchCommand : PupBaseCommand
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The page context to make the request from")]
        public PupPage Page { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "The URL to fetch")]
        public string Url { get; set; }

        [Parameter(HelpMessage = "HTTP method (GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS)")]
        [ValidateSet("GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS")]
        public string Method { get; set; } = "GET";

        [Parameter(HelpMessage = "Request body (string or object to be serialized as JSON)")]
        public object Body { get; set; }

        [Parameter(HelpMessage = "Additional HTTP headers as a hashtable")]
        public Hashtable Headers { get; set; }

        [Parameter(HelpMessage = "Content-Type header value")]
        public string ContentType { get; set; }

        [Parameter(HelpMessage = "Parse response body as JSON")]
        public SwitchParameter AsJson { get; set; }

        [Parameter(HelpMessage = "Request timeout in milliseconds (default: 30000)")]
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
