using System;
using System.Collections.Generic;

namespace Pup.Transport
{
    public class PupNetworkEntry
    {
        public string RequestId { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string ResourceType { get; set; }
        public int? Status { get; set; }
        public string StatusText { get; set; }
        public string MimeType { get; set; }
        public bool? FromDiskCache { get; set; }
        public bool? FromServiceWorker { get; set; }
        public bool? FromMemoryCache { get; set; }
        public long? EncodedDataLength { get; set; }
        public string ErrorText { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Dictionary<string, string> RequestHeaders { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public string RemoteAddress { get; set; }

        // Body capture (optional, may be null/empty to save memory)
        public string Body { get; set; }
        public bool BodyBase64Encoded { get; set; }

        public override string ToString()
        {
            return $"{Method} {Url} {(Status.HasValue ? Status.ToString() : "")}";
        }
    }
}
