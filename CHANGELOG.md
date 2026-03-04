# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

## [0.4.5] - 2026-03-04

### Added
- `-Visible` flag on `Find-PupElements` to return only elements that are rendered and not hidden by CSS
- Default dialog handler — pages auto-dismiss `alert()`/`confirm()`/`prompt()` dialogs to prevent freezes (user-registered handlers take precedence)

### Fixed
- **Browser crash when opening/closing multiple pages** — CDP sessions created for network monitoring were never detached on page close, leaking resources in Chrome until it crashed
- **Browser crash from closing last page** — closing the only remaining page caused Chrome to exit; `Remove-PupPage` now ensures at least one page stays open
- **Browser crash in headless mode on macOS** — Metal GPU backend (`--use-gl=angle --use-angle=metal`) could crash Chrome's GPU process in headless; now only applied in GUI mode
- **Browser crash at startup in headless mode** — default `about:blank` page was closed leaving Chrome with 0 pages, causing a race condition on the next `New-PupPage`
- `IsVisible` property always returning `false` — replaced unreliable `IsIntersectingViewportAsync` (IntersectionObserver) with a synchronous layout dimension check that works in all modes
- Stale element errors flooding the pipeline — all element commands now detect navigation-invalidated elements and emit a single warning instead of errors for each remaining element

### Changed
- Capture lists (console, network, WebSocket entries) are now trimmed when exceeding 10,000 entries to prevent unbounded memory growth

## [0.4.4] - 2026-02-23

### Added
- `Select-PupText` - extract text from a page or frame using a regex pattern, returning the first capture group (or full match if no groups)
- Stealth documentation (`docs/stealth.md`)
- Stealth tests against browserscan.net and iphey.com (`tests/Stealth.Tests.ps1`)

### Changed
- Major stealth improvements — scores 100% on browserscan.net fingerprint authenticity
  - User-agent now built dynamically from the actual installed Chrome version and OS instead of a hardcoded string
  - CDP-level client hints override (`sec-ch-ua` headers) replaces `HeadlessChrome` with `Google Chrome`
  - `Accept-Language` header injected to avoid fresh-profile bot signal
  - `navigator.webdriver` override moved to `Navigator.prototype` to survive property descriptor inspection
  - `navigator.platform` spoofed to match user-agent OS
  - WebGL renderer override when SwiftShader (headless software renderer) is detected
  - Screen dimensions, `outerWidth`/`outerHeight`, `devicePixelRatio`, and `colorDepth` patched for headless consistency
  - `navigator.connection` (Network Information API) mocked
  - `navigator.mimeTypes` mocked to match plugin list
  - `Notification.permission` fixed to return `default` instead of `denied`
  - `navigator.userAgentData` and `getHighEntropyValues()` patched to hide HeadlessChrome
  - `window.chrome.app`, `chrome.csi`, and `chrome.loadTimes` stubs added
  - Plugin list updated to match modern Chrome (5 PDF viewer entries)
  - `Function.prototype.toString` patch improved with a `Set` of patched getters
- Browser launch now uses full Chrome instead of ChromeHeadlessShell for better stealth
- `--enable-automation` removed from default launch args via `IgnoredDefaultArgs`
- `--disable-blink-features=AutomationControlled` and `--lang=en-US` added to launch args
- GPU acceleration enabled (Metal on macOS) to avoid SwiftShader fingerprint

## [0.4.3] - 2026-02-17

### Added
- `Invoke-PupRecording` - replay recorded interactions on a page with optional `-Delay` between actions
- `Set-PupDownloadPath` - configure browser download directory or disable downloads with `-Deny`
- `-OutFile` parameter on `Invoke-PupHttpFetch` for binary-safe file downloads (images, PDFs, ZIPs, etc.)
- Download event handler now fully functional in `Set-PupPageHandler -Event Download` with state tracking, file renaming, and ScriptBlock callbacks

### Changed
- `Set-PupViewport` now also resizes the browser window to match the requested viewport dimensions
- `-Width` and `-Height` on `Start-PupBrowser` and `New-PupPage` are now optional — when omitted, the viewport auto-resizes with the browser window instead of defaulting to 1280x720
- `-Fullscreen` and `-Maximized` on `Start-PupBrowser` now work correctly with auto-resizing viewport

### Fixed
- Recording: suppressed synthetic click events caused by Enter key on buttons/links
- Recording: text input now captured on focus-out instead of a debounce timer, preventing missed or partial values
- Recording: pending input is flushed before action keys (Enter, Tab, Escape) and before `Stop-PupRecording`
- Recording: bare modifier keys (Ctrl, Shift, Alt, Meta) are no longer recorded as keydown events
- Recording: AltGr key combinations (used for characters like @ on some keyboards) no longer incorrectly recorded as Ctrl combos

## [0.4.0] - 2026-02-15

### Added
- **Frame support** for working with iframes and nested frames
  - `Get-PupFrame` - get frames from a page with filtering by `-Name`, `-Url`, `-First`, `-IncludeMain`
  - `Find-PupElements -Frame` - find elements within a specific frame
  - `Wait-PupElement -Frame` - wait for elements in a frame
  - `Invoke-PupScript -Frame` - execute JavaScript in frame context
  - `Get-PupSource -Frame` - get HTML source from a frame
- **Event handler system** for browser and page-level events
  - `Set-PupBrowserHandler` - register handlers for browser events (PopupCreated, PageCreated, PageClosed, Disconnected)
  - `Get-PupBrowserHandler` - view active browser event handlers
  - `Remove-PupBrowserHandler` - remove browser event handlers
  - `Set-PupPageHandler` - register handlers for page events (Dialog, Console, PageError, Load, DOMContentLoaded, Request, Response, FrameNavigated, and more)
  - `Get-PupPageHandler` - view active page event handlers
  - `Remove-PupPageHandler` - remove page event handlers
  - ScriptBlock support for custom event handling logic
  - Built-in actions (Accept, Dismiss, Ignore) for common scenarios like dialog handling

## [0.3.2] - 2026-02-12

### Added
- `-Fullscreen` and `-Maximized` options for `Start-PupBrowser`

### Changed
- Browser storage moved from session variable to module-scoped static storage (no more variable collision)
- Added `HelpMessage` to cmdlet parameters for better documentation

## [0.3.1] - 2026-02-11

### Fixed
- Windows PowerShell compatibility fix

## [0.3.0] - 2026-02-10

### Added
- Live browser interaction recording (`Start-PupRecording`, `Stop-PupRecording`, `Get-PupRecording`, `Clear-PupRecording`)
- `ConvertTo-PupScript` to convert recorded events to PowerShell scripts
- `Get-PupElementPattern` for finding CSS selector patterns
- Commands section in README with categorized command list

### Changed
- Refactored JavaScript code into embedded resources
- Improved documentation

## [0.2.2] - 2026-02-09

### Added
- Search elements by visible text (`-Text`, `-TextContains` parameters)
- `Get-PupCertificate` for retrieving page certificates

## [0.2.1] - 2026-02-08

### Fixed
- Bug fixes and stability improvements

## [0.2.0] - 2026-02-07

### Added
- Initial public release
- Browser automation via Chrome DevTools Protocol
- Element finding, clicking, typing, screenshots
- PDF export, cookie management, storage access
- Network interception, WebSocket monitoring
- Session export/import
- Cross-platform support (Windows, Linux, macOS)
