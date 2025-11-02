using System;
using System.Management.Automation;
using PuppeteerSharp;

namespace PowerBrowser.Transport
{
    /// <summary>
    /// PowerShell-friendly wrapper for IPage with additional metadata
    /// </summary>
    public class PBPage
    {
        public string PageId => $"{Browser.BrowserType}_{PageName}";
        public string PageName { get; set; }
        public PBBrowser Browser { get; set; }
        [Hidden]
        public IPage Page { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }


        public PBPage(PBBrowser browser, IPage page, string pageName, int width, int height)
        {
            PageName = pageName;
            Browser = browser;
            Page = page;
            CreatedTime = DateTime.Now;
            ViewportWidth = width;
            ViewportHeight = height;
        }

        // Properties for PowerShell display
        public string ViewportSize => $"{ViewportWidth}x{ViewportHeight}";
        public bool IsClosed => Page?.IsClosed ?? true;

        public string Url
        {
            get
            {
                try
                {
                    return Page?.Url ?? "about:blank";
                }
                catch
                {
                    return "about:blank";
                }
            }
        }

        public string Content
        {
            get
            {
                try
                {
                    return Page?.GetContentAsync().GetAwaiter().GetResult() ?? "";
                }
                catch
                {
                    return "";
                }
            }
        }

        public string Title
        {
            get
            {
                try
                {
                    return Page?.GetTitleAsync().GetAwaiter().GetResult() ?? "";
                }
                catch
                {
                    return "Unknown";
                }
            }
        }

        public override string ToString()
        {
            return $"{PageName} ({Url}) - {ViewportSize}";
        }
    }
}