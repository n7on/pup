# Pup TODO - Missing Features & Enhancements

This document tracks missing functionality and enhancements.

## Network & Performance
- [ ] **Get-BrowserHar** - Export network log/filters  
  - Params: `-IncludeBody`, `-MaxContentBytes`, `-Filter` (domain/type/status)  
  - Export HAR/JSON for captured traffic
- [ ] **Set-BrowserNetworkCondition** - Simulate network conditions  
  - Params: `-Speed` (Fast3G, Slow3G, Offline), `-Latency`, `-Throughput`

## Frames
- [ ] **Get-BrowserFrame** / **Switch-BrowserFrame** - List and switch iframes

## Auth & Permissions
- [ ] **Set-BrowserAuthentication** - HTTP auth  
  - Params: `-Username`, `-Password`, `-Type`
- [ ] **Set-BrowserPermission** - Camera/mic/location/notifications toggles

## Device Emulation
- [ ] **Set-BrowserMobileEmulation** - Device profiles (width/height/user agent/touch)
- [ ] **Set-BrowserThrottle** - Network throttling/offline switches

## Session Portability
- [ ] **Export/Import-BrowserSession** - Bundle cookies + storage to/from file

## Quality of Life
- [ ] **Use system-installed browser** - Allow `-ExecutablePath` to launch local Chrome/Chromium

## CI/CD
- [x] **GitHub Actions** - Automated build/test on PR/push
- [ ] **Code coverage** - Track coverage metrics
- [ ] **Test reporting** - Publish Pester results
