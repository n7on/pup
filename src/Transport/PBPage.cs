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
        public string ViewportSize => $"{ViewportWidth}x{ViewportHeight}";
        public bool Running => Page?.IsClosed ?? true;
        public string PageName { get;}
        public PBBrowser Browser { get; }
        [Hidden]
        public IPage Page { get; }
        public DateTime CreatedTime { get; }
        public int ViewportWidth { get; }
        public int ViewportHeight { get; }
        public string Url { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }

        public PBPage(
            PBBrowser browser,
            IPage page,
            string pageName,
            int width,
            int height
        )
        {
            PageName = pageName;
            Browser = browser;
            Page = page;
            CreatedTime = DateTime.Now;
            ViewportWidth = width;
            ViewportHeight = height;
        }

        public override string ToString()
        {
            return $"{PageName} ({Url}) - {ViewportSize}";
        }
    }
}