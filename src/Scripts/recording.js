(function() {
    if (window.__pup_recording_active) return;
    window.__pup_recording_active = true;

    const PREFIX = '__PUP_RECORDING__:';
    // Placeholders replaced by C# at runtime
    const options = { includeScroll: "{{INCLUDE_SCROLL}}" === "true", includeHover: "{{INCLUDE_HOVER}}" === "true" };

    // Simple escape for attribute values (just escape quotes)
    function escapeAttr(s) { return s.split('"').join('\\"'); }

    function generateSelector(el) {
        if (!el || el.nodeType !== 1) return null;
        // Use attribute selector for IDs to avoid CSS.escape issues with numeric IDs
        if (el.id) return '[id="' + escapeAttr(el.id) + '"]';

        const path = [];
        let current = el;
        while (current && current.nodeType === 1) {
            let sel = current.tagName.toLowerCase();
            // Use attribute selector for IDs
            if (current.id) {
                path.unshift('[id="' + escapeAttr(current.id) + '"]');
                break;
            }

            const classes = current.className && typeof current.className === 'string'
                ? current.className.split(/\s+/).filter(c => c && !c.startsWith('hover') && !c.startsWith('active') && !c.startsWith('focus'))
                : [];
            if (classes.length) {
                sel += '.' + classes[0];
            } else {
                const siblings = [...(current.parentElement?.children || [])];
                const sameTags = siblings.filter(s => s.tagName === current.tagName);
                if (sameTags.length > 1) sel += ':nth-of-type(' + (sameTags.indexOf(current) + 1) + ')';
            }
            path.unshift(sel);
            if (current.tagName === 'BODY') break;
            current = current.parentElement;
        }
        return path.join(' > ');
    }

    // Deduplication - prevent emitting same event twice in quick succession
    let lastEmit = null;
    let lastEmitTime = 0;
    function emit(data) {
        const now = Date.now();
        const key = data.type + '|' + (data.selector || '') + '|' + (data.value || '') + '|' + (data.key || '');

        // Skip if same event within 100ms (likely duplicate)
        if (lastEmit === key && now - lastEmitTime < 100) {
            return;
        }

        lastEmit = key;
        lastEmitTime = now;
        data.timestamp = new Date().toISOString();
        console.log(PREFIX + JSON.stringify(data));
    }

    // Click
    document.addEventListener('click', e => {
        emit({ type: 'click', selector: generateSelector(e.target), clickCount: e.detail, tagName: e.target.tagName });
    }, true);

    // Change (checkboxes, selects, etc.)
    document.addEventListener('change', e => {
        const t = e.target, type = (t.type || '').toLowerCase() || t.tagName.toLowerCase();
        let value = t.value;
        if (type === 'checkbox' || type === 'radio') value = String(t.checked);
        else if (t.tagName === 'SELECT') value = [...t.selectedOptions].map(o => o.value).join(',');
        emit({ type: 'change', selector: generateSelector(t), value: value, inputType: type });
    }, true);

    // Input (text fields) - debounced
    let inputTimeout = null;
    let lastInputTarget = null;
    let lastInputValue = null;
    document.addEventListener('input', e => {
        const t = e.target;
        if (['checkbox','radio'].includes(t.type) || t.tagName === 'SELECT') return;

        lastInputTarget = t;
        lastInputValue = t.value;
        clearTimeout(inputTimeout);
        inputTimeout = setTimeout(() => {
            if (lastInputTarget && lastInputValue !== null) {
                emit({ type: 'input', selector: generateSelector(lastInputTarget), value: lastInputValue, inputType: lastInputTarget.type || 'text' });
                lastInputTarget = null;
                lastInputValue = null;
            }
        }, 500);
    }, true);

    // Special keys (Enter, Tab, etc.)
    document.addEventListener('keydown', e => {
        const special = ['Enter','Tab','Escape','Backspace','Delete','ArrowUp','ArrowDown','ArrowLeft','ArrowRight'];
        if (special.includes(e.key) || e.ctrlKey || e.metaKey) {
            const mods = [];
            if (e.ctrlKey) mods.push('Control');
            if (e.shiftKey) mods.push('Shift');
            if (e.altKey) mods.push('Alt');
            if (e.metaKey) mods.push('Meta');
            emit({ type: 'keydown', key: e.key, modifiers: mods.length ? mods : null, selector: generateSelector(e.target) });
        }
    }, true);

    // Scroll (debounced, optional)
    if (options.includeScroll) {
        let scrollTimeout = null;
        let lastScrollData = null;

        document.addEventListener('scroll', e => {
            const target = e.target;
            const isWindow = target === document || target === document.documentElement;

            lastScrollData = {
                type: 'scroll',
                scrollX: isWindow ? window.scrollX : target.scrollLeft,
                scrollY: isWindow ? window.scrollY : target.scrollTop,
                selector: isWindow ? null : generateSelector(target)
            };

            clearTimeout(scrollTimeout);
            scrollTimeout = setTimeout(() => {
                if (lastScrollData) {
                    emit(lastScrollData);
                    lastScrollData = null;
                }
            }, 200);
        }, true);
    }

    // Navigation tracking
    const origPush = history.pushState, origReplace = history.replaceState;
    let inPopState = false;  // Flag to suppress navigate during back/forward
    let historyUrls = [location.href];
    let historyIndex = 0;

    // Hook pushState - emit navigate only if not triggered by back/forward
    history.pushState = function() {
        origPush.apply(this, arguments);
        if (!inPopState) {
            historyIndex++;
            historyUrls = historyUrls.slice(0, historyIndex);
            historyUrls.push(location.href);
            emit({ type: 'navigate', url: location.href });
        }
    };

    // Don't emit for replaceState - it updates current entry, not a navigation
    history.replaceState = function() {
        origReplace.apply(this, arguments);
    };

    // Handle back/forward (SPA)
    window.addEventListener('popstate', () => {
        inPopState = true;
        const currentUrl = location.href;
        const prevIndex = historyUrls.indexOf(currentUrl);
        if (prevIndex !== -1 && prevIndex < historyIndex) {
            emit({ type: 'back', url: currentUrl });
            historyIndex = prevIndex;
        } else if (prevIndex !== -1 && prevIndex > historyIndex) {
            emit({ type: 'forward', url: currentUrl });
            historyIndex = prevIndex;
        } else {
            // URL not in our history - assume back
            emit({ type: 'back', url: currentUrl });
        }
        // Reset flag after a short delay to allow framework handlers to run
        setTimeout(() => { inPopState = false; }, 100);
    });

    // Detect back/forward for traditional page navigation (non-SPA)
    (function detectBackForward() {
        let navType = null;
        if (performance && performance.getEntriesByType) {
            const navEntries = performance.getEntriesByType('navigation');
            if (navEntries.length > 0) navType = navEntries[0].type;
        }
        if (!navType && performance && performance.navigation) {
            navType = performance.navigation.type === 2 ? 'back_forward' : null;
        }
        if (navType === 'back_forward') {
            emit({ type: 'back', url: location.href });
        }
    })();

    // Detect bfcache restoration
    window.addEventListener('pageshow', (e) => {
        if (e.persisted) {
            emit({ type: 'back', url: location.href });
        }
    });

    window.__pup_recording_cleanup = () => {
        window.__pup_recording_active = false;
        history.pushState = origPush;
        history.replaceState = origReplace;
        clearTimeout(inputTimeout);
    };
})();
