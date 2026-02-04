using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Http
{
    [Cmdlet(VerbsCommon.Set, "PupHttpHeader")]
    [OutputType(typeof(void))]
    public class SetHttpHeaderCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to set HTTP request headers for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "SingleHeader",
            HelpMessage = "HTTP header name")]
        public string Name { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            ParameterSetName = "SingleHeader",
            HelpMessage = "HTTP header value")]
        public string Value { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "MultipleHeaders",
            HelpMessage = "Hashtable of header name/value pairs")]
        public Hashtable Headers { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Clear",
            HelpMessage = "Clear all extra headers")]
        public SwitchParameter Clear { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var headers = new Dictionary<string, string>();

                if (Clear.IsPresent)
                {
                    // Set empty headers to clear
                    pageService.SetExtraHeadersAsync(headers).GetAwaiter().GetResult();
                    WriteVerbose("Cleared all extra HTTP request headers");
                    return;
                }

                if (Headers != null)
                {
                    foreach (DictionaryEntry entry in Headers)
                    {
                        headers[entry.Key.ToString()] = entry.Value?.ToString() ?? "";
                    }
                }
                else
                {
                    headers[Name] = Value;
                }

                pageService.SetExtraHeadersAsync(headers).GetAwaiter().GetResult();

                if (Headers != null)
                {
                    WriteVerbose($"Set {headers.Count} extra HTTP request header(s)");
                }
                else
                {
                    WriteVerbose($"Set HTTP request header: {Name}");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetHttpHeaderError", ErrorCategory.InvalidOperation, Page));
            }
        }
    }
}
