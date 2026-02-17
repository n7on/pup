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

    // Track Enter keydown to suppress synthetic clicks
    let lastEnterTarget = null;
    let lastEnterTime = 0;

    // Click - skip synthetic clicks caused by Enter on buttons/links
    document.addEventListener('click', e => {
        if (lastEnterTarget && Date.now() - lastEnterTime < 50 && (lastEnterTarget === e.target || lastEnterTarget.contains(e.target))) {
            lastEnterTarget = null;
            return;
        }
        emit({ type: 'click', selector: generateSelector(e.target), clickCount: e.detail, tagName: e.target.tagName });
    }, true);

    // Change - only for checkboxes, radios, selects (text inputs are handled by input tracking)
    document.addEventListener('change', e => {
        const t = e.target, type = (t.type || '').toLowerCase();
        if (type === 'checkbox' || type === 'radio' || t.tagName === 'SELECT') {
            let value = t.value;
            if (type === 'checkbox' || type === 'radio') value = String(t.checked);
            else if (t.tagName === 'SELECT') value = [...t.selectedOptions].map(o => o.value).join(',');
            emit({ type: 'change', selector: generateSelector(t), value: value, inputType: type });
        }
    }, true);

    // Input - track pending value, only emit when field loses focus or an action key is pressed
    let pendingInput = null;
    function flushPendingInput() {
        if (pendingInput) {
            emit({ type: 'input', selector: generateSelector(pendingInput.target), value: pendingInput.value, inputType: pendingInput.target.type || 'text' });
            pendingInput = null;
        }
    }

    document.addEventListener('input', e => {
        const t = e.target;
        if (['checkbox','radio'].includes(t.type) || t.tagName === 'SELECT') return;
        pendingInput = { target: t, value: t.value };
    }, true);

    // Flush pending input when field loses focus (click away, tab out, etc.)
    document.addEventListener('focusout', e => {
        if (pendingInput && pendingInput.target === e.target) {
            flushPendingInput();
        }
    }, true);

    // Special keys - flush pending input before action keys, skip bare modifier keys
    document.addEventListener('keydown', e => {
        // Skip bare modifier keys (not useful for replay)
        if (['Control','Shift','Alt','Meta'].includes(e.key)) return;

        const actionKeys = ['Enter','Tab','Escape'];
        const isActionKey = actionKeys.includes(e.key);
        // Ctrl combos but not AltGr (Ctrl+Alt on Windows, used for characters like @)
        const isModifierCombo = ((e.ctrlKey && !e.altKey) || e.metaKey);

        if (isActionKey || isModifierCombo) {
            // Flush pending input before action keys so the input event comes first
            if (isActionKey) {
                flushPendingInput();
            }

            // Track Enter to suppress the synthetic click that follows
            if (e.key === 'Enter') {
                lastEnterTarget = e.target;
                lastEnterTime = Date.now();
            }

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

    window.__pup_flush_pending_input = flushPendingInput;

    window.__pup_recording_cleanup = () => {
        flushPendingInput();
        window.__pup_recording_active = false;
        history.pushState = origPush;
        history.replaceState = origReplace;
    };
})();
