namespace Pup.Transport
{
    /// <summary>
    /// A selector pattern that can match similar elements
    /// </summary>
    public class PupElementPattern
    {
        public string Type { get; set; }
        public string Selector { get; set; }
        public int MatchCount { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Selector} ({MatchCount} matches)";
        }
    }
}
