using System.Collections.Generic;
using System.Threading.Tasks;
using PowerBrowser.Transport;

namespace PowerBrowser.Services
{
    public interface IPageService
    {
        Task<PBPage> CreatePageAsync(PBBrowser pBrowser, string name, int width, int height, string url, bool waitForLoad);

        List<PBPage> GetPages();

        List<PBPage> GetPagesByBrowser(PBBrowser pBrowser);

        Task RemovePageAsync(PBPage browserPage);

        Task NavigatePageAsync(PBPage browserPage, string url, bool waitForLoad);

        Task<byte[]> GetPageScreenshotAsync(PBPage browserPage, string filePath = null, bool fullPage = false);

        Task<T> ExecuteScriptAsync<T>(PBPage browserPage, string script, params object[] args);
        Task ExecuteScriptAsync(PBPage browserPage, string script, params object[] args);

        Task<List<PBCookie>> GetCookiesAsync(PBPage browserPage);
        Task DeleteCookiesAsync(PBPage browserPage, PBCookie[] cookies);
        Task SetCookiesAsync(PBPage browserPage, PBCookie[] cookies);
    }
}