using System;
using PuppeteerSharp;
using System.Management.Automation;

namespace Pup.Transport
{
    /// <summary>
    /// PowerShell-friendly wrapper for IElementHandle with additional metadata
    /// </summary>
    public class PupElement
    {
        public string ElementId { get; set; }
        [Hidden]
        public IElementHandle Element { get; }
        [Hidden]
        public IPage Page { get; }
        public string Selector { get; set; }
        public int Index { get; set; }
        public DateTime FoundTime { get; set; }

        private readonly string _tagName;
        private readonly string _innerText;
        private readonly string _innerHTML;
        private readonly string _id;
        private readonly bool? _isVisible;
        public PupElement(
            IElementHandle element,
            IPage page,
            string elementId,
            string selector,
            int index,
            string tagName,
            string innerText,
            string innerHTML,
            string id,
            bool? isVisible
        )
        {
            Element = element;
            Page = page;
            ElementId = elementId;
            Selector = selector;
            Index = index;
            FoundTime = DateTime.Now;
            _tagName = tagName;
            _innerText = innerText;
            _innerHTML = innerHTML;
            _id = id;
            _isVisible = isVisible;
        }

        // Properties for PowerShell display
        public string TagName
        {
            get
            {return _tagName;}
        }

        public string InnerText
        {
            get{return _innerText;}
        }

        public string InnerHTML
        {
            get{return _innerHTML;}
        }

        public string Id
        {
            get{ return _id;}
        }

        public bool IsVisible
        {
            get { return _isVisible ?? false; }
        }

        public override string ToString()
        {
            var text = InnerText;
            var displayText = string.IsNullOrEmpty(text) ? "" : $" '{text.Substring(0, Math.Min(30, text.Length))}'";
            if (text.Length > 30) displayText += "...";
            
            return $"{TagName}[{Index}]{displayText} ({Selector})";
        }
    }
}
