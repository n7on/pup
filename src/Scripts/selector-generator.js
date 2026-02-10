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
}
