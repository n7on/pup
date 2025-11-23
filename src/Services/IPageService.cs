using System.Collections.Generic;
using System.Threading.Tasks;
using Pup.Transport;

namespace Pup.Services
{
    public interface IPageService
    {
        Task RemovePageAsync();

        Task<PupElement> FindElementBySelectorAsync(string selector, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsBySelectorAsync(string selector, bool waitForLoad, int timeout);
        Task<PupElement> FindElementByXPathAsync(string xpath, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsByXPathAsync(string xpath, bool waitForLoad, int timeout);
        Task ClickElementBySelectorAsync(string selector);
        Task ClickElementByCoordinatesAsync(double x, double y);
        Task FocusElementBySelectorAsync(string selector);
        
        Task HoverElementBySelectorAsync(string selector);
        
        Task WaitForElementAsync(string selector, int timeout);
        Task WaitForElementToBeVisibleAsync(string selector, int timeout);
        Task WaitForElementToBeHiddenAsync(string selector, int timeout);
        Task<PupPage> NavigatePageAsync(string url, bool waitForLoad);

        Task<byte[]> GetPageScreenshotAsync(string filePath = null, bool fullPage = false);

        Task<T> ExecuteScriptAsync<T>(string script, params object[] args);
        Task ExecuteScriptAsync(string script, params object[] args);

        Task<List<PupCookie>> GetCookiesAsync();
        Task DeleteCookiesAsync(PupCookie[] cookies);
        Task SetCookiesAsync(PupCookie[] cookies);

        Task<PupPage> NavigateBackAsync(bool waitForLoad);
        Task<PupPage> NavigateForwardAsync(bool waitForLoad);
        Task<PupPage> ReloadPageAsync(bool waitForLoad);
    }
}