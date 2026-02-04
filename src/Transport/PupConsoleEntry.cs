using System;

namespace Pup.Transport
{
    public class PupConsoleEntry
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public int? LineNumber { get; set; }
        public int? ColumnNumber { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Text}";
        }
    }
}
