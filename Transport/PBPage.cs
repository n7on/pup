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
        private string _url;
        private string _content;

        private string _title;

        public PBPage(
            PBBrowser browser,
            IPage page,
            string pageName,
            int width,
            int height,
            string url,
            string content,
            string title
        )
        {
            PageName = pageName;
            Browser = browser;
            Page = page;
            CreatedTime = DateTime.Now;
            ViewportWidth = width;
            ViewportHeight = height;
            _url = url;
            _content = content;
            _title = title; 
        }

        // Properties for PowerShell display
        public string ViewportSize => $"{ViewportWidth}x{ViewportHeight}";
        public bool IsClosed => Page?.IsClosed ?? true;

        public string Url
        {
            get{ return _url; }
        }

        public string Content
        {
            get{return _content; }
        }

        public string Title
        {
            get{return _title; }
        }

        public override string ToString()
        {
            return $"{PageName} ({Url}) - {ViewportSize}";
        }
    }
}