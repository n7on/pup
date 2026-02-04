using System;
using System.Collections.Generic;

namespace Pup.Transport
{
    public class PupWebSocketEntry
    {
        public string RequestId { get; set; }
        public string Url { get; set; }
        public string State { get; set; }  // "connecting", "open", "closed"
        public DateTime? CreatedTime { get; set; }
        public DateTime? ClosedTime { get; set; }
        public List<PupWebSocketFrame> Frames { get; set; } = new List<PupWebSocketFrame>();

        public override string ToString()
        {
            return $"[{State}] {Url} ({Frames.Count} frames)";
        }
    }
}
