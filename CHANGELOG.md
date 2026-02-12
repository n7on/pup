# Changelog

All notable changes to this project will be documented in this file.

## [Unreleased]

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
