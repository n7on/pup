using System;
using Pup.Common;

namespace Pup.Transport
{
    public class PupCookie
    {
        public PupCookie(string name, string url, string value, string domain, string path, DateTime? expires, bool? httpOnly, bool? secure, PupSameSite? sameSite)
        {
            Name = name;
            Url = url;
            Value = value;
            Domain = domain;
            Path = path;
            Expires = expires;
            HttpOnly = httpOnly;
            Secure = secure;
            SameSite = sameSite;
        }   
        public string Name { get; set; }
        public string Url { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public string Path { get; set; }
        public DateTime? Expires { get; set; }
        public bool? HttpOnly { get; set; }

        public bool? Secure { get; set; }
        public PupSameSite? SameSite { get; set; }
    }
}   