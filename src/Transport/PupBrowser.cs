using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;
using System.Management.Automation;
using Pup.Common;

namespace Pup.Transport
{
    /// <summary>
    /// Stores handler configuration for a browser event
    /// </summary>
    internal class BrowserEventHandler
    {
        public PupBrowserEvent Event { get; set; }
        public ScriptBlock ScriptBlock { get; set; }
        public SessionState SessionState { get; set; }
        public PupHandlerAction? Action { get; set; }
        public object NativeHandler { get; set; }
    }

    /// <summary>
    /// PowerShell-friendly wrapper for IBrowser with additional metadata
    /// </summary>
    public class PupBrowser
    {

        [Hidden]
        public IBrowser Browser { get; set; }
        public PupSupportedBrowser BrowserType { get; set; }
        public DateTime StartTime { get; set; }
        public bool Headless { get; set; }
        public string WindowSize { get; set; }

        // Additional properties for Get-Browser display
        public string Size { get; set; }
        public string Path { get; set; }

        // Event handlers storage
        [Hidden]
        internal object HandlersLock { get; } = new object();
        [Hidden]
        internal Dictionary<PupBrowserEvent, BrowserEventHandler> EventHandlers { get; } = new Dictionary<PupBrowserEvent, BrowserEventHandler>();

        public PupBrowser(IBrowser browser, bool headless, string windowSize, string path)
        {
            Browser = browser;
            BrowserType = browser?.BrowserType.ToPBSupportedBrowser() ?? PupSupportedBrowser.Chrome;
            StartTime = DateTime.Now;
            Headless = headless;
            WindowSize = windowSize;
            Path = path;
        }

        public PupBrowser(PupSupportedBrowser browserType, string path)
        {
            Browser = null;
            BrowserType = browserType;
            StartTime = DateTime.MinValue;
            Headless = false;
            WindowSize = "Unknown"; // Default size
            Path = path;
            Size = "Unknown"; // Default size
        }

        // Properties for PowerShell display
        public int ProcessId => Browser?.Process?.Id ?? -1;
        public string WebSocketEndpoint => Browser?.WebSocketEndpoint ?? "Unknown";
        public bool Running => Browser?.IsConnected ?? false;  // User-friendly alias
        public int PageCount => Browser != null ? Task.Run(() => Browser.PagesAsync()).GetAwaiter().GetResult().Length : 0;

        public override string ToString()
        {
            return $"(PID: {ProcessId}, Pages: {PageCount}, Running: {Running})";
        }
    }
}
