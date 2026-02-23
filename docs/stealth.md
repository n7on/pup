# Stealth Mode

Pup automatically applies stealth measures to every page, making the browser appear as a regular user session rather than an automated one. This is applied transparently — no configuration needed.

Stealth mode scores **100%** on [browserscan.net](https://www.browserscan.net) fingerprint authenticity checks.

## How It Works

Stealth operates at two levels:

1. **Browser launch arguments** — flags passed to Chrome at startup
2. **JavaScript injection** — a stealth script injected into every page (and every future navigation) before any site code runs
3. **CDP-level overrides** — Chrome DevTools Protocol commands that set HTTP headers the browser sends

## What Gets Patched

### User-Agent Consistency

The user-agent string is built dynamically from the actual installed Chrome version. This ensures the `User-Agent` HTTP header, `navigator.userAgent`, and `navigator.userAgentData.brands` all report the same version.

If no custom user-agent is specified, Pup constructs one matching the real browser:

```
Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{version}.0.0.0 Safari/537.36
```

You can override it with `-UserAgent` on `Start-PupBrowser`, or pass `"none"` to use the browser's native (unmodified) user-agent.

### Client Hints (sec-ch-ua)

Chrome sends `sec-ch-ua` HTTP headers that identify the browser brand. In headless mode, these headers contain `HeadlessChrome` — a dead giveaway. Pup overrides these at the CDP level using `Emulation.setUserAgentOverride` so the headers report `Google Chrome` instead.

This also covers `navigator.userAgentData.brands` and `getHighEntropyValues()` in JavaScript.

### WebDriver Property

Automated browsers set `navigator.webdriver` to `true`. Pup overrides this on `Navigator.prototype` to return `false`, matching a real non-automated Chrome. The override is placed on the prototype (not the navigator instance) to survive property descriptor inspection and `hasOwnProperty` checks.

The same override is applied inside iframes to prevent cross-frame detection.

### Automation Flags

- `--enable-automation` is removed from Chrome's default launch arguments
- `--disable-blink-features=AutomationControlled` prevents Chrome from flagging itself as automation-controlled

### HTTP Headers

An `Accept-Language` header is added to every page. Chrome with a fresh profile omits this header, which is a common bot signal.

### Browser Plugins

`navigator.plugins` is mocked with three standard Chrome plugins:
- Chrome PDF Plugin
- Chrome PDF Viewer
- Native Client

### Chrome Object

The `window.chrome` object is populated with properties present in real Chrome but missing in headless/automated mode:

- `chrome.runtime` — extension messaging stubs
- `chrome.app` — app installation API stubs
- `chrome.csi` — Client Side Instrumentation timing data
- `chrome.loadTimes` — page load timing data

### Other Patches

- `navigator.languages` returns `['en-US', 'en']`
- `navigator.permissions.query` returns correct notification permission state
- `Function.prototype.toString` is patched so overridden getters still report `[native code]`
- Iframe `contentWindow` access propagates the webdriver override to nested frames

## Customizing the User-Agent

```powershell
# Default: automatic stealth UA matching real browser version
$browser = Start-PupBrowser -Headless

# Custom UA string
$browser = Start-PupBrowser -Headless -UserAgent "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X)"

# Use the browser's native UA (no override)
$browser = Start-PupBrowser -Headless -UserAgent "none"
```

## Testing Stealth

The `tests/Stealth.Tests.ps1` file verifies stealth by navigating to [browserscan.net](https://www.browserscan.net) and asserting that the fingerprint authenticity score is at least 90%.

```powershell
Invoke-Pester ./tests/Stealth.Tests.ps1 -Output Detailed
```

## Limitations

Pup's stealth mode defeats standard browser fingerprint checks, but commercial bot protection services like DataDome, Akamai Bot Manager, and Cloudflare Bot Management use additional server-side detection (proprietary JavaScript challenges, IP reputation) that these patches cannot address.

For sites protected by these services, consider:

- **Manual session bootstrap** — open the site in non-headless mode, solve any challenge once, then export the session with `Export-PupSession` and reuse it
- **Residential proxies** — IP reputation is a factor in bot scoring
