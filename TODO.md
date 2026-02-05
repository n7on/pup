# Pup TODO - Missing Features & Enhancements

This document tracks missing functionality and enhancements.

## Pentesting Features

- [x] **Stealth mode** - Hide `navigator.webdriver` and other bot detection signals
  - Always enabled by default
- [x] **File upload** - Set file input values for testing upload vulnerabilities
  - `Send-PupFile -Element $el -FilePath "malicious.pdf"`
- [x] **HTTP Basic Auth** - Handle auth dialogs automatically
  - `Set-PupHttpAuth -Page $p -Username "admin" -Password "pass"`
- [x] **Custom HTTP Headers** - Inject headers into all requests
  - `Set-PupHttpHeader -Page $p -Name "X-Forwarded-For" -Value "127.0.0.1"`
- [x] **WebSocket support** - Test WS-based APIs
  - `Get-PupWebSocket` - List active WebSocket connections with frames
  - `Send-PupWebSocketMessage` - Send messages through WebSocket
- [x] **In-browser HTTP requests** - Make fetch requests from browser context
  - `Invoke-PupHttpFetch -Page $p -Url "/api/data" -Method POST -Body @{...}`
- [ ] **iframe support** - Interact with elements inside iframes
  - `Get-PupFrame` / `Switch-PupFrame` - List and switch iframes
  - `Find-PupElements -Frame $frame -Selector "..."`
- [ ] **Multi-tab/popup handling** - Handle OAuth popups, window.open() flows
  - `Get-PupPage -All` - List all pages/tabs
  - Event handler for new popups

## Auth & Permissions
- [ ] **Set-PupPermission** - Camera/mic/location/notifications toggles

## Session Portability
- [x] **Export/Import-PupPageSession** - Bundle cookies + storage to/from file
  - `Export-PupPageSession -Page $p -FilePath "session.json"`
  - `Import-PupPageSession -Page $p -FilePath "session.json" -Reload`
