// Stealth script to hide automation indicators

// Override webdriver on the prototype to match a real non-automated Chrome.
// Real Chrome defines it on Navigator.prototype as an accessor returning false.
// Overriding on the instance instead of the prototype is detectable.
Object.defineProperty(Navigator.prototype, 'webdriver', {
    get: () => false,
    enumerable: true,
    configurable: true
});

// Override navigator.platform to match the user agent
const _isMac = navigator.userAgent.includes('Macintosh');
const _isLinux = navigator.userAgent.includes('Linux');
const _uaPlatform = _isMac ? 'MacIntel' : _isLinux ? 'Linux x86_64' : 'Win32';
Object.defineProperty(Navigator.prototype, 'platform', {
    get: () => _uaPlatform,
    enumerable: true,
    configurable: true
});

// Override WebGL renderer only when SwiftShader is detected (headless software
// renderer). When a real GPU is available, let it report its actual identity
// so that the rendering hashes match the claimed GPU.
{
    let _needsWebglOverride = false;
    try {
        const _c = document.createElement('canvas');
        const _gl = _c.getContext('webgl');
        if (_gl) {
            const _ext = _gl.getExtension('WEBGL_debug_renderer_info');
            if (_ext) {
                _needsWebglOverride = _gl.getParameter(_ext.UNMASKED_RENDERER_WEBGL).includes('SwiftShader');
            }
        }
    } catch {}
    if (_needsWebglOverride) {
        const _webglRendererOverrides = _isLinux
            ? { vendor: 'Google Inc. (Intel)', renderer: 'ANGLE (Intel, Mesa Intel(R) UHD Graphics 630 (CFL GT2), OpenGL 4.5)' }
            : _isMac
            ? { vendor: 'Google Inc. (Apple)', renderer: 'ANGLE (Apple, Apple M1, OpenGL 4.1)' }
            : { vendor: 'Google Inc. (NVIDIA)', renderer: 'ANGLE (NVIDIA, NVIDIA GeForce GTX 1650 Direct3D11 vs_5_0 ps_5_0, D3D11)' };
        const _webglGetParameterHandler = function(origFn) {
            return function(param) {
                if (param === 37445) return _webglRendererOverrides.vendor;   // UNMASKED_VENDOR_WEBGL
                if (param === 37446) return _webglRendererOverrides.renderer; // UNMASKED_RENDERER_WEBGL
                return origFn.call(this, param);
            };
        };
        if (typeof WebGLRenderingContext !== 'undefined') {
            WebGLRenderingContext.prototype.getParameter = _webglGetParameterHandler(WebGLRenderingContext.prototype.getParameter);
        }
        if (typeof WebGL2RenderingContext !== 'undefined') {
            WebGL2RenderingContext.prototype.getParameter = _webglGetParameterHandler(WebGL2RenderingContext.prototype.getParameter);
        }
    }
}

// Fix devicePixelRatio for macOS (all modern Macs have Retina displays)
if (_isMac && window.devicePixelRatio === 1) {
    Object.defineProperty(window, 'devicePixelRatio', { get: () => 2, configurable: true });
}

// Fix screen and window dimensions for headless consistency.
// outerWidth/outerHeight should be >= innerWidth/innerHeight in a real browser.
if (window.outerWidth < window.innerWidth) {
    Object.defineProperty(window, 'outerWidth', { get: () => window.innerWidth, configurable: true });
}
if (window.outerHeight < window.innerHeight) {
    Object.defineProperty(window, 'outerHeight', { get: () => window.innerHeight + 85, configurable: true });
}
// Real displays are always larger than 1024px wide. In headless mode
// screen dimensions default to the window size (e.g. 800x600), which is a giveaway.
const _screenW = _isMac ? 1440 : 1920;
const _screenH = _isMac ? 900 : 1080;
if (screen.width < 1024) {
    Object.defineProperty(screen, 'width', { get: () => _screenW, configurable: true });
    Object.defineProperty(screen, 'availWidth', { get: () => _screenW, configurable: true });
    Object.defineProperty(screen, 'height', { get: () => _screenH, configurable: true });
    Object.defineProperty(screen, 'availHeight', { get: () => _screenH - 40, configurable: true });
}
// macOS with Apple displays uses 30-bit color (P3), Windows/Linux use 24-bit
const _colorDepth = _isMac ? 30 : 24;
Object.defineProperty(screen, 'colorDepth', { get: () => _colorDepth, configurable: true });
Object.defineProperty(screen, 'pixelDepth', { get: () => _colorDepth, configurable: true });

// Mock navigator.connection (Network Information API)
if (!navigator.connection) {
    Object.defineProperty(navigator, 'connection', {
        get: () => ({
            effectiveType: '4g',
            rtt: 50,
            downlink: 10,
            saveData: false,
            onchange: null
        }),
        configurable: true
    });
}

// Mock plugins to match modern Chrome (91+)
Object.defineProperty(navigator, 'plugins', {
    get: () => {
        const plugins = [
            { name: 'PDF Viewer', filename: 'internal-pdf-viewer', description: 'Portable Document Format', length: 2 },
            { name: 'Chrome PDF Viewer', filename: 'internal-pdf-viewer', description: 'Portable Document Format', length: 2 },
            { name: 'Chromium PDF Viewer', filename: 'internal-pdf-viewer', description: 'Portable Document Format', length: 2 },
            { name: 'Microsoft Edge PDF Viewer', filename: 'internal-pdf-viewer', description: 'Portable Document Format', length: 2 },
            { name: 'WebKit built-in PDF', filename: 'internal-pdf-viewer', description: 'Portable Document Format', length: 2 }
        ];
        plugins.item = (i) => plugins[i] || null;
        plugins.namedItem = (name) => plugins.find(p => p.name === name) || null;
        plugins.refresh = () => {};
        return plugins;
    },
    configurable: true
});

// Mock mimeTypes to match plugins (PDF viewer)
Object.defineProperty(navigator, 'mimeTypes', {
    get: () => {
        const mimes = [
            { type: 'application/pdf', suffixes: 'pdf', description: 'Portable Document Format', enabledPlugin: { name: 'PDF Viewer' } },
            { type: 'text/pdf', suffixes: 'pdf', description: 'Portable Document Format', enabledPlugin: { name: 'PDF Viewer' } }
        ];
        mimes.item = (i) => mimes[i] || null;
        mimes.namedItem = (type) => mimes.find(m => m.type === type) || null;
        return mimes;
    },
    configurable: true
});

// Mock languages
Object.defineProperty(navigator, 'languages', {
    get: () => ['en-US', 'en'],
    configurable: true
});

// Fix Notification.permission to return 'default' instead of 'denied' in headless
if (typeof Notification !== 'undefined' && Notification.permission === 'denied') {
    Object.defineProperty(Notification, 'permission', {
        get: () => 'default',
        configurable: true
    });
}

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
    Object.getOwnPropertyDescriptor(Navigator.prototype, 'platform')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'plugins')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'mimeTypes')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'languages')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'connection')?.get,
    Object.getOwnPropertyDescriptor(navigator, 'userAgentData')?.get
].filter(Boolean));
Function.prototype.toString = function() {
    if (patchedGetters.has(this)) {
        return 'function get () { [native code] }';
    }
    return originalToString.call(this);
};
