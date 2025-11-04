using System.Collections.Generic;
using System.Threading.Tasks;
using PowerBrowser.Transport;

namespace PowerBrowser.Services
{
    public interface IElementService
    {
        Task<PBElement> FindElementBySelectorAsync(PBPage page, string selector, bool waitForLoad, int timeout);
        Task<List<PBElement>> FindElementsBySelectorAsync(PBPage page, string selector, bool waitForLoad, int timeout);
        
        Task ClickElementAsync(PBElement element);
        Task ClickElementBySelectorAsync(PBPage page, string selector);
        Task ClickElementByCoordinatesAsync(PBPage page, double x, double y);
        Task SetElementTextAsync(PBElement element, string text);
        Task SetElementValueAsync(PBElement element, string value);
        
        Task<string> GetElementAttributeAsync(PBElement element, string attributeName);
        Task<string> GetElementTextAsync(PBElement element);
        Task<string> GetElementValueAsync(PBElement element);
        
        Task HoverElementAsync(PBElement element);
        Task HoverElementBySelectorAsync(PBPage page, string selector);
        
        Task FocusElementAsync(PBElement element);
        Task FocusElementBySelectorAsync(PBPage page, string selector);
        
        Task<bool> IsElementVisibleAsync(PBElement element);
        Task<bool> IsElementEnabledAsync(PBElement element);
        
        Task WaitForElementAsync(PBPage page, string selector, int timeout);
        Task WaitForElementToBeVisibleAsync(PBPage page, string selector, int timeout);
        Task WaitForElementToBeHiddenAsync(PBPage page, string selector, int timeout);
    }
}