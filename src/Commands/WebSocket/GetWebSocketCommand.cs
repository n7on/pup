using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.WebSocket
{
    [Cmdlet(VerbsCommon.Get, "PupWebSocket")]
    [OutputType(typeof(PupWebSocketEntry))]
    public class GetWebSocketCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get WebSocket connections from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Filter by URL pattern (substring match)")]
        public string Url { get; set; }

        [Parameter(HelpMessage = "Only show active (open) connections")]
        public SwitchParameter Active { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var entries = SnapshotWebSocketEntries();

                // Apply URL filter
                if (!string.IsNullOrEmpty(Url))
                {
                    entries = entries.Where(e => e.Url != null && e.Url.IndexOf(Url, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }

                // Apply active filter
                if (Active.IsPresent)
                {
                    entries = entries.Where(e => e.State == "open" || e.State == "connecting").ToList();
                }

                WriteObject(entries.ToArray(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetWebSocketError", ErrorCategory.ReadError, Page));
            }
        }

        private List<PupWebSocketEntry> SnapshotWebSocketEntries()
        {
            var list = new List<PupWebSocketEntry>();
            lock (Page.WebSocketLock)
            {
                foreach (var entry in Page.WebSocketEntries)
                {
                    var copy = new PupWebSocketEntry
                    {
                        RequestId = entry.RequestId,
                        Url = entry.Url,
                        State = entry.State,
                        CreatedTime = entry.CreatedTime,
                        ClosedTime = entry.ClosedTime,
                        Frames = new List<PupWebSocketFrame>(entry.Frames.Select(f => new PupWebSocketFrame
                        {
                            Direction = f.Direction,
                            Timestamp = f.Timestamp,
                            Opcode = f.Opcode,
                            PayloadData = f.PayloadData,
                            PayloadLength = f.PayloadLength
                        }))
                    };
                    list.Add(copy);
                }
            }
            return list;
        }
    }
}
