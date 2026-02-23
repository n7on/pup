// Stealth script to hide automation indicators

// Override webdriver on the prototype to match a real non-automated Chrome.
// Real Chrome defines it on Navigator.prototype as an accessor returning false.
// Overriding on the instance instead of the prototype is detectable.
Object.defineProperty(Navigator.prototype, 'webdriver', {
    get: () => false,
    enumerable: true,
    configurable: true
});

// Mock plugins to look like a real browser
Object.defineProperty(navigator, 'plugins', {
    get: () => {
        const plugins = [
            { name: 'Chrome PDF Plugin', filename: 'internal-pdf-viewer', description: 'Portable Document Format' },
            { name: 'Chrome PDF Viewer', filename: 'mhjfbmdgcfjbbpaeojofohoefgiehjai', description: '' },
            { name: 'Native Client', filename: 'internal-nacl-plugin', description: '' }
        ];
        plugins.item = (i) => plugins[i] || null;
        plugins.namedItem = (name) => plugins.find(p => p.name === name) || null;
        plugins.refresh = () => {};
        return plugins;
    },
    configurable: true
});

// Mock languages
Object.defineProperty(navigator, 'languages', {
    get: () => ['en-US', 'en'],
    configurable: true
});

// Fix permissions API
const originalQuery = window.navigator.permissions?.query;
if (originalQuery) {
    window.navigator.permissions.query = (parameters) => (
        parameters.name === 'notifications' ?
            Promise.resolve({ state: Notification.permission }) :
            originalQuery(parameters)
    );
}

// Override userAgentData to hide HeadlessChrome
if (navigator.userAgentData) {
    const origUAData = navigator.userAgentData;
    const patchBrands = (brands) => brands.map(b => ({
        ...b,
        brand: b.brand === 'HeadlessChrome' ? 'Google Chrome' : b.brand
    }));
    const patchedBrands = patchBrands(origUAData.brands);
    const origGetHighEntropyValues = origUAData.getHighEntropyValues.bind(origUAData);

    Object.defineProperty(navigator, 'userAgentData', {
        get: () => ({
            brands: patchedBrands,
            mobile: origUAData.mobile,
            platform: origUAData.platform,
            getHighEntropyValues: (hints) => origGetHighEntropyValues(hints).then(data => ({
                ...data,
                brands: patchBrands(data.brands),
                fullVersionList: data.fullVersionList ? patchBrands(data.fullVersionList) : undefined
            })),
            toJSON: () => ({
                brands: patchedBrands,
                mobile: origUAData.mobile,
                platform: origUAData.platform
            })
        }),
        configurable: true
    });
}

// Mock chrome object to look like a real browser
if (!window.chrome) {
    window.chrome = {};
}
if (!window.chrome.runtime) {
    window.chrome.runtime = {
        connect: () => {},
        sendMessage: () => {},
        onMessage: { addListener: () => {} }
    };
}
if (!window.chrome.app) {
    window.chrome.app = {
        isInstalled: false,
        InstallState: { DISABLED: 'disabled', INSTALLED: 'installed', NOT_INSTALLED: 'not_installed' },
        RunningState: { CANNOT_RUN: 'cannot_run', READY_TO_RUN: 'ready_to_run', RUNNING: 'running' },
        getDetails: function() { return null; },
        getIsInstalled: function() { return false; },
        installState: function(cb) { if (cb) cb('not_installed'); },
        runningState: function() { return 'cannot_run'; }
    };
}
if (!window.chrome.csi) {
    window.chrome.csi = function() {
        return {
            onloadT: Date.now(),
            startE: Date.now(),
            pageT: performance.now(),
            tpiT: 0
        };
    };
}
if (!window.chrome.loadTimes) {
    window.chrome.loadTimes = function() {
        const now = Date.now() / 1000;
        return {
            commitLoadTime: now,
            connectionInfo: 'http/1.1',
            finishDocumentLoadTime: now,
            finishLoadTime: now,
            firstPaintAfterLoadTime: 0,
            firstPaintTime: now,
            navigationType: 'Other',
            npnNegotiatedProtocol: 'unknown',
            requestTime: now,
            startLoadTime: now,
            wasAlternateProtocolAvailable: false,
            wasFetchedViaSpdy: false,
            wasNpnNegotiated: false
        };
    };
}

// Fix iframe contentWindow access detection
const originalContentWindow = Object.getOwnPropertyDescriptor(HTMLIFrameElement.prototype, 'contentWindow');
Object.defineProperty(HTMLIFrameElement.prototype, 'contentWindow', {
    get: function() {
        const win = originalContentWindow.get.call(this);
        if (win) {
            // Make sure nested iframes also don't expose webdriver
            try {
                Object.defineProperty(Object.getPrototypeOf(win.navigator), 'webdriver', {
                    get: () => false,
                    enumerable: true,
                    configurable: true
                });
            } catch {
                // Ignore - cross-origin iframes will throw
            }
        }
        return win;
    }
});

// Patch toString to hide native code modifications
const originalToString = Function.prototype.toString;
const patchedGetters = new Set([
    Object.getOwnPropertyDescriptor(Navigator.prototype, 'webdriver')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'plugins')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'languages')?.get
].filter(Boolean));
Function.prototype.toString = function() {
    if (patchedGetters.has(this)) {
        return 'function get () { [native code] }';
    }
    return originalToString.call(this);
};
