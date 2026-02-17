using System;
using System.Collections.Generic;
using System.Management.Automation;
using PuppeteerSharp;
using Pup.Common;

namespace Pup.Transport
{
    /// <summary>
    /// Stores handler configuration for a page event
    /// </summary>
    internal class PageEventHandler
    {
        public PupPageEvent Event { get; set; }
        public ScriptBlock ScriptBlock { get; set; }
        public SessionState SessionState { get; set; }
        public PupHandlerAction? Action { get; set; }
        public object NativeHandler { get; set; }
    }

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

        // Event handlers storage
        [Hidden]
        internal object HandlersLock { get; } = new object();
        [Hidden]
        internal Dictionary<PupPageEvent, PageEventHandler> EventHandlers { get; } = new Dictionary<PupPageEvent, PageEventHandler>();

        // Legacy dialog handler for PageService compatibility
        [Hidden]
        internal EventHandler<DialogEventArgs> DialogHandler { get; set; }

        [Hidden]
        internal bool CaptureInitialized { get; set; }
        [Hidden]
        internal object ConsoleLock { get; } = new object();
        [Hidden]
        internal object NetworkLock { get; } = new object();
        [Hidden]
        internal List<PupConsoleEntry> ConsoleEntries { get; } = new List<PupConsoleEntry>();
        [Hidden]
        internal List<PupNetworkEntry> NetworkEntries { get; } = new List<PupNetworkEntry>();
        [Hidden]
        internal Dictionary<string, PupNetworkEntry> NetworkMap { get; } = new Dictionary<string, PupNetworkEntry>(StringComparer.OrdinalIgnoreCase);
        [Hidden]
        internal ICDPSession NetworkSession { get; set; }

        [Hidden]
        internal object WebSocketLock { get; } = new object();
        [Hidden]
        internal List<PupWebSocketEntry> WebSocketEntries { get; } = new List<PupWebSocketEntry>();
        [Hidden]
        internal Dictionary<string, PupWebSocketEntry> WebSocketMap { get; } = new Dictionary<string, PupWebSocketEntry>(StringComparer.OrdinalIgnoreCase);

        [Hidden]
        internal object RecordingLock { get; } = new object();
        [Hidden]
        internal bool RecordingActive { get; set; }
        [Hidden]
        internal string DownloadPath { get; set; }
        [Hidden]
        internal List<PupRecordingEvent> RecordingEvents { get; } = new List<PupRecordingEvent>();

        public PupPage(IPage page, string title)
        {
            Page = page;
            Title = title;
            Url = page.Url;
        }
    }
}
