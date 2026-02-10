using System;
using System.Collections.Generic;
using System.Management.Automation;
using PuppeteerSharp;

namespace Pup.Transport
{
    /// <summary>
    /// PowerShell-friendly wrapper for IPage with additional metadata
    /// </summary>
    public class PupPage
    {
        public bool Running => Page != null && !Page.IsClosed;
        [Hidden]
        public IPage Page { get; }
        public string Url { get; internal set; }
        public string Title { get; internal set; }

        [Hidden]
        internal EventHandler<DialogEventArgs> DialogHandler { get; set; }

        [Hidden]
        internal bool CaptureInitialized { get; set; }
        [Hidden]
        internal object ConsoleLock { get; } = new object();
        [Hidden]
        internal object NetworkLock { get; } = new object();
        [Hidden]
        internal System.Collections.Generic.List<PupConsoleEntry> ConsoleEntries { get; } = new System.Collections.Generic.List<PupConsoleEntry>();
        [Hidden]
        internal System.Collections.Generic.List<PupNetworkEntry> NetworkEntries { get; } = new System.Collections.Generic.List<PupNetworkEntry>();
        [Hidden]
        internal System.Collections.Generic.Dictionary<string, PupNetworkEntry> NetworkMap { get; } = new System.Collections.Generic.Dictionary<string, PupNetworkEntry>(System.StringComparer.OrdinalIgnoreCase);
        [Hidden]
        internal PuppeteerSharp.ICDPSession NetworkSession { get; set; }

        [Hidden]
        internal object WebSocketLock { get; } = new object();
        [Hidden]
        internal System.Collections.Generic.List<PupWebSocketEntry> WebSocketEntries { get; } = new System.Collections.Generic.List<PupWebSocketEntry>();
        [Hidden]
        internal System.Collections.Generic.Dictionary<string, PupWebSocketEntry> WebSocketMap { get; } = new System.Collections.Generic.Dictionary<string, PupWebSocketEntry>(System.StringComparer.OrdinalIgnoreCase);

        [Hidden]
        internal object RecordingLock { get; } = new object();
        [Hidden]
        internal bool RecordingActive { get; set; }
        [Hidden]
        internal System.Collections.Generic.List<PupRecordingEvent> RecordingEvents { get; } = new System.Collections.Generic.List<PupRecordingEvent>();

        public PupPage(IPage page, string title)
        {
            Page = page;
            Title = title;
            Url = page.Url;
        }
    }
}
