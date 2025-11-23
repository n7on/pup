using System.Collections.Generic;
using System.Threading.Tasks;
using Pup.Transport;

namespace Pup.Services
{
    public interface IElementService
    {
        Task ClickElementAsync();
        Task SetElementTextAsync(string text);
        Task SetElementValueAsync(string value);
        
        Task<string> GetElementAttributeAsync(string attributeName);
        Task<string> GetElementTextAsync();
        Task<string> GetElementValueAsync();
        
        Task HoverElementAsync();
        Task FocusElementAsync();
        Task<bool> IsElementVisibleAsync();
        Task<bool> IsElementEnabledAsync();
        
        Task<PupElement> FindElementBySelectorAsync(string selector);
        Task<List<PupElement>> FindElementsBySelectorAsync(string selector);
        Task<PupElement> FindElementByXPathAsync(string xpath);
        Task<List<PupElement>> FindElementsByXPathAsync(string xpath);
        
        Task<string> GetElementSelectorAsync(bool unique = false, bool shortest = false, bool fullPath = false);
        Task<string> GetSimilarElementsSelectorAsync(bool sameTag = false, bool sameClass = false);
        Task<int> CountElementsBySelectorAsync(string selector);
    }
}