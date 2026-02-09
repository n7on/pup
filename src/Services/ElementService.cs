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
        Task ClickElementAsync(int clickCount = 1);
        Task ScrollIntoViewAsync();
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
        Task<List<PupElement>> FindElementsByTextAsync(string text, bool exactMatch, string selector);
        
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

        // File upload
        Task UploadFilesAsync(params string[] filePaths);

        // Pattern detection
        Task<List<PupElementPattern>> GetElementPatternsAsync(int depth = 0);
    }

    public class ElementService : IElementService
    {

        private readonly PupElement _element;

        public ElementService(PupElement element)
        {
            _element = element;
        }

        public async Task ClickElementAsync(int clickCount = 1)
        {
            if (clickCount == 1)
            {
                await _element.Element.ClickAsync().ConfigureAwait(false);
            }
            else
            {
                // Use JavaScript for double/triple click since ClickOptions.ClickCount may not be available
                await _element.Element.EvaluateFunctionAsync(@"(el, count) => {
                    const rect = el.getBoundingClientRect();
                    const x = rect.left + rect.width / 2;
                    const y = rect.top + rect.height / 2;
                    const event = new MouseEvent('dblclick', {
                        bubbles: true,
                        cancelable: true,
                        view: window,
                        detail: count,
                        clientX: x,
                        clientY: y
                    });
                    el.dispatchEvent(event);
                }", clickCount).ConfigureAwait(false);
            }
        }

        public async Task ScrollIntoViewAsync()
        {
            await _element.Element.ScrollIntoViewAsync().ConfigureAwait(false);
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

        public async Task<List<PupElement>> FindElementsByTextAsync(string text, bool exactMatch, string selector = null)
        {
            // JavaScript that finds elements by visible text within this element
            var script = @"(root, searchText, exactMatch, cssSelector) => {
                const normalize = (s) => (s || '').trim().replace(/\s+/g, ' ');
                const searchNorm = normalize(searchText);

                // Get candidate elements within root
                let candidates;
                if (cssSelector) {
                    candidates = [...root.querySelectorAll(cssSelector)];
                } else {
                    candidates = [...root.querySelectorAll('*')];
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

            var elementHandles = await _element.Element.EvaluateFunctionHandleAsync(script, text, exactMatch, selector).ConfigureAwait(false);
            var jsHandle = elementHandles as IJSHandle;

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
                    // Generate a stable CSS selector for this element (relative to root element)
                    var elementSelector = await element.EvaluateFunctionAsync<string>(GenerateUniqueSelectorScript, _element.Element).ConfigureAwait(false);

                    pbElements.Add(new PupElement(
                        element,
                        _element.Page,
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
            (element, rootElement) => {
                // Try ID first (most stable) - but only if not searching within a root
                if (!rootElement && element.id) return '#' + CSS.escape(element.id);

                // Build path from element up to root (or document)
                const path = [];
                let current = element;

                while (current && current.nodeType === Node.ELEMENT_NODE) {
                    // Stop if we've reached the root element (for relative selectors)
                    if (rootElement && current === rootElement) break;

                    let selector = current.tagName.toLowerCase();

                    // If element has ID and we're not doing relative selector, use it and stop
                    if (!rootElement && current.id) {
                        selector = '#' + CSS.escape(current.id);
                        path.unshift(selector);
                        break;
                    }

                    // Add class if available (more readable than nth-of-type)
                    const classes = current.className && typeof current.className === 'string'
                        ? current.className.trim().split(/\s+/).filter(c => c.length > 0)
                        : [];
                    if (classes.length > 0) {
                        selector += '.' + CSS.escape(classes[0]);
                    } else {
                        // Add nth-of-type for uniqueness among siblings if no class
                        const parent = current.parentElement;
                        if (parent && (!rootElement || parent !== rootElement)) {
                            const siblings = [...parent.children];
                            const sameTagSiblings = siblings.filter(s => s.tagName === current.tagName);
                            if (sameTagSiblings.length > 1) {
                                const index = sameTagSiblings.indexOf(current) + 1;
                                selector += ':nth-of-type(' + index + ')';
                            }
                        }
                    }

                    path.unshift(selector);

                    // Stop at body (for absolute selectors)
                    if (!rootElement && current.tagName.toLowerCase() === 'body') break;

                    current = current.parentElement;
                }

                return path.join(' > ');
            }";

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

        public async Task UploadFilesAsync(params string[] filePaths)
        {
            await _element.Element.UploadFileAsync(filePaths).ConfigureAwait(false);
        }

        public async Task<List<PupElementPattern>> GetElementPatternsAsync(int depth = 0)
        {
            var script = @"
                (element, depth) => {
                    // Traverse up to the specified depth
                    let target = element;
                    for (let i = 0; i < depth && target.parentElement; i++) {
                        target = target.parentElement;
                    }
                    element = target;
                    const results = [];
                    const escape = (s) => s ? CSS.escape(s) : '';

                    // Helper to count matches
                    const count = (sel) => {
                        try { return document.querySelectorAll(sel).length; }
                        catch { return 0; }
                    };

                    // Helper to check if selector matches our element
                    const matchesElement = (sel) => {
                        try {
                            const matches = document.querySelectorAll(sel);
                            return Array.from(matches).includes(element);
                        } catch { return false; }
                    };

                    const tag = element.tagName.toLowerCase();
                    const classes = element.className && typeof element.className === 'string'
                        ? element.className.trim().split(/\s+/).filter(c => c.length > 0)
                        : [];

                    // 1. ByTag - just the tag name
                    const byTag = tag;
                    if (matchesElement(byTag)) {
                        results.push({
                            type: 'ByTag',
                            selector: byTag,
                            matchCount: count(byTag),
                            description: 'All ' + tag + ' elements'
                        });
                    }

                    // 2. ByClass - tag with classes
                    if (classes.length > 0) {
                        const byClass = tag + '.' + classes.map(c => escape(c)).join('.');
                        if (matchesElement(byClass)) {
                            results.push({
                                type: 'ByClass',
                                selector: byClass,
                                matchCount: count(byClass),
                                description: 'Elements with same tag and classes'
                            });
                        }
                    }

                    // 3. ByParentClass - find nearest parent with meaningful class
                    let parent = element.parentElement;
                    let parentDepth = 0;
                    while (parent && parentDepth < 5) {
                        const parentClasses = parent.className && typeof parent.className === 'string'
                            ? parent.className.trim().split(/\s+/).filter(c => c.length > 0)
                            : [];

                        if (parentClasses.length > 0) {
                            const parentSel = '.' + escape(parentClasses[0]);
                            const childSel = parentSel + ' ' + tag;
                            if (matchesElement(childSel)) {
                                results.push({
                                    type: 'ByParentClass',
                                    selector: childSel,
                                    matchCount: count(childSel),
                                    description: 'All ' + tag + ' inside .' + parentClasses[0]
                                });
                            }

                            // Also try direct child
                            const directChildSel = parentSel + ' > ' + tag;
                            if (matchesElement(directChildSel) && count(directChildSel) !== count(childSel)) {
                                results.push({
                                    type: 'ByDirectParent',
                                    selector: directChildSel,
                                    matchCount: count(directChildSel),
                                    description: 'Direct ' + tag + ' children of .' + parentClasses[0]
                                });
                            }
                            break;
                        }
                        parent = parent.parentElement;
                        parentDepth++;
                    }

                    // 4. ByAncestorId - find nearest ancestor with ID
                    let ancestor = element.parentElement;
                    let ancestorDepth = 0;
                    let pathFromAncestor = [tag];
                    while (ancestor && ancestorDepth < 10) {
                        if (ancestor.id) {
                            const byAncestorId = '#' + escape(ancestor.id) + ' ' + pathFromAncestor.join(' > ');
                            if (matchesElement(byAncestorId)) {
                                results.push({
                                    type: 'ByAncestorId',
                                    selector: byAncestorId,
                                    matchCount: count(byAncestorId),
                                    description: 'All ' + tag + ' under #' + ancestor.id
                                });
                            }
                            break;
                        }
                        pathFromAncestor.unshift(ancestor.tagName.toLowerCase());
                        ancestor = ancestor.parentElement;
                        ancestorDepth++;
                    }

                    // 5. ByStructure - find repeating container pattern
                    // Look for parent that has multiple similar children
                    let container = element.parentElement;
                    let relPath = tag;
                    let structureDepth = 0;
                    while (container && structureDepth < 6) {
                        const containerTag = container.tagName.toLowerCase();
                        const containerParent = container.parentElement;

                        if (containerParent) {
                            // Count siblings with same tag
                            const siblings = [...containerParent.children].filter(
                                c => c.tagName === container.tagName
                            );

                            if (siblings.length > 1) {
                                // Found a repeating container
                                let containerSel = containerTag;
                                const containerClasses = container.className && typeof container.className === 'string'
                                    ? container.className.trim().split(/\s+/).filter(c => c.length > 0)
                                    : [];
                                if (containerClasses.length > 0) {
                                    containerSel += '.' + escape(containerClasses[0]);
                                }

                                const structureSel = containerSel + ' ' + relPath;
                                if (matchesElement(structureSel)) {
                                    results.push({
                                        type: 'ByStructure',
                                        selector: structureSel,
                                        matchCount: count(structureSel),
                                        description: 'Elements in repeating ' + containerSel + ' containers'
                                    });
                                }
                                break;
                            }
                        }

                        relPath = containerTag + ' > ' + relPath;
                        container = containerParent;
                        structureDepth++;
                    }

                    // 6. ByRole - if element has role or aria attributes
                    const role = element.getAttribute('role');
                    if (role) {
                        const byRole = '[role=""' + role + '""]';
                        if (matchesElement(byRole)) {
                            results.push({
                                type: 'ByRole',
                                selector: byRole,
                                matchCount: count(byRole),
                                description: 'Elements with role=' + role
                            });
                        }
                    }

                    // 7. ByDataAttribute - common data-* attributes
                    const dataAttrs = [...element.attributes]
                        .filter(a => a.name.startsWith('data-') && !a.name.includes('id'))
                        .map(a => a.name);
                    if (dataAttrs.length > 0) {
                        const attr = dataAttrs[0];
                        const byData = tag + '[' + attr + ']';
                        if (matchesElement(byData)) {
                            results.push({
                                type: 'ByDataAttribute',
                                selector: byData,
                                matchCount: count(byData),
                                description: 'All ' + tag + ' with ' + attr + ' attribute'
                            });
                        }
                    }

                    // Sort by match count (prefer patterns with reasonable count)
                    results.sort((a, b) => {
                        // Prefer counts > 1 but not too high
                        const scoreA = a.matchCount > 1 && a.matchCount < 1000 ? 1000 - a.matchCount : -a.matchCount;
                        const scoreB = b.matchCount > 1 && b.matchCount < 1000 ? 1000 - b.matchCount : -b.matchCount;
                        return scoreB - scoreA;
                    });

                    return results;
                }
            ";

            var patternsJson = await _element.Element.EvaluateFunctionAsync<JsonElement>(script, depth).ConfigureAwait(false);
            var patterns = new List<PupElementPattern>();

            foreach (var item in patternsJson.EnumerateArray())
            {
                patterns.Add(new PupElementPattern
                {
                    Type = item.GetProperty("type").GetString(),
                    Selector = item.GetProperty("selector").GetString(),
                    MatchCount = item.GetProperty("matchCount").GetInt32(),
                    Description = item.GetProperty("description").GetString()
                });
            }

            return patterns;
        }
    }
}
