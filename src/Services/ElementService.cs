using Pup.Transport;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pup.Services
{
    public interface IElementService
    {
        Task ClickElementAsync();
        Task SetElementTextAsync(string text);
        Task SetElementValueAsync(string value);
        
        Task<string> GetElementAttributeAsync(string attributeName);
        Task<string> GetElementTextAsync();
        Task<object> GetElementValueAsync();
        
        Task HoverElementAsync();
        Task FocusElementAsync();
        Task<bool> IsElementVisibleAsync();
        Task<bool> IsElementEnabledAsync();
        Task SetElementFormValueAsync(object value, bool triggerChange = true);
        
        Task<PupElement> FindElementBySelectorAsync(string selector);
        Task<List<PupElement>> FindElementsBySelectorAsync(string selector);
        Task<PupElement> FindElementByXPathAsync(string xpath);
        Task<List<PupElement>> FindElementsByXPathAsync(string xpath);
        
        Task<string> GetElementSelectorAsync(bool unique = false, bool shortest = false, bool fullPath = false);
        Task<string> GetSimilarElementsSelectorAsync(bool sameTag = false, bool sameClass = false);
        Task<int> CountElementsBySelectorAsync(string selector);

        // Select/dropdown operations
        Task<string[]> SelectOptionByValueAsync(params string[] values);
        Task<string[]> SelectOptionByTextAsync(params string[] texts);
        Task<string[]> SelectOptionByIndexAsync(params int[] indices);
        Task<List<PupSelectOption>> GetSelectOptionsAsync();

        // Screenshot
        Task<byte[]> GetScreenshotAsync(string filePath = null);
    }

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
            await _element.Element.EvaluateFunctionAsync("(el, val) => el.value = val", value).ConfigureAwait(false);
        }

        public async Task SetElementFormValueAsync(object value, bool triggerChange = true)
        {
            await _element.Element.EvaluateFunctionAsync(@"(el, val, fire) => {
                const dispatch = (type) => el.dispatchEvent(new Event(type, { bubbles: true }));
                const tag = (el.tagName || '').toLowerCase();
                const type = (el.type || '').toLowerCase();

                const fireEvents = () => { if (fire) { dispatch('input'); dispatch('change'); } };

                if (tag === 'input' && (type === 'checkbox' || type === 'radio')) {
                    const desired = !!val;
                    if (el.checked !== desired) {
                        el.checked = desired;
                        fireEvents();
                    }
                    return;
                }

                if (tag === 'select') {
                    const values = Array.isArray(val) ? val.map(v => v != null ? String(v) : '') : [val != null ? String(val) : ''];
                    let changed = false;
                    for (const option of el.options) {
                        const shouldSelect = values.includes(option.value) || values.includes(option.text);
                        if (option.selected !== shouldSelect) {
                            option.selected = shouldSelect;
                            changed = true;
                        }
                    }
                    if (changed) fireEvents();
                    return;
                }

                const newValue = val == null ? '' : String(val);
                if (el.value !== newValue) {
                    el.value = newValue;
                    fireEvents();
                }
            }", value, triggerChange).ConfigureAwait(false);
        }

        public async Task<string> GetElementAttributeAsync(string attributeName)
        {
            return await _element.Element.EvaluateFunctionAsync<string>("(el, attr) => el.getAttribute(attr)", attributeName).ConfigureAwait(false);
        }

        public async Task<string> GetElementTextAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<string>("el => el.textContent").ConfigureAwait(false);
        }

        public async Task<object> GetElementValueAsync()
        {
            var result = await _element.Element.EvaluateFunctionAsync<JsonElement>(@"
                (el) => {
                    const tag = (el.tagName || '').toLowerCase();
                    const type = (el.type || '').toLowerCase();

                    if (tag === 'input' && (type === 'checkbox' || type === 'radio')) {
                        return { kind: 'bool', value: el.checked };
                    }

                    if (tag === 'select' && el.multiple) {
                        return { kind: 'array', value: Array.from(el.selectedOptions).map(o => o.value) };
                    }

                    return { kind: 'string', value: el.value };
                }
            ").ConfigureAwait(false);

            if (!result.TryGetProperty("kind", out var kindProp) || !result.TryGetProperty("value", out var valueProp))
            {
                return null;
            }

            switch (kindProp.GetString())
            {
                case "bool":
                    return valueProp.GetBoolean();
                case "array":
                    var values = new List<string>();
                    foreach (var item in valueProp.EnumerateArray())
                    {
                        values.Add(item.GetString() ?? string.Empty);
                    }
                    return values.ToArray();
                default:
                    return valueProp.GetString();
            }
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

        public async Task<string[]> SelectOptionByValueAsync(params string[] values)
        {
            return await _element.Element.SelectAsync(values).ConfigureAwait(false);
        }

        public async Task<string[]> SelectOptionByTextAsync(params string[] texts)
        {
            // Get values for options matching the given text labels
            var values = await _element.Element.EvaluateFunctionAsync<string[]>(@"
                (el, texts) => {
                    const options = Array.from(el.options);
                    const matchingValues = [];
                    for (const text of texts) {
                        const option = options.find(o => o.text === text || o.textContent.trim() === text);
                        if (option) {
                            matchingValues.push(option.value);
                        }
                    }
                    return matchingValues;
                }", (object)texts
            ).ConfigureAwait(false);

            if (values.Length == 0)
            {
                return values;
            }

            return await _element.Element.SelectAsync(values).ConfigureAwait(false);
        }

        public async Task<string[]> SelectOptionByIndexAsync(params int[] indices)
        {
            // Get values for options at the given indices
            var values = await _element.Element.EvaluateFunctionAsync<string[]>(@"
                (el, indices) => {
                    const options = Array.from(el.options);
                    const matchingValues = [];
                    for (const index of indices) {
                        if (index >= 0 && index < options.length) {
                            matchingValues.push(options[index].value);
                        }
                    }
                    return matchingValues;
                }", (object)indices
            ).ConfigureAwait(false);

            if (values.Length == 0)
            {
                return values;
            }

            return await _element.Element.SelectAsync(values).ConfigureAwait(false);
        }

        public async Task<List<PupSelectOption>> GetSelectOptionsAsync()
        {
            var optionsData = await _element.Element.EvaluateFunctionAsync<JsonElement>(@"
                (el) => {
                    return Array.from(el.options).map((o, i) => ({
                        value: o.value,
                        text: o.text,
                        index: i,
                        selected: o.selected,
                        disabled: o.disabled
                    }));
                }"
            ).ConfigureAwait(false);

            var options = new List<PupSelectOption>();
            foreach (var item in optionsData.EnumerateArray())
            {
                options.Add(new PupSelectOption(
                    value: item.GetProperty("value").GetString() ?? "",
                    text: item.GetProperty("text").GetString() ?? "",
                    index: item.GetProperty("index").GetInt32(),
                    selected: item.GetProperty("selected").GetBoolean(),
                    disabled: item.GetProperty("disabled").GetBoolean()
                ));
            }

            return options;
        }

        public async Task<byte[]> GetScreenshotAsync(string filePath = null)
        {
            var options = new ElementScreenshotOptions
            {
                Type = ScreenshotType.Png
            };

            if (!string.IsNullOrEmpty(filePath))
            {
                await _element.Element.ScreenshotAsync(filePath, options).ConfigureAwait(false);
            }

            return await _element.Element.ScreenshotDataAsync(options).ConfigureAwait(false);
        }
    }
}
