using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;
using Pup.Services;
using Pup.Transport;

namespace Pup.Common
{
    public static class ElementHelper
    {
        private static readonly string SelectorGeneratorScript = EmbeddedResourceService.LoadScript("selector-generator.js");

        /// <summary>
        /// Creates a list of PupElement objects from a JSHandle containing an array of elements.
        /// </summary>
        /// <param name="jsHandle">JSHandle containing an array of IElementHandle objects</param>
        /// <param name="page">The page the elements belong to</param>
        /// <param name="rootElement">Optional root element for generating relative selectors</param>
        public static async Task<List<PupElement>> CreatePupElementsFromHandleAsync(IJSHandle jsHandle, IPage page, IElementHandle rootElement = null)
        {
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
                    var pupElement = await CreatePupElementAsync(element, page, index++, rootElement).ConfigureAwait(false);
                    pbElements.Add(pupElement);
                }
            }

            return pbElements;
        }

        /// <summary>
        /// Creates a single PupElement from an IElementHandle.
        /// </summary>
        /// <param name="element">The element handle</param>
        /// <param name="page">The page the element belongs to</param>
        /// <param name="index">Index position of the element</param>
        /// <param name="rootElement">Optional root element for generating relative selectors</param>
        public static async Task<PupElement> CreatePupElementAsync(IElementHandle element, IPage page, int index = 0, IElementHandle rootElement = null)
        {
            // Generate selector - pass rootElement for relative selectors, or null for absolute
            var selector = rootElement != null
                ? await element.EvaluateFunctionAsync<string>(SelectorGeneratorScript, rootElement).ConfigureAwait(false)
                : await element.EvaluateFunctionAsync<string>(SelectorGeneratorScript).ConfigureAwait(false);

            return new PupElement(
                element,
                page,
                Guid.NewGuid().ToString(),
                selector,
                index,
                await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                await element.IsIntersectingViewportAsync().ConfigureAwait(false)
            );
        }
    }
}
