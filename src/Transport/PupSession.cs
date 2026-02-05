using System;
using System.Collections.Generic;

namespace Pup.Transport
{
    public class PupSession
    {
        public string Url { get; set; }
        public DateTime ExportedAt { get; set; }
        public List<PupCookie> Cookies { get; set; } = new List<PupCookie>();
        public Dictionary<string, string> LocalStorage { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SessionStorage { get; set; } = new Dictionary<string, string>();
    }
}
