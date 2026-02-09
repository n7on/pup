using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.Json;
using System.Threading.Tasks;
using Pup.Common;
using Pup.Transport;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using PuppeteerSharp.Media;

namespace Pup.Services
{

    public interface IPageService
    {
        Task RemovePageAsync();

        Task<PupElement> FindElementBySelectorAsync(string selector, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsBySelectorAsync(string selector, bool waitForLoad, int timeout);
        Task<PupElement> FindElementByXPathAsync(string xpath, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsByXPathAsync(string xpath, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsByTextAsync(string text, bool exactMatch, string selector);
        Task ClickElementBySelectorAsync(string selector);
        Task ClickElementByCoordinatesAsync(double x, double y);
        Task FocusElementBySelectorAsync(string selector);
        
        Task HoverElementBySelectorAsync(string selector);
        
        Task WaitForElementAsync(string selector, int timeout);
        Task WaitForElementToBeVisibleAsync(string selector, int timeout);
        Task WaitForElementToBeHiddenAsync(string selector, int timeout);
        Task WaitForElementConditionAsync(string selector, string condition, string textArg, string attributeName, string attributeValue, int timeout, int pollingInterval);
        Task<PupPage> NavigatePageAsync(string url, bool waitForLoad);

        Task<byte[]> GetPageScreenshotAsync(string filePath = null, bool fullPage = false);

        Task<T> ExecuteScriptAsync<T>(string script, params object[] args);
        Task ExecuteScriptAsync(string script, params object[] args);
        Task<object> ExecuteScriptWithConversionAsync(string script, params object[] args);

        Task<List<PupCookie>> GetCookiesAsync();
        Task DeleteCookiesAsync(PupCookie[] cookies);
        Task SetCookiesAsync(PupCookie[] cookies);

        Task<PupPage> NavigateBackAsync(bool waitForLoad);
        Task<PupPage> NavigateForwardAsync(bool waitForLoad);
        Task<PupPage> ReloadPageAsync(bool waitForLoad);

        // Keyboard
        Task SendKeyAsync(string key, string[] modifiers = null);
        Task SendKeysAsync(string text);

        // Dialog handling
        void SetDialogHandler(PupDialogAction action, string promptText = null);
        void RemoveDialogHandler();

        // PDF
        Task<byte[]> ExportPdfAsync(string filePath = null, bool landscape = false, bool printBackground = true, string format = "A4", decimal scale = 1);

        // Storage
        Task<Dictionary<string, string>> GetStorageAsync(string type);
        Task SetStorageAsync(string type, Dictionary<string, string> items);
        Task ClearStorageAsync(string type, string key = null);

        // Extra HTTP headers
        Task SetExtraHeadersAsync(Dictionary<string, string> headers);

        // HTTP Basic Authentication
        Task SetAuthenticationAsync(string username, string password);
        Task ClearAuthenticationAsync();

        // HTTP Fetch
        Task<PupFetchResponse> FetchAsync(string url, string method, object body, Dictionary<string, string> headers, string contentType, int timeout, bool parseJsonBody = false);

        // Viewport
        Task SetViewportAsync(int width, int height, double deviceScaleFactor = 1, bool isMobile = false, bool hasTouch = false, bool isLandscape = false);

        // Session Export/Import
        Task<PupSession> ExportSessionAsync();
        Task ImportSessionAsync(PupSession session, bool includeCookies = true, bool includeLocalStorage = true, bool includeSessionStorage = true);
    }


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

        public async Task<List<PupElement>> FindElementsByTextAsync(string text, bool exactMatch, string selector = null)
        {
            // JavaScript that finds elements by visible text and returns the most specific matches
            var script = @"(searchText, exactMatch, cssSelector) => {
                const normalize = (s) => (s || '').trim().replace(/\s+/g, ' ');
                const searchNorm = normalize(searchText);

                // Get candidate elements
                let candidates;
                if (cssSelector) {
                    candidates = [...document.querySelectorAll(cssSelector)];
                } else {
                    candidates = [...document.querySelectorAll('*')];
                }

                // Filter to elements whose innerText matches
                const matches = candidates.filter(el => {
                    const text = normalize(el.innerText);
                    if (exactMatch) {
                        return text === searchNorm;
                    } else {
                        return text.toLowerCase().includes(searchNorm.toLowerCase());
                    }
                });

                // Filter to most specific elements (no child also matches)
                const specific = matches.filter(el => {
                    const dominated = matches.some(other =>
                        other !== el && el.contains(other)
                    );
                    return !dominated;
                });

                return specific;
            }";

            var elementHandles = await _page.Page.EvaluateFunctionHandleAsync(script, text, exactMatch, selector).ConfigureAwait(false);
            var jsHandle = elementHandles as PuppeteerSharp.IJSHandle;

            // Get properties to iterate over array elements
            var properties = await jsHandle.GetPropertiesAsync().ConfigureAwait(false);

            var pbElements = new List<PupElement>();
            int index = 0;

            foreach (var prop in properties)
            {
                // Skip non-numeric properties (like 'length')
                if (!int.TryParse(prop.Key, out _))
                    continue;

                if (prop.Value is IElementHandle element)
                {
                    // Generate a stable CSS selector for this element
                    var elementSelector = await element.EvaluateFunctionAsync<string>(GenerateUniqueSelectorScript).ConfigureAwait(false);

                    pbElements.Add(new PupElement(
                        element,
                        _page.Page,
                        Guid.NewGuid().ToString(),
                        elementSelector,
                        index++,
                        await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                        await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                        await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                        await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                        await element.IsIntersectingViewportAsync().ConfigureAwait(false)
                    ));
                }
            }

            return pbElements;
        }

        private const string GenerateUniqueSelectorScript = @"
            (element) => {
                // Try ID first (most stable)
                if (element.id) return '#' + CSS.escape(element.id);

                // Build path from element to a unique ancestor
                const path = [];
                let current = element;

                while (current && current.nodeType === Node.ELEMENT_NODE) {
                    let selector = current.tagName.toLowerCase();

                    // If element has ID, use it and stop
                    if (current.id) {
                        selector = '#' + CSS.escape(current.id);
                        path.unshift(selector);
                        break;
                    }

                    // Add nth-child for uniqueness among siblings
                    const parent = current.parentElement;
                    if (parent) {
                        const siblings = [...parent.children];
                        const sameTagSiblings = siblings.filter(s => s.tagName === current.tagName);
                        if (sameTagSiblings.length > 1) {
                            const index = sameTagSiblings.indexOf(current) + 1;
                            selector += ':nth-of-type(' + index + ')';
                        }
                    }

                    path.unshift(selector);

                    // Stop at body
                    if (current.tagName.toLowerCase() === 'body') break;

                    current = parent;
                }

                return path.join(' > ');
            }";

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

        public async Task WaitForElementConditionAsync(string selector, string condition, string textArg, string attributeName, string attributeValue, int timeout = 30000, int pollingInterval = 200)
        {
            string script = @"(selector, condition, textArg, attrName, attrValue) => {
                const el = document.querySelector(selector);
                const isVisible = (node) => !!(node && (node.offsetWidth || node.offsetHeight || node.getClientRects().length));

                if (condition === 'hidden') {
                    return !el || !isVisible(el);
                }

                if (!el) return false;

                switch (condition) {
                    case 'visible':
                        return isVisible(el);
                    case 'enabled':
                        return !el.disabled;
                    case 'disabled':
                        return !!el.disabled;
                    case 'textContains':
                        return (el.innerText || '').toLowerCase().includes((textArg || '').toLowerCase());
                    case 'attributeEquals':
                        return (el.getAttribute(attrName) || '') === (attrValue || '');
                    default:
                        return false;
                }
            }";

            await _page.Page.WaitForFunctionAsync(script, new WaitForFunctionOptions
            {
                Timeout = timeout,
                PollingInterval = pollingInterval
            }, selector, condition, textArg, attributeName, attributeValue).ConfigureAwait(false);
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

        public async Task<object> ExecuteScriptWithConversionAsync(string script, params object[] args)
        {
            var result = await _page.Page.EvaluateFunctionAsync<object>(script, args).ConfigureAwait(false);
            if (result is JsonElement element)
            {
                return JsonHelper.ConvertJsonElement(element);
            }
            return result;
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

        public async Task<Dictionary<string, string>> GetStorageAsync(string type)
        {
            return await _page.Page.EvaluateFunctionAsync<Dictionary<string, string>>(@"(storageType) => {
                const storage = storageType === 'session' ? window.sessionStorage : window.localStorage;
                const result = {};
                for (let i = 0; i < storage.length; i++) {
                    const key = storage.key(i);
                    result[key] = storage.getItem(key);
                }
                return result;
            }", type?.ToLowerInvariant() == "session" ? "session" : "local").ConfigureAwait(false);
        }

        public async Task SetStorageAsync(string type, Dictionary<string, string> items)
        {
            await _page.Page.EvaluateFunctionAsync(@"(storageType, items) => {
                const storage = storageType === 'session' ? window.sessionStorage : window.localStorage;
                Object.keys(items || {}).forEach(k => {
                    storage.setItem(k, items[k] ?? '');
                });
            }", type?.ToLowerInvariant() == "session" ? "session" : "local", items ?? new Dictionary<string, string>()).ConfigureAwait(false);
        }

        public async Task ClearStorageAsync(string type, string key = null)
        {
            await _page.Page.EvaluateFunctionAsync(@"(storageType, key) => {
                const storage = storageType === 'session' ? window.sessionStorage : window.localStorage;
                if (key) {
                    storage.removeItem(key);
                } else {
                    storage.clear();
                }
            }", type?.ToLowerInvariant() == "session" ? "session" : "local", key).ConfigureAwait(false);
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

        public async Task SetExtraHeadersAsync(Dictionary<string, string> headers)
        {
            await _page.Page.SetExtraHttpHeadersAsync(headers).ConfigureAwait(false);
        }

        public async Task SetAuthenticationAsync(string username, string password)
        {
            await _page.Page.AuthenticateAsync(new Credentials
            {
                Username = username,
                Password = password
            }).ConfigureAwait(false);
        }

        public async Task ClearAuthenticationAsync()
        {
            await _page.Page.AuthenticateAsync(null).ConfigureAwait(false);
        }

        public async Task<PupFetchResponse> FetchAsync(string url, string method, object body, Dictionary<string, string> headers, string contentType, int timeout, bool parseJsonBody = false)
        {
            // Build fetch options
            var fetchOptions = new Dictionary<string, object>
            {
                ["method"] = method ?? "GET"
            };

            // Handle headers
            var headerDict = headers != null
                ? new Dictionary<string, string>(headers)
                : new Dictionary<string, string>();

            // Handle body
            if (body != null)
            {
                string bodyString;
                if (body is string str)
                {
                    bodyString = str;
                }
                else if (body is Hashtable ht)
                {
                    bodyString = JsonSerializer.Serialize(PowerShellHelper.ConvertHashtable(ht));

                    if (string.IsNullOrEmpty(contentType) && !headerDict.ContainsKey("Content-Type"))
                    {
                        headerDict["Content-Type"] = "application/json";
                    }
                }
                else
                {
                    bodyString = JsonSerializer.Serialize(body);
                    if (string.IsNullOrEmpty(contentType) && !headerDict.ContainsKey("Content-Type"))
                    {
                        headerDict["Content-Type"] = "application/json";
                    }
                }

                fetchOptions["body"] = bodyString;
            }

            // Set explicit content-type if provided
            if (!string.IsNullOrEmpty(contentType))
            {
                headerDict["Content-Type"] = contentType;
            }

            if (headerDict.Count > 0)
            {
                fetchOptions["headers"] = headerDict;
            }

            var script = @"
async (url, options, timeout) => {
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), timeout);

    try {
        const fetchOptions = { ...options, signal: controller.signal };
        const response = await fetch(url, fetchOptions);
        clearTimeout(timeoutId);

        const headers = {};
        response.headers.forEach((v, k) => headers[k] = v);

        let body = '';
        try {
            body = await response.text();
        } catch (e) {
            // Body may not be readable
        }

        return {
            status: response.status,
            statusText: response.statusText,
            headers: headers,
            body: body,
            ok: response.ok,
            url: response.url
        };
    } catch (e) {
        clearTimeout(timeoutId);
        if (e.name === 'AbortError') {
            throw new Error('Request timed out');
        }
        throw e;
    }
}";

            var jsonResult = await _page.Page.EvaluateFunctionAsync<JsonElement>(
                script,
                url,
                fetchOptions,
                timeout
            ).ConfigureAwait(false);

            var response = new PupFetchResponse
            {
                Status = jsonResult.GetProperty("status").GetInt32(),
                StatusText = jsonResult.GetProperty("statusText").GetString(),
                Body = jsonResult.GetProperty("body").GetString(),
                Ok = jsonResult.GetProperty("ok").GetBoolean(),
                Url = jsonResult.GetProperty("url").GetString()
            };

            // Parse headers
            var headersElement = jsonResult.GetProperty("headers");
            foreach (var prop in headersElement.EnumerateObject())
            {
                response.Headers[prop.Name] = prop.Value.GetString();
            }

            // Parse JSON body if requested
            if (parseJsonBody && !string.IsNullOrEmpty(response.Body))
            {
                try
                {
                    using var doc = JsonDocument.Parse(response.Body);
                    response.JsonBody = JsonHelper.ConvertJsonElement(doc.RootElement);
                }
                catch
                {
                    // Leave JsonBody as null if parsing fails
                }
            }

            return response;
        }

        public async Task SetViewportAsync(int width, int height, double deviceScaleFactor = 1, bool isMobile = false, bool hasTouch = false, bool isLandscape = false)
        {
            await _page.Page.SetViewportAsync(new ViewPortOptions
            {
                Width = width,
                Height = height,
                DeviceScaleFactor = deviceScaleFactor,
                IsMobile = isMobile,
                HasTouch = hasTouch,
                IsLandscape = isLandscape
            }).ConfigureAwait(false);
        }

        public async Task<PupSession> ExportSessionAsync()
        {
            var session = new PupSession
            {
                Url = _page.Page.Url,
                ExportedAt = DateTime.UtcNow,
                Cookies = await GetCookiesAsync().ConfigureAwait(false),
                LocalStorage = await GetStorageAsync("local").ConfigureAwait(false),
                SessionStorage = await GetStorageAsync("session").ConfigureAwait(false)
            };

            return session;
        }

        public async Task ImportSessionAsync(PupSession session, bool includeCookies = true, bool includeLocalStorage = true, bool includeSessionStorage = true)
        {
            if (includeCookies && session.Cookies?.Count > 0)
            {
                await SetCookiesAsync(session.Cookies.ToArray()).ConfigureAwait(false);
            }

            if (includeLocalStorage && session.LocalStorage?.Count > 0)
            {
                await SetStorageAsync("local", session.LocalStorage).ConfigureAwait(false);
            }

            if (includeSessionStorage && session.SessionStorage?.Count > 0)
            {
                await SetStorageAsync("session", session.SessionStorage).ConfigureAwait(false);
            }
        }

    }
}
