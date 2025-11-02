using System;
using PuppeteerSharp;
using PowerBrowser.Transport;
using PowerBrowser.Common;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;

namespace PowerBrowser.Services
{

    public class PageService : IPageService
    {
        private const string RunningPagesKey = "RunningPages";

        private readonly SessionStateService<PBPage> _sessionStateService;

        public PageService(SessionState sessionState)
        {
            _sessionStateService = new SessionStateService<PBPage>(sessionState, RunningPagesKey);
        }
    
        public List<PBPage> GetPagesByBrowser(PBBrowser pBrowser)
        {
            return _sessionStateService.GetAll().Where(s => s.Browser == pBrowser).ToList();
        }
        public List<PBPage> GetPages()
        {
            return _sessionStateService.GetAll();
        }

        public PBPage CreatePage(PBBrowser pBrowser, string name, int width, int height, string url, bool waitForLoad)
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

            var browserPage = new PBPage(
                pBrowser,
                page,
                pageName,
                width,
                height
            );

            _sessionStateService.Save(browserPage.PageId, browserPage);
            return browserPage;
        }
        public void RemovePage(PBPage browserPage)
        {
            browserPage.Page.CloseAsync().GetAwaiter().GetResult();
            _sessionStateService.Remove(browserPage.PageId);
        }

        public void NavigatePage(PBPage browserPage, string url, bool waitForLoad)
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

        public PBElement FindElementBySelector(PBPage browserPage, string selector, bool waitForLoad = false, int timeout = 30000)
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
            return new PBElement(element, browserPage.Page, Guid.NewGuid().ToString(), browserPage.PageName, selector, 0);
        }

        public List<PBCookie> GetCookies(PBPage browserPage)
        {
            var puppeteerCookies = browserPage.Page.GetCookiesAsync().GetAwaiter().GetResult();
            var cookies = new List<PBCookie>();
            foreach (var c in puppeteerCookies)
            {
                cookies.Add(new PBCookie
                {
                    Name = c.Name,
                    Value = c.Value,
                    Domain = c.Domain,
                    Path = c.Path,
                    Expires = DateTimeOffset.FromUnixTimeSeconds((long)c.Expires).DateTime,
                    HttpOnly = c.HttpOnly,
                    Secure = c.Secure,
                    SameSite = c.SameSite.ToSupportedPBSameSite()
                });
            }
            return cookies;
        }


        public void DeleteCookies(PBPage browserPage, PBCookie[] cookies)
        {
            var puppeteerCookies = new List<CookieParam>();
            foreach (var c in cookies)
            {
                puppeteerCookies.Add(new CookieParam
                {
                    Name = c.Name,
                    Domain = c.Domain,
                    Path = c.Path,
                    Url = c.Url
                });
            }
            browserPage.Page.DeleteCookieAsync(puppeteerCookies.ToArray()).GetAwaiter().GetResult();
        }


        public void SetCookies(PBPage browserPage, PBCookie[] cookies)
        {
            var puppeteerCookies = new List<CookieParam>();
            foreach (var c in cookies)
            {
                double? expires = null;
                if (c.Expires.HasValue)
                {
                    expires = ((DateTimeOffset)c.Expires.Value).ToUnixTimeSeconds();
                }
                puppeteerCookies.Add(new CookieParam
                {
                    Name = c.Name,
                    Value = c.Value,
                    Domain = c.Domain,
                    Path = c.Path,
                    Expires = expires,
                    HttpOnly = c.HttpOnly,
                    Secure = c.Secure,
                    SameSite = c.SameSite.ToPuppeteerSameSiteMode()
                });
            }
            browserPage.Page.SetCookieAsync(puppeteerCookies.ToArray()).GetAwaiter().GetResult();
        }   

    }
}