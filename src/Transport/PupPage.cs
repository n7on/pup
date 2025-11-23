using System;
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
        public string Url { get; }
        public string Title { get; }

        public PupPage(IPage page, string title)
        {
            Page = page;
            Title = title;
            Url = page.Url;
        }
    }
}