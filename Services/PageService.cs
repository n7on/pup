using System;
using PuppeteerSharp;
using PowerBrowser.Transport;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;

namespace PowerBrowser.Services
{

    public class PageService : IPageService
    {
        private const string RunningPagesKey = "RunningPages";

        private readonly SessionStateService<PBrowserPage> _sessionStateService;

        public PageService(SessionState sessionState)
        {
            _sessionStateService = new SessionStateService<PBrowserPage>(sessionState, RunningPagesKey);
        }

        public List<PBrowserPage> GetPagesByBrowser(PBrowser pBrowser)
        {
            return _sessionStateService.GetAll().Where(s => s.Browser == pBrowser).ToList();
        }
        public List<PBrowserPage> GetPages()
        {
            return _sessionStateService.GetAll();
        }

        public PBrowserPage CreatePage(PBrowser pBrowser, string name, int width, int height, string url, bool waitForLoad)
        {
            var pages = pBrowser.Browser.PagesAsync().GetAwaiter().GetResult();
            string pageName = string.IsNullOrEmpty(name) ? $"Page{pages.Length + 1}" : name;

            var page = pBrowser.Browser.NewPageAsync().GetAwaiter().GetResult();

            // Set viewport size
            page.SetViewportAsync(new ViewPortOptions
            {
                Width = width,
                Height = height
            }).GetAwaiter().GetResult();

            // Navigate to URL if specified
            if (!string.IsNullOrEmpty(url) && url != "about:blank")
            {
                if (waitForLoad)
                {
                    page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    page.GoToAsync(url).GetAwaiter().GetResult();
                }
            }

            var browserPage = new PBrowserPage(
                pBrowser,
                page,
                pageName,
                width,
                height
            );

            _sessionStateService.Save(browserPage.PageId, browserPage);
            return browserPage;
        }
        public void RemovePage(PBrowserPage browserPage)
        {
            browserPage.Page.CloseAsync().GetAwaiter().GetResult();
            _sessionStateService.Remove(browserPage.PageId);
        }

        public void NavigatePage(PBrowserPage browserPage, string url, bool waitForLoad)
        {
            if (waitForLoad)
            {
                browserPage.Page.GoToAsync(url, new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).GetAwaiter().GetResult();
            }
            else
            {
                browserPage.Page.GoToAsync(url).GetAwaiter().GetResult();
            }
        }

        public PBrowserElement FindElementBySelector(PBrowserPage browserPage, string selector, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle element;
            if (waitForLoad)
            {
                element = browserPage.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).GetAwaiter().GetResult();
            }
            else
            {
                var cookies = browserPage.Page.GetCookiesAsync().GetAwaiter().GetResult(); // Dummy call to ensure page is responsive
                element = browserPage.Page.QuerySelectorAsync(selector).GetAwaiter().GetResult();
            }
            if (element == null)
            {
                return null;
            }
            return new PBrowserElement(element, browserPage.Page, Guid.NewGuid().ToString(), browserPage.PageName, selector, 0);
        }   


    }
}