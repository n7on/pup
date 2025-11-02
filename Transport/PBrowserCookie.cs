using PuppeteerSharp;
using System;

namespace PowerBrowser.Transport
{
    public class PBrowserCookie
    {

        public CookieParam Cookie { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime? Expires { get; set; }
        public bool HttpOnly { get; set; }
    }
}   