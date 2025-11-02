using System;
using System.Collections.Generic;
using PowerBrowser.Transport;

namespace PowerBrowser.Services
{
    public interface IPageService
    {
        PBPage CreatePage(PBBrowser pBrowser, string name, int width, int height, string url, bool waitForLoad);

        List<PBPage> GetPages();

        List<PBPage> GetPagesByBrowser(PBBrowser pBrowser);

        void RemovePage(PBPage browserPage);

        PBElement FindElementBySelector(PBPage browserPage, string selector, bool waitForLoad, int timeout);
        void NavigatePage(PBPage browserPage, string url, bool waitForLoad);
    }
}