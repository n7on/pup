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
- [x] **iframe support** - Interact with elements inside iframes
  - `Get-PupFrame -Page $p -Name "frameName"` - List and filter frames
  - `Find-PupElements -Frame $frame -Selector "..."` - Find elements in frame
  - `Wait-PupElement -Frame $frame -Selector "..."` - Wait for elements in frame
  - `Invoke-PupScript -Frame $frame -Script "..."` - Execute JS in frame context
  - `Get-PupSource -Frame $frame` - Get frame HTML source
- [x] **Multi-tab/popup handling** - Handle OAuth popups, window.open() flows
  - `Get-PupPage` - List all pages/tabs (already existed)
  - `Set-PupBrowserHandler -Browser $b -Event PopupCreated -Action Dismiss` - Auto-close popups
  - `Set-PupBrowserHandler -Browser $b -Event PopupCreated -ScriptBlock { param($e) $e.Page }` - Custom popup handling

## Event Handlers
- [x] **Set-PupBrowserHandler** - Handle browser-level events with actions or script blocks
  - Events: PopupCreated, PageCreated, PageClosed, Disconnected
  - `Set-PupBrowserHandler -Browser $b -Event PopupCreated -ScriptBlock { param($e) Write-Host "Popup: $($e.Url)" }`
  - `Set-PupBrowserHandler -Browser $b -Event PopupCreated -Remove` - Remove handler
- [x] **Set-PupPageHandler** - Handle page-level events with actions or script blocks
  - Events: Dialog, Console, PageError, Load, DOMContentLoaded, Request, RequestFinished, RequestFailed, Response, FrameAttached, FrameDetached, FrameNavigated, Download, FileChooser, WorkerCreated, WorkerDestroyed, Close
  - `Set-PupPageHandler -Page $p -Event Dialog -Action Accept` - Auto-accept dialogs
  - `Set-PupPageHandler -Page $p -Event Console -ScriptBlock { param($e) Write-Host "$($e.Type): $($e.Text)" }`
  - `Set-PupPageHandler -Page $p -Event Request -ScriptBlock { param($e) if ($e.Url -match "api") { Write-Host $e.Url } }`

## Auth & Permissions
- [x] **Set-PupPagePermission** - Camera/mic/location/notifications toggles
  - `Set-PupPagePermission -Page $p -Permission "geolocation" -State "Granted"`
  - Supports: geolocation, notifications, camera, microphone, clipboard-read, clipboard-write, midi, accelerometer, gyroscope, magnetometer, and more
  - States: Granted, Denied, Prompt

## Session Portability
- [x] **Export/Import-PupPageSession** - Bundle cookies + storage to/from file
  - `Export-PupPageSession -Page $p -FilePath "session.json"`
  - `Import-PupPageSession -Page $p -FilePath "session.json" -Reload`

## Script Generation
- [x] **Convert-PupRecording** - Convert Chrome DevTools Recorder to Pup scripts
  - `Convert-PupRecording -InputFile "recording.json" -OutputFile "script.ps1" -IncludeSetup`
  - Supports: navigate, click, doubleClick, change, keyDown, scroll, hover, waitForElement, setViewport
