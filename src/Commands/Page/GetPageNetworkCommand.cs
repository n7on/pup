using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.Json;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPageNetwork")]
    [OutputType(typeof(PupNetworkEntry))]
    public class GetPageNetworkCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get network requests from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Include response bodies when possible (text only)")]
        public SwitchParameter IncludeContent { get; set; }

        [Parameter(HelpMessage = "Maximum bytes to store per body when including content (default 256KB)")]
        public int MaxContentBytes { get; set; } = 262144;

        [Parameter(HelpMessage = "Return captured entries to the pipeline (default: true)")]
        public SwitchParameter PassThru { get; set; } = true;

        protected override void ProcessRecord()
        {
            try
            {
                var entries = SnapshotNetworkEntries();

                if (IncludeContent.IsPresent)
                {
                    FetchBodies(entries);
                }

                if (PassThru.IsPresent)
                {
                    WriteObject(entries.ToArray(), true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageNetworkError", ErrorCategory.ReadError, Page));
            }
        }

        private List<PupNetworkEntry> SnapshotNetworkEntries()
        {
            var list = new List<PupNetworkEntry>();
            lock (Page.NetworkLock)
            {
                foreach (var entry in Page.NetworkEntries)
                {
                    list.Add(new PupNetworkEntry
                    {
                        RequestId = entry.RequestId,
                        Url = entry.Url,
                        Method = entry.Method,
                        ResourceType = entry.ResourceType,
                        Status = entry.Status,
                        StatusText = entry.StatusText,
                        MimeType = entry.MimeType,
                        FromDiskCache = entry.FromDiskCache,
                        FromServiceWorker = entry.FromServiceWorker,
                        FromMemoryCache = entry.FromMemoryCache,
                        EncodedDataLength = entry.EncodedDataLength,
                        ErrorText = entry.ErrorText,
                        StartTime = entry.StartTime,
                        EndTime = entry.EndTime,
                        RequestHeaders = new Dictionary<string, string>(entry.RequestHeaders, StringComparer.OrdinalIgnoreCase),
                        ResponseHeaders = new Dictionary<string, string>(entry.ResponseHeaders, StringComparer.OrdinalIgnoreCase),
                        RemoteAddress = entry.RemoteAddress,
                        Body = entry.Body,
                        BodyBase64Encoded = entry.BodyBase64Encoded,
                        SecurityDetails = entry.SecurityDetails
                    });
                }
            }
            return list;
        }

        private void FetchBodies(List<PupNetworkEntry> entries)
        {
            if (Page.NetworkSession == null)
            {
                WriteVerbose("Network session not available; skipping body retrieval.");
                return;
            }

            foreach (var entry in entries.Where(e => e.Body == null && (e.EncodedDataLength ?? 0) <= MaxContentBytes))
            {
                try
                {
                    var bodyResult = Page.NetworkSession.SendAsync<JsonElement>("Network.getResponseBody", new Dictionary<string, object> { { "requestId", entry.RequestId } }).GetAwaiter().GetResult();
                    if (!bodyResult.ValueKind.Equals(JsonValueKind.Undefined))
                    {
                        entry.Body = bodyResult.TryGetProperty("body", out var bodyProp) ? bodyProp.GetString() : null;
                        entry.BodyBase64Encoded = bodyResult.TryGetProperty("base64Encoded", out var b64) && b64.GetBoolean();
                    }
                }
                catch
                {
                    // ignore body fetch errors
                }
            }
        }
    }
}
