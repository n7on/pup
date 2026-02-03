using System;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;
using Pup.Transport;
using Pup.Common;
using System.Management.Automation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pup.Services
{

    public class PageService : IPageService
    {

        private readonly PupPage _page;
        public PageService(PupPage page)
        {
            _page = page;
        }
        public async Task RemovePageAsync()
        {
            await _page.Page.CloseAsync().ConfigureAwait(false);
        }

        public async Task<PupElement> FindElementBySelectorAsync(string selector, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle element;
            if (waitForLoad)
            {
                element = await _page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
            }
            else
            {
                element = await _page.Page.QuerySelectorAsync(selector).ConfigureAwait(false);
            }
            if (element == null)
            {
                return null;
            }
            return new PupElement(
                element: element,
                page: _page.Page,
                elementId: Guid.NewGuid().ToString(),
                selector: selector,
                index: 0,
                tagName: await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                innerText: await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                innerHTML: await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                id: await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                isVisible: await element.IsIntersectingViewportAsync().ConfigureAwait(false)
            );
        }

        public async Task<List<PupElement>> FindElementsBySelectorAsync(string selector, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle[] elements;
            if (waitForLoad)
            {
                await _page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
                elements = await _page.Page.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            }
            else
            {
                elements = await _page.Page.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            }

            var pbElements = new List<PupElement>();
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                pbElements.Add(new PupElement(
                    element,
                    _page.Page,
                    Guid.NewGuid().ToString(),
                    selector,
                    i,
                    await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                    await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                    await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                    await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                    await element.IsIntersectingViewportAsync().ConfigureAwait(false)
                ));
            }
            return pbElements;
        }

        public async Task<PupElement> FindElementByXPathAsync(string xpath, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle element;
            if (waitForLoad)
            {
                element = await _page.Page.WaitForSelectorAsync($"xpath/{xpath}", new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
            }
            else
            {
                element = await _page.Page.QuerySelectorAsync($"xpath/{xpath}").ConfigureAwait(false);
            }
            if (element == null)
            {
                return null;
            }
            return new PupElement(
                element: element,
                page: _page.Page,
                elementId: Guid.NewGuid().ToString(),
                selector: xpath,
                index: 0,
                tagName: await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                innerText: await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                innerHTML: await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                id: await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                isVisible: await element.IsIntersectingViewportAsync().ConfigureAwait(false)
            );
        }

        public async Task<List<PupElement>> FindElementsByXPathAsync(string xpath, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle[] elements;
            if (waitForLoad)
            {
                await _page.Page.WaitForSelectorAsync($"xpath/{xpath}", new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
                elements = await _page.Page.QuerySelectorAllAsync($"xpath/{xpath}").ConfigureAwait(false);
            }
            else
            {
                elements = await _page.Page.QuerySelectorAllAsync($"xpath/{xpath}").ConfigureAwait(false);
            }

            var pbElements = new List<PupElement>();
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                pbElements.Add(new PupElement(
                    element,
                    _page.Page,
                    Guid.NewGuid().ToString(),
                    xpath,
                    i,
                    await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                    await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                    await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                    await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                    await element.IsIntersectingViewportAsync().ConfigureAwait(false)
                ));
            }
            return pbElements;
        }

        public async Task ClickElementBySelectorAsync(string selector)
        {
            await _page.Page.ClickAsync(selector).ConfigureAwait(false);
        }

        public async Task ClickElementByCoordinatesAsync(double x, double y)
        {
            await _page.Page.Mouse.ClickAsync((decimal)x, (decimal)y).ConfigureAwait(false);
        }
        public async Task HoverElementBySelectorAsync(string selector)
        {
            await _page.Page.HoverAsync(selector).ConfigureAwait(false);
        }
        public async Task FocusElementBySelectorAsync(string selector)
        {
            await _page.Page.FocusAsync(selector).ConfigureAwait(false);
        }
        public async Task WaitForElementAsync(string selector, int timeout = 30000)
        {
            await _page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
        }

        public async Task WaitForElementToBeVisibleAsync(string selector, int timeout = 30000)
        {
            await _page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions 
            { 
                Timeout = timeout,
                Visible = true 
            }).ConfigureAwait(false);
        }

        public async Task WaitForElementToBeHiddenAsync(string selector, int timeout = 30000)
        {
            await _page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions 
            { 
                Timeout = timeout,
                Hidden = true 
            }).ConfigureAwait(false);
        }
        public async Task<PupPage> NavigatePageAsync(string url, bool waitForLoad)
        {
            if (waitForLoad)
            {
                await _page.Page.GoToAsync(url, new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await _page.Page.GoToAsync(url).ConfigureAwait(false);
            }
            return _page;
        }

        public async Task<byte[]> GetPageScreenshotAsync(string filePath = null, bool fullPage = false)
        {
            var screenshotOptions = new ScreenshotOptions
            {
                FullPage = fullPage,
                Type = ScreenshotType.Png
            };

            if (!string.IsNullOrEmpty(filePath))
            {
                // Save screenshot to file if path is provided
                await _page.Page.ScreenshotAsync(filePath, screenshotOptions).ConfigureAwait(false);
            }

            // Always return the screenshot data
            return await _page.Page.ScreenshotDataAsync(screenshotOptions).ConfigureAwait(false);
        }

        public async Task<T> ExecuteScriptAsync<T>(string script, params object[] args)
        {
            return await _page.Page.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
        }

        public async Task ExecuteScriptAsync(string script, params object[] args)
        {
            await _page.Page.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
        }

        public async Task<List<PupCookie>> GetCookiesAsync()
        {
            var puppeteerCookies = await _page.Page.GetCookiesAsync().ConfigureAwait(false);
            var cookies = new List<PupCookie>();
            foreach (var c in puppeteerCookies)
            {
                DateTime? expires = null;
                if (c.Expires.HasValue)
                {
                    expires = DateTimeOffset.FromUnixTimeSeconds((long)c.Expires.Value).DateTime;
                }
                cookies.Add(new PupCookie(
                    name:c.Name,
                    value: c.Value,
                    domain: c.Domain,
                    path: c.Path,
                    expires: expires,
                    httpOnly: c.HttpOnly,
                    secure: c.Secure,
                    sameSite: c.SameSite.ToPupSameSite(),
                    url: c.Url
                ));
            }
            return cookies;
        }


        public async Task DeleteCookiesAsync(PupCookie[] cookies)
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
            await _page.Page.DeleteCookieAsync(puppeteerCookies.ToArray()).ConfigureAwait(false);
        }


        public async Task SetCookiesAsync(PupCookie[] cookies)
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
            await _page.Page.SetCookieAsync(puppeteerCookies.ToArray()).ConfigureAwait(false);
        }

        public async Task<PupPage> NavigateBackAsync(bool waitForLoad)
        {
            if (waitForLoad)
            {
                await _page.Page.GoBackAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await _page.Page.GoBackAsync().ConfigureAwait(false);
            }

            return _page;
        }

        public async Task<PupPage> NavigateForwardAsync(bool waitForLoad)
        {
            if (waitForLoad)
            {
                await _page.Page.GoForwardAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await _page.Page.GoForwardAsync().ConfigureAwait(false);
            }

            return _page;
        }
        public async Task<PupPage> ReloadPageAsync(bool waitForLoad)
        {
            if (waitForLoad)
            {
                await _page.Page.ReloadAsync(new NavigationOptions
                {
                    WaitUntil = new[] { WaitUntilNavigation.Load, WaitUntilNavigation.DOMContentLoaded }
                }).ConfigureAwait(false);
            }
            else
            {
                await _page.Page.ReloadAsync().ConfigureAwait(false);
            }
            return _page;
        }

        public async Task SendKeyAsync(string key, string[] modifiers = null)
        {
            if (modifiers != null && modifiers.Length > 0)
            {
                // Hold down modifier keys
                foreach (var modifier in modifiers)
                {
                    await _page.Page.Keyboard.DownAsync(modifier).ConfigureAwait(false);
                }
            }

            // Press the main key
            await _page.Page.Keyboard.PressAsync(key).ConfigureAwait(false);

            if (modifiers != null && modifiers.Length > 0)
            {
                // Release modifier keys in reverse order
                for (int i = modifiers.Length - 1; i >= 0; i--)
                {
                    await _page.Page.Keyboard.UpAsync(modifiers[i]).ConfigureAwait(false);
                }
            }
        }

        public async Task SendKeysAsync(string text)
        {
            await _page.Page.Keyboard.TypeAsync(text).ConfigureAwait(false);
        }

        public void SetDialogHandler(PupDialogAction action, string promptText = null)
        {
            // Remove existing handler if any
            RemoveDialogHandler();

            _page.DialogHandler = async (sender, e) =>
            {
                if (action == PupDialogAction.Accept)
                {
                    await e.Dialog.Accept(promptText).ConfigureAwait(false);
                }
                else
                {
                    await e.Dialog.Dismiss().ConfigureAwait(false);
                }
            };

            _page.Page.Dialog += _page.DialogHandler;
        }

        public void RemoveDialogHandler()
        {
            if (_page.DialogHandler != null)
            {
                _page.Page.Dialog -= _page.DialogHandler;
                _page.DialogHandler = null;
            }
        }

        public async Task<byte[]> ExportPdfAsync(string filePath = null, bool landscape = false, bool printBackground = true, string format = "A4", decimal scale = 1)
        {
            var options = new PdfOptions
            {
                Landscape = landscape,
                PrintBackground = printBackground,
                Scale = scale,
                Format = format switch
                {
                    "Letter" => PaperFormat.Letter,
                    "Legal" => PaperFormat.Legal,
                    "Tabloid" => PaperFormat.Tabloid,
                    "Ledger" => PaperFormat.Ledger,
                    "A0" => PaperFormat.A0,
                    "A1" => PaperFormat.A1,
                    "A2" => PaperFormat.A2,
                    "A3" => PaperFormat.A3,
                    "A4" => PaperFormat.A4,
                    "A5" => PaperFormat.A5,
                    "A6" => PaperFormat.A6,
                    _ => PaperFormat.A4
                }
            };

            if (!string.IsNullOrEmpty(filePath))
            {
                await _page.Page.PdfAsync(filePath, options).ConfigureAwait(false);
            }

            return await _page.Page.PdfDataAsync(options).ConfigureAwait(false);
        }
    }
}