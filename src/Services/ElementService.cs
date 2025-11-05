using System;
using System.Collections.Generic;
using System.Linq;
using PuppeteerSharp;
using PowerBrowser.Transport;
using System.Management.Automation.Language;
using System.Threading.Tasks;

namespace PowerBrowser.Services
{
    public class ElementService : IElementService
    {
        public async Task<PBElement> FindElementBySelectorAsync(PBPage browserPage, string selector, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle element;
            if (waitForLoad)
            {
                element = await browserPage.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
            }
            else
            {
                element = await browserPage.Page.QuerySelectorAsync(selector).ConfigureAwait(false);
            }
            if (element == null)
            {
                return null;
            }
            return new PBElement(
                element,
                browserPage.Page,
                Guid.NewGuid().ToString(),
                browserPage.PageName,
                selector,
                0,
                await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                await element.IsIntersectingViewportAsync().ConfigureAwait(false)
            );
        }

        public async Task<List<PBElement>> FindElementsBySelectorAsync(PBPage page, string selector, bool waitForLoad = false, int timeout = 30000)
        {
            IElementHandle[] elements;
            if (waitForLoad)
            {
                await page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
                elements = await page.Page.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            }
            else
            {
                elements = await page.Page.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            }

            var pbElements = new List<PBElement>();
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                pbElements.Add(new PBElement(
                    element,
                    page.Page,
                    Guid.NewGuid().ToString(),
                    page.PageName,
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

        public async Task ClickElementAsync(PBElement element)
        {
            await element.Element.ClickAsync().ConfigureAwait(false);
        }

        public async Task ClickElementBySelectorAsync(PBPage page, string selector)
        {
            await page.Page.ClickAsync(selector).ConfigureAwait(false);
        }

        public async Task ClickElementByCoordinatesAsync(PBPage page, double x, double y)
        {
            await page.Page.Mouse.ClickAsync((decimal)x, (decimal)y).ConfigureAwait(false);
        }

        public async Task SetElementTextAsync(PBElement element, string text)
        {
            await element.Element.TypeAsync(text).ConfigureAwait(false);
        }

        public async Task SetElementValueAsync(PBElement element, string value)
        {
            await element.Element.EvaluateFunctionAsync("el => el.value = arguments[0]", value).ConfigureAwait(false);
        }

        public async Task<string> GetElementAttributeAsync(PBElement element, string attributeName)
        {
            return await element.Element.EvaluateFunctionAsync<string>("(el, attr) => el.getAttribute(attr)", attributeName).ConfigureAwait(false);
        }

        public async Task<string> GetElementTextAsync(PBElement element)
        {
            return await element.Element.EvaluateFunctionAsync<string>("el => el.textContent").ConfigureAwait(false);
        }

        public async Task<string> GetElementValueAsync(PBElement element)
        {
            return await element.Element.EvaluateFunctionAsync<string>("el => el.value").ConfigureAwait(false);
        }

        public async Task HoverElementAsync(PBElement element)
        {
            await element.Element.HoverAsync().ConfigureAwait(false);
        }

        public async Task HoverElementBySelectorAsync(PBPage page, string selector)
        {
            await page.Page.HoverAsync(selector).ConfigureAwait(false);
        }

        public async Task FocusElementAsync(PBElement element)
        {
            await element.Element.FocusAsync().ConfigureAwait(false);
        }

        public async Task FocusElementBySelectorAsync(PBPage page, string selector)
        {
            await page.Page.FocusAsync(selector).ConfigureAwait(false);
        }

        public async Task<bool> IsElementVisibleAsync(PBElement element)
        {
            return await element.Element.EvaluateFunctionAsync<bool>("el => !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length)").ConfigureAwait(false);
        }

        public async Task<bool> IsElementEnabledAsync(PBElement element)
        {
            return await element.Element.EvaluateFunctionAsync<bool>("el => !el.disabled").ConfigureAwait(false);
        }

        public async Task WaitForElementAsync(PBPage page, string selector, int timeout = 30000)
        {
            await page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = timeout }).ConfigureAwait(false);
        }

        public async Task WaitForElementToBeVisibleAsync(PBPage page, string selector, int timeout = 30000)
        {
            await page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions 
            { 
                Timeout = timeout,
                Visible = true 
            }).ConfigureAwait(false);
        }

        public async Task WaitForElementToBeHiddenAsync(PBPage page, string selector, int timeout = 30000)
        {
            await page.Page.WaitForSelectorAsync(selector, new WaitForSelectorOptions 
            { 
                Timeout = timeout,
                Hidden = true 
            }).ConfigureAwait(false);
        }
    }
}