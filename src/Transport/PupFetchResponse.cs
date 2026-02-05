using System.Collections.Generic;

namespace Pup.Transport
{
    public class PupFetchResponse
    {
        public int Status { get; set; }
        public string StatusText { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Body { get; set; }
        public object JsonBody { get; set; }
        public bool Ok { get; set; }
        public string Url { get; set; }
    }
}
