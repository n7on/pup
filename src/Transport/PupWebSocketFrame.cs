using System;

namespace Pup.Transport
{
    public class PupWebSocketFrame
    {
        public string Direction { get; set; }  // "sent" or "received"
        public DateTime Timestamp { get; set; }
        public int Opcode { get; set; }  // 1=text, 2=binary
        public string PayloadData { get; set; }
        public long PayloadLength { get; set; }

        public override string ToString()
        {
            var typeStr = Opcode == 1 ? "text" : Opcode == 2 ? "binary" : $"opcode:{Opcode}";
            var preview = PayloadData?.Length > 50 ? PayloadData.Substring(0, 50) + "..." : PayloadData;
            return $"[{Direction}] {typeStr} ({PayloadLength} bytes): {preview}";
        }
    }
}
