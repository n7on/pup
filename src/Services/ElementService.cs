using Pup.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pup.Services
{
    public class ElementService : IElementService
    {

        private readonly PupElement _element;

        public ElementService(PupElement element)
        {
            _element = element;
        }

        public async Task ClickElementAsync()
        {
            await _element.Element.ClickAsync().ConfigureAwait(false);
        }


        public async Task SetElementTextAsync(string text)
        {
            await _element.Element.TypeAsync(text).ConfigureAwait(false);
        }

        public async Task SetElementValueAsync(string value)
        {
            await _element.Element.EvaluateFunctionAsync("el => el.value = arguments[0]", value).ConfigureAwait(false);
        }

        public async Task<string> GetElementAttributeAsync(string attributeName)
        {
            return await _element.Element.EvaluateFunctionAsync<string>("(el, attr) => el.getAttribute(attr)", attributeName).ConfigureAwait(false);
        }

        public async Task<string> GetElementTextAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<string>("el => el.textContent").ConfigureAwait(false);
        }

        public async Task<string> GetElementValueAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<string>("el => el.value").ConfigureAwait(false);
        }

        public async Task HoverElementAsync()
        {
            await _element.Element.HoverAsync().ConfigureAwait(false);
        }


        public async Task FocusElementAsync()
        {
            await _element.Element.FocusAsync().ConfigureAwait(false);
        }


        public async Task<bool> IsElementVisibleAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<bool>("el => !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length)").ConfigureAwait(false);
        }

        public async Task<bool> IsElementEnabledAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<bool>("el => !el.disabled").ConfigureAwait(false);
        }

        public async Task<PupElement> FindElementBySelectorAsync(string selector)
        {
            var element = await _element.Element.QuerySelectorAsync(selector).ConfigureAwait(false);
            if (element == null)
            {
                return null;
            }

            return new PupElement(
                element: element,
                page: _element.Page,
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

        public async Task<List<PupElement>> FindElementsBySelectorAsync(string selector)
        {
            var elements = await _element.Element.QuerySelectorAllAsync(selector).ConfigureAwait(false);
            var results = new List<PupElement>();
            
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                results.Add(new PupElement(
                    element: element,
                    page: _element.Page,
                    elementId: Guid.NewGuid().ToString(),
                    selector: selector,
                    index: i,
                    tagName: await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                    innerText: await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                    innerHTML: await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                    id: await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                    isVisible: await element.IsIntersectingViewportAsync().ConfigureAwait(false)
                ));
            }
            
            return results;
        }

        public async Task<PupElement> FindElementByXPathAsync(string xpath)
        {
            var element = await _element.Element.QuerySelectorAsync($"xpath/{xpath}").ConfigureAwait(false);
            if (element == null)
            {
                return null;
            }

            return new PupElement(
                element: element,
                page: _element.Page,
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

        public async Task<List<PupElement>> FindElementsByXPathAsync(string xpath)
        {
            var elements = await _element.Element.QuerySelectorAllAsync($"xpath/{xpath}").ConfigureAwait(false);
            var results = new List<PupElement>();
            
            for (int i = 0; i < elements.Length; i++)
            {
                var element = elements[i];
                results.Add(new PupElement(
                    element: element,
                    page: _element.Page,
                    elementId: Guid.NewGuid().ToString(),
                    selector: xpath,
                    index: i,
                    tagName: await element.EvaluateFunctionAsync<string>("el => el.tagName").ConfigureAwait(false),
                    innerText: await element.EvaluateFunctionAsync<string>("el => el.innerText").ConfigureAwait(false),
                    innerHTML: await element.EvaluateFunctionAsync<string>("el => el.innerHTML").ConfigureAwait(false),
                    id: await element.EvaluateFunctionAsync<string>("el => el.id").ConfigureAwait(false),
                    isVisible: await element.IsIntersectingViewportAsync().ConfigureAwait(false)
                ));
            }
            
            return results;
        }

        public async Task<string> GetElementSelectorAsync(bool unique = false, bool shortest = false, bool fullPath = false)
        {
            if (fullPath)
            {
                // Generate full path selector
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        const path = [];
                        let current = element;
                        while (current && current.nodeType === Node.ELEMENT_NODE) {
                            let selector = current.nodeName.toLowerCase();
                            if (current.id) {
                                selector += '#' + current.id;
                                path.unshift(selector);
                                break;
                            } else {
                                let sibling = current.previousElementSibling;
                                let index = 1;
                                while (sibling) {
                                    if (sibling.nodeName === current.nodeName) index++;
                                    sibling = sibling.previousElementSibling;
                                }
                                if (index > 1) selector += ':nth-of-type(' + index + ')';
                                path.unshift(selector);
                                current = current.parentElement;
                            }
                        }
                        return path.join(' > ');
                    }
                ").ConfigureAwait(false);
            }
            else if (unique)
            {
                // Generate unique selector using multiple attributes
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        // Try ID first (most unique)
                        if (element.id) return '#' + element.id;
                        
                        // Try combination of tag + classes + attributes
                        let parts = [element.tagName.toLowerCase()];
                        
                        // Add classes
                        if (element.className && typeof element.className === 'string') {
                            const classes = element.className.trim().split(/\s+/);
                            parts.push('.' + classes.join('.'));
                        }
                        
                        // Add unique attributes
                        const uniqueAttrs = ['name', 'data-testid', 'data-id', 'aria-label'];
                        for (const attr of uniqueAttrs) {
                            const value = element.getAttribute(attr);
                            if (value) {
                                parts.push('[' + attr + '=""' + value + '""]');
                                break;
                            }
                        }
                        
                        return parts.join('');
                    }
                ").ConfigureAwait(false);
            }
            else if (shortest)
            {
                // Generate shortest selector
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        // Try ID first (shortest if exists)
                        if (element.id) return '#' + element.id;
                        
                        // Try single class that's unique
                        if (element.className && typeof element.className === 'string') {
                            const classes = element.className.trim().split(/\s+/);
                            for (const cls of classes) {
                                const selector = '.' + cls;
                                if (document.querySelectorAll(selector).length === 1) {
                                    return selector;
                                }
                            }
                        }
                        
                        // Try tag name if unique
                        const tagSelector = element.tagName.toLowerCase();
                        if (document.querySelectorAll(tagSelector).length === 1) {
                            return tagSelector;
                        }
                        
                        // Fall back to tag + first class
                        let result = tagSelector;
                        if (element.className && typeof element.className === 'string') {
                            const firstClass = element.className.trim().split(/\s+/)[0];
                            if (firstClass) result += '.' + firstClass;
                        }
                        return result;
                    }
                ").ConfigureAwait(false);
            }
            else
            {
                // Default: Generate a reasonable selector
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        // Use ID if available
                        if (element.id) return '#' + element.id;
                        
                        // Use tag + classes
                        let result = element.tagName.toLowerCase();
                        if (element.className && typeof element.className === 'string') {
                            const classes = element.className.trim().split(/\s+/);
                            if (classes.length > 0) {
                                result += '.' + classes.join('.');
                            }
                        }
                        
                        return result;
                    }
                ").ConfigureAwait(false);
            }
        }

        public async Task<string> GetSimilarElementsSelectorAsync(bool sameTag = false, bool sameClass = false)
        {
            if (sameTag && !sameClass)
            {
                // Return selector that matches all elements with same tag
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        return element.tagName.toLowerCase();
                    }
                ").ConfigureAwait(false);
            }
            else if (sameClass && !sameTag)
            {
                // Return selector that matches all elements with same classes
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        if (element.className && typeof element.className === 'string') {
                            const classes = element.className.trim().split(/\s+/);
                            if (classes.length > 0) {
                                return '.' + classes.join('.');
                            }
                        }
                        return element.tagName.toLowerCase(); // Fallback to tag if no classes
                    }
                ").ConfigureAwait(false);
            }
            else
            {
                // Default: Return selector for elements at same hierarchical level with same structure
                return await _element.Element.EvaluateFunctionAsync<string>(@"
                    (element) => {
                        // Build a path from the element up to a meaningful parent container
                        function buildHierarchicalSelector(el) {
                            const path = [];
                            let current = el;
                            let foundContainer = false;
                            
                            // Build path up to a meaningful container or root
                            while (current && current.nodeType === Node.ELEMENT_NODE && !foundContainer) {
                                let selector = current.nodeName.toLowerCase();
                                
                                // Add ID if present (creates a good stopping point)
                                if (current.id) {
                                    selector += '#' + current.id;
                                    path.unshift(selector);
                                    foundContainer = true;
                                    break;
                                }
                                
                                // Add classes for specificity
                                if (current.className && typeof current.className === 'string') {
                                    const classes = current.className.trim().split(/\s+/).filter(c => c.length > 0);
                                    if (classes.length > 0) {
                                        selector += '.' + classes.join('.');
                                    }
                                }
                                
                                // Check if this looks like a container element
                                const containerClasses = ['container', 'wrapper', 'content', 'main', 'section', 'list', 'grid', 'items'];
                                const hasContainerClass = current.className && containerClasses.some(cls => 
                                    current.className.includes(cls)
                                );
                                
                                path.unshift(selector);
                                
                                // Stop at container-like elements or after reasonable depth
                                if (hasContainerClass || path.length >= 3) {
                                    foundContainer = true;
                                    break;
                                }
                                
                                current = current.parentElement;
                            }
                            
                            return path.join(' > ');
                        }
                        
                        // Get the hierarchical selector
                        const hierarchicalPath = buildHierarchicalSelector(element);
                        
                        // Remove any :nth-child() or :nth-of-type() to make it match siblings
                        return hierarchicalPath.replace(/:nth-child\([^)]+\)|:nth-of-type\([^)]+\)/g, '');
                    }
                ").ConfigureAwait(false);
            }
        }

        public async Task<int> CountElementsBySelectorAsync(string selector)
        {
            return await _element.Page.EvaluateFunctionAsync<int>(@"
                (selector) => {
                    return document.querySelectorAll(selector).length;
                }", selector
            ).ConfigureAwait(false);
        }
    }
}