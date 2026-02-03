namespace Pup.Transport
{
    /// <summary>
    /// Represents an option in a select/dropdown element
    /// </summary>
    public class PupSelectOption
    {
        public string Value { get; }
        public string Text { get; }
        public int Index { get; }
        public bool Selected { get; }
        public bool Disabled { get; }

        public PupSelectOption(string value, string text, int index, bool selected, bool disabled)
        {
            Value = value;
            Text = text;
            Index = index;
            Selected = selected;
            Disabled = disabled;
        }

        public override string ToString()
        {
            var status = Selected ? " [Selected]" : "";
            status += Disabled ? " [Disabled]" : "";
            return $"[{Index}] {Text} (value: {Value}){status}";
        }
    }
}
