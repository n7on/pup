using System;
using PowerBrowser.Common;

namespace PowerBrowser.Transport
{
    public class PBCookie
    {

        public string Name { get; set; }
        public string Url { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime? Expires { get; set; }
        public bool? HttpOnly { get; set; }

        public bool? Secure { get; set; }
        public PBSameSite? SameSite { get; set; }
    }
}   