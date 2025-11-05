using System;
using PuppeteerSharp;
using PowerBrowser.Transport;
using PowerBrowser.Common;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public PBPage Save(PBPage page)
        {
            page.Url = page.Page.Url;
            page.Content = page.Page.GetContentAsync().GetAwaiter().GetResult();
            page.Title = page.Page.GetTitleAsync().GetAwaiter().GetResult();
            _sessionStateService.Save(page.PageId, page);
            return page;
        }
        public async Task<PBPage> CreatePageAsync(PBBrowser pBrowser, string name, int width, int height, string url, bool waitForLoad)
        {
            var pages = await pBrowser.Browser.PagesAsync().ConfigureAwait(false);
            string pageName = string.IsNullOrEmpty(name) ? $"Page{pages.Length + 1}" : name;

            var page = await pBrowser.Browser.NewPageAsync().ConfigureAwait(false);

            // Set viewport size
            await page.SetViewportAsync(new ViewPortOptions
            {
                Width = width,
                Height = height
            }).ConfigureAwait(false);

            // Navigate to URL if specified
            if (!string.IsNullOrEmpty(url) && url != "about:blank")
            {
                if (waitForLoad)
                {
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                    }).ConfigureAwait(false);
                }
                else
                {
                    await page.GoToAsync(url).ConfigureAwait(false);
                }
            }

            return Save(new PBPage(
                pBrowser,
                page,
                pageName,
                width,
                height
            ));
        }
        public async Task RemovePageAsync(PBPage browserPage)
        {
            await browserPage.Page.CloseAsync().ConfigureAwait(false);
            _sessionStateService.Remove(browserPage.PageId);
        }

        public async Task<PBPage> NavigatePageAsync(PBPage browserPage, string url, bool waitForLoad)
        {
            if (waitForLoad)
            {
                await browserPage.Page.GoToAsync(url, new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await browserPage.Page.GoToAsync(url).ConfigureAwait(false);
            }
            return Save(browserPage);
        }

        public async Task<byte[]> GetPageScreenshotAsync(PBPage browserPage, string filePath = null, bool fullPage = false)
        {
            var screenshotOptions = new ScreenshotOptions
            {
                FullPage = fullPage,
                Type = ScreenshotType.Png
            };

            if (!string.IsNullOrEmpty(filePath))
            {
                // Save screenshot to file if path is provided
                await browserPage.Page.ScreenshotAsync(filePath, screenshotOptions).ConfigureAwait(false);
            }

            // Always return the screenshot data
            return await browserPage.Page.ScreenshotDataAsync(screenshotOptions).ConfigureAwait(false);
        }

        public async Task<T> ExecuteScriptAsync<T>(PBPage browserPage, string script, params object[] args)
        {
            return await browserPage.Page.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
        }

        public async Task ExecuteScriptAsync(PBPage browserPage, string script, params object[] args)
        {
            await browserPage.Page.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
        }

        public async Task<List<PBCookie>> GetCookiesAsync(PBPage browserPage)
        {
            var puppeteerCookies = await browserPage.Page.GetCookiesAsync().ConfigureAwait(false);
            var cookies = new List<PBCookie>();
            foreach (var c in puppeteerCookies)
            {
                DateTime? expires = null;
                if (c.Expires.HasValue)
                {
                    expires = DateTimeOffset.FromUnixTimeSeconds((long)c.Expires.Value).DateTime;
                }
                cookies.Add(new PBCookie(
                    name:c.Name,
                    value: c.Value,
                    domain: c.Domain,
                    path: c.Path,
                    expires: expires,
                    httpOnly: c.HttpOnly,
                    secure: c.Secure,
                    sameSite: c.SameSite.ToSupportedPBSameSite(),
                    url: c.Url
                ));
            }
            return cookies;
        }


        public async Task DeleteCookiesAsync(PBPage browserPage, PBCookie[] cookies)
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
            await browserPage.Page.DeleteCookieAsync(puppeteerCookies.ToArray()).ConfigureAwait(false);
        }


        public async Task SetCookiesAsync(PBPage browserPage, PBCookie[] cookies)
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
            await browserPage.Page.SetCookieAsync(puppeteerCookies.ToArray()).ConfigureAwait(false);
        }

        public async Task<PBPage> NavigateBackAsync(PBPage browserPage, bool waitForLoad)
        {
            if (waitForLoad)
            {
                await browserPage.Page.GoBackAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await browserPage.Page.GoBackAsync().ConfigureAwait(false);
            }

            return Save(browserPage);
        }

        public async Task<PBPage> NavigateForwardAsync(PBPage browserPage, bool waitForLoad)
        {
            if (waitForLoad)
            {
                await browserPage.Page.GoForwardAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await browserPage.Page.GoForwardAsync().ConfigureAwait(false);
            }

            return Save(browserPage);
        }
        public async Task<PBPage> ReloadPageAsync(PBPage browserPage, bool waitForLoad)
        {
            if (waitForLoad)
            {
                await browserPage.Page.ReloadAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await browserPage.Page.ReloadAsync().ConfigureAwait(false);
            }
            return Save(browserPage);
        }
    }
}