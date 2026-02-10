// Stealth script to hide automation indicators

// Remove webdriver property
Object.defineProperty(navigator, 'webdriver', {
    get: () => undefined,
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

// Mock chrome runtime to look like a real browser
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

// Remove automation-related properties from navigator
const automationProps = ['__webdriver_evaluate', '__selenium_evaluate', '__webdriver_script_function',
    '__webdriver_script_func', '__webdriver_script_fn', '__fxdriver_evaluate', '__driver_unwrapped',
    '__webdriver_unwrapped', '__driver_evaluate', '__selenium_unwrapped', '__fxdriver_unwrapped'];
automationProps.forEach(prop => {
    delete navigator[prop];
    delete window[prop];
});

// Fix iframe contentWindow access detection
const originalContentWindow = Object.getOwnPropertyDescriptor(HTMLIFrameElement.prototype, 'contentWindow');
Object.defineProperty(HTMLIFrameElement.prototype, 'contentWindow', {
    get: function() {
        const win = originalContentWindow.get.call(this);
        if (win) {
            // Make sure nested iframes also don't expose webdriver
            try {
                Object.defineProperty(win.navigator, 'webdriver', {
                    get: () => undefined,
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
Function.prototype.toString = function() {
    if (this === navigator.webdriver?.get ||
        this === navigator.plugins?.get ||
        this === navigator.languages?.get) {
        return 'function get webdriver() { [native code] }';
    }
    return originalToString.call(this);
};
