using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pup.Common;
using Pup.Transport;
using PuppeteerSharp;

namespace Pup.Services
{
    public interface IFrameService
    {
        Task<PupElement> FindElementBySelectorAsync(string selector, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsBySelectorAsync(string selector, bool waitForLoad, int timeout);
        Task<PupElement> FindElementByXPathAsync(string xpath, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsByXPathAsync(string xpath, bool waitForLoad, int timeout);
        Task<List<PupElement>> FindElementsByTextAsync(string text, bool exactMatch, string selector);
        Task WaitForElementAsync(string selector, int timeout);
        Task WaitForElementToBeVisibleAsync(string selector, int timeout);
        Task WaitForElementToBeHiddenAsync(string selector, int timeout);
        Task<T> ExecuteScriptAsync<T>(string script, params object[] args);
        Task ExecuteScriptAsync(string script, params object[] args);
        Task<object> ExecuteScriptWithConversionAsync(string script, params object[] args);
        Task<string> GetSourceAsync();
    }

    public class FrameService : IFrameService
    {
        private readonly PupFrame _frame;

        public FrameService(PupFrame frame)
        {
            _frame = frame;
        }

        public async Task<PupElement> FindElementBySelectorAsync(string selector, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle element;
            if (waitForLoad)
            {
                element = await _frame.Frame.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
            }
            else
            {
                element = await _frame.Frame.QuerySelectorAsync(selector).ConfigureAwait(false);
            }

            if (element == null)
            {
                return null;
            }

            var page = _frame.Frame.Page;

            return new PupElement(
                element: element,
                page: page,
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
                await _frame.Frame.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
                elements = await _frame.Frame.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            }
            else
            {
                elements = await _frame.Frame.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            }

            var page = _frame.Frame.Page;
            var pupElements = new List<PupElement>();

            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                pupElements.Add(new PupElement(
                    element,
                    page,
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
            return pupElements;
        }

        public async Task<PupElement> FindElementByXPathAsync(string xpath, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle element;
            if (waitForLoad)
            {
                element = await _frame.Frame.WaitForSelectorAsync($"xpath/{xpath}", new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
            }
            else
            {
                element = await _frame.Frame.QuerySelectorAsync($"xpath/{xpath}").ConfigureAwait(false);
            }

            if (element == null)
            {
                return null;
            }

            var page = _frame.Frame.Page;

            return new PupElement(
                element: element,
                page: page,
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
                await _frame.Frame.WaitForSelectorAsync($"xpath/{xpath}", new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
                elements = await _frame.Frame.QuerySelectorAllAsync($"xpath/{xpath}").ConfigureAwait(false);
            }
            else
            {
                elements = await _frame.Frame.QuerySelectorAllAsync($"xpath/{xpath}").ConfigureAwait(false);
            }

            var page = _frame.Frame.Page;
            var pupElements = new List<PupElement>();

            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                pupElements.Add(new PupElement(
                    element,
                    page,
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
            return pupElements;
        }

        public async Task<List<PupElement>> FindElementsByTextAsync(string text, bool exactMatch, string selector = null)
        {
            var script = @"(searchText, exactMatch, cssSelector) => {
                const normalize = (s) => (s || '').trim().replace(/\s+/g, ' ');
                const searchNorm = normalize(searchText);

                let candidates;
                if (cssSelector) {
                    candidates = [...document.querySelectorAll(cssSelector)];
                } else {
                    candidates = [...document.querySelectorAll('*')];
                }

                const matches = candidates.filter(el => {
                    const text = normalize(el.innerText);
                    if (exactMatch) {
                        return text === searchNorm;
                    } else {
                        return text.toLowerCase().includes(searchNorm.toLowerCase());
                    }
                });

                const specific = matches.filter(el => {
                    const dominated = matches.some(other =>
                        other !== el && el.contains(other)
                    );
                    return !dominated;
                });

                return specific;
            }";

            var jsHandle = await _frame.Frame.EvaluateFunctionHandleAsync(script, text, exactMatch, selector).ConfigureAwait(false);
            return await ElementHelper.CreatePupElementsFromHandleAsync(jsHandle, _frame.Frame.Page).ConfigureAwait(false);
        }

        public async Task WaitForElementAsync(string selector, int timeout = 30000)
        {
            await _frame.Frame.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
        }

        public async Task WaitForElementToBeVisibleAsync(string selector, int timeout = 30000)
        {
            await _frame.Frame.WaitForSelectorAsync(selector, new WaitForSelectorOptions
            {
                Timeout = timeout,
                Visible = true
            }).ConfigureAwait(false);
        }

        public async Task WaitForElementToBeHiddenAsync(string selector, int timeout = 30000)
        {
            await _frame.Frame.WaitForSelectorAsync(selector, new WaitForSelectorOptions
            {
                Timeout = timeout,
                Hidden = true
            }).ConfigureAwait(false);
        }

        public async Task<T> ExecuteScriptAsync<T>(string script, params object[] args)
        {
            return await _frame.Frame.EvaluateFunctionAsync<T>(script, args).ConfigureAwait(false);
        }

        public async Task ExecuteScriptAsync(string script, params object[] args)
        {
            await _frame.Frame.EvaluateFunctionAsync(script, args).ConfigureAwait(false);
        }

        public async Task<object> ExecuteScriptWithConversionAsync(string script, params object[] args)
        {
            var result = await _frame.Frame.EvaluateFunctionAsync<object>(script, args).ConfigureAwait(false);
            if (result is System.Text.Json.JsonElement element)
            {
                return JsonHelper.ConvertJsonElement(element);
            }
            return result;
        }

        public async Task<string> GetSourceAsync()
        {
            return await _frame.Frame.EvaluateFunctionAsync<string>("() => document.documentElement.outerHTML").ConfigureAwait(false);
        }
    }
}
