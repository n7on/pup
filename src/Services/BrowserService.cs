using System;
using PuppeteerSharp;
using System.IO;
using System.Linq;
using Pup.Transport;
using System.Management.Automation;
using System.Collections.Generic;
using Pup.Common;
using System.Threading.Tasks;

namespace Pup.Services
{

    public class BrowserService : SupportedBrowserService, IBrowserService
    {
        private readonly PupBrowser _browser;

        public BrowserService(PupBrowser browser, SessionState sessionState) : base(sessionState)
        {
            _browser = browser;
        }

        public async Task<List<PupPage>> GetPagesAsync()
        {
            var pages = await _browser.Browser.PagesAsync().ConfigureAwait(false);

            var pupPages = new List<PupPage>();
            foreach (var page in pages)
            {
                var title = await page.GetTitleAsync().ConfigureAwait(false);
                pupPages.Add(new PupPage(page, title));
            }

            return pupPages;
        }

        public async Task<PupPage> CreatePageAsync(string name, int width, int height, string url, bool waitForLoad)
        {
            var pages = await _browser.Browser.PagesAsync().ConfigureAwait(false);
            string pageName = string.IsNullOrEmpty(name) ? $"Page{pages.Length + 1}" : name;

            var page = await _browser.Browser.NewPageAsync().ConfigureAwait(false);

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

            var title = await page.GetTitleAsync().ConfigureAwait(false);
            return new PupPage(page, title);
        }

        public bool RemoveBrowser()
        {
            if (Directory.Exists(_browser.Path))
            {
                Directory.Delete(_browser.Path, true);
                return true;
            }
            return false;
        }

        public bool StopBrowser()
        {

            if (!_browser.Running)
            {
                return false;
            }

            _browser.Browser.CloseAsync().GetAwaiter().GetResult();

            _sessionStateService.Remove(_browser.BrowserType.ToString());

            return true;
        }
    }
}