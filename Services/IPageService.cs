using System;
using System.Collections.Generic;
using PowerBrowser.Transport;

namespace PowerBrowser.Services
{
    public interface IPageService
    {
        PBrowserPage CreatePage(PBrowser pBrowser, string name, int width, int height, string url, bool waitForLoad);

        List<PBrowserPage> GetPages();

        List<PBrowserPage> GetPagesByBrowser(PBrowser pBrowser);

        void RemovePage(PBrowserPage browserPage);

        PBrowserElement FindElementBySelector(PBrowserPage browserPage, string selector, bool waitForLoad, int timeout);
        void NavigatePage(PBrowserPage browserPage, string url, bool waitForLoad);
    }

}