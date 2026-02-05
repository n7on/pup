using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Pup.Transport
{
    public class PupRecording
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("steps")]
        public List<PupRecordingStep> Steps { get; set; } = new List<PupRecordingStep>();
    }

    public class PupRecordingStep
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        // Navigation
        [JsonPropertyName("url")]
        public string Url { get; set; }

        // Selectors (array of selector arrays for fallbacks)
        [JsonPropertyName("selectors")]
        public List<List<string>> Selectors { get; set; }

        // Click/interaction offset
        [JsonPropertyName("offsetX")]
        public double? OffsetX { get; set; }

        [JsonPropertyName("offsetY")]
        public double? OffsetY { get; set; }

        // Input value
        [JsonPropertyName("value")]
        public string Value { get; set; }

        // Key events
        [JsonPropertyName("key")]
        public string Key { get; set; }

        // Scroll
        [JsonPropertyName("x")]
        public double? X { get; set; }

        [JsonPropertyName("y")]
        public double? Y { get; set; }

        // Viewport
        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("deviceScaleFactor")]
        public double? DeviceScaleFactor { get; set; }

        [JsonPropertyName("isMobile")]
        public bool? IsMobile { get; set; }

        [JsonPropertyName("hasTouch")]
        public bool? HasTouch { get; set; }

        [JsonPropertyName("isLandscape")]
        public bool? IsLandscape { get; set; }

        // Wait conditions
        [JsonPropertyName("visible")]
        public bool? Visible { get; set; }

        [JsonPropertyName("timeout")]
        public int? Timeout { get; set; }

        // Frame target
        [JsonPropertyName("target")]
        public string Target { get; set; }

        // Button for click
        [JsonPropertyName("button")]
        public string Button { get; set; }
    }
}
