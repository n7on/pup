using System;

namespace Pup.Transport
{
    public class PupRecordingEvent
    {
        public string Type { get; set; }           // click, change, input, keydown, scroll, navigate
        public DateTime Timestamp { get; set; }
        public string Selector { get; set; }       // CSS selector
        public string Value { get; set; }          // For input/change events
        public string Key { get; set; }            // For keydown events
        public string[] Modifiers { get; set; }    // Ctrl, Shift, Alt, Meta
        public string Url { get; set; }            // For navigate events
        public string TagName { get; set; }
        public string InputType { get; set; }      // text, checkbox, select, etc.
        public int? ClickCount { get; set; }       // 1=single, 2=double
        public double? ScrollX { get; set; }       // For scroll events
        public double? ScrollY { get; set; }       // For scroll events

        public override string ToString()
        {
            return $"{Type}: {Selector ?? Url ?? Key ?? "unknown"}";
        }
    }
}
