@{
    RootModule = 'Pup.dll'
    ModuleVersion = '0.4.4'
    GUID = '17b431d1-d9da-44e6-b740-8ad3bfb4c0cf'
    Author = 'Anton Lindström'
    CompanyName = 'Anton Lindström'
    Copyright = '(c) 2026 Anton Lindström. All rights reserved.'
    Description = 'Browser automation for PowerShell using the Chrome DevTools Protocol. Scrape websites, fill forms, take screenshots, export PDFs, and record interactions as replayable scripts. Works with PowerShell 5.1+ on Windows, Linux, and macOS.'

    PowerShellVersion = '5.1'
    DotNetFrameworkVersion = '4.6.1'

    FunctionsToExport = @()
    CmdletsToExport = @(
        'Clear-PupRecording',
        'Clear-PupStorage',
        'ConvertTo-PupScript',
        'Enter-PupConsole',
        'Export-PupPdf',
        'Export-PupSession',
        'Find-PupElements',
        'Get-PupBrowser',
        'Get-PupBrowserHandler',
        'Get-PupCertificate',
        'Get-PupConsole',
        'Get-PupCookie',
        'Get-PupElementAttribute',
        'Get-PupElementPattern',
        'Get-PupElementScreenshot',
        'Get-PupElementSelector',
        'Get-PupElementValue',
        'Get-PupFrame',
        'Get-PupNetwork',
        'Get-PupPage',
        'Get-PupPageHandler',
        'Get-PupPageScreenshot',
        'Get-PupPermission',
        'Get-PupRecording',
        'Get-PupSource',
        'Get-PupStorage',
        'Get-PupWebSocket',
        'Import-PupSession',
        'Install-PupBrowser',
        'Invoke-PupCdpMessage',
        'Invoke-PupElementClick',
        'Invoke-PupElementFocus',
        'Invoke-PupElementHover',
        'Invoke-PupElementScroll',
        'Invoke-PupHttpFetch',
        'Invoke-PupRecording',
        'Invoke-PupPageBack',
        'Invoke-PupPageForward',
        'Invoke-PupPageReload',
        'Invoke-PupPageScroll',
        'Invoke-PupScript',
        'Move-PupPage',
        'New-PupPage',
        'Remove-PupBrowserHandler',
        'Remove-PupCookie',
        'Remove-PupPage',
        'Remove-PupPageHandler',
        'Select-PupElementOption',
        'Select-PupText',
        'Send-PupFile',
        'Send-PupKey',
        'Send-PupWebSocketMessage',
        'Set-PupCookie',
        'Set-PupDownloadPath',
        'Set-PupElement',
        'Set-PupElementAttribute',
        'Set-PupElementValue',
        'Set-PupBrowserHandler',
        'Set-PupHttpAuth',
        'Set-PupPageHandler',
        'Set-PupHttpHeader',
        'Set-PupPermission',
        'Set-PupStorage',
        'Set-PupViewport',
        'Start-PupBrowser',
        'Start-PupRecording',
        'Stop-PupBrowser',
        'Stop-PupRecording',
        'Uninstall-PupBrowser',
        'Wait-PupElement'
    )
    VariablesToExport = @()
    AliasesToExport = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Browser', 'Automation', 'WebScraping', 'Scraping', 'Puppeteer', 'Chrome', 'Chromium', 'Headless', 'Selenium', 'CDP', 'DevTools', 'Screenshot', 'PDF', 'Testing', 'Web', 'E2E', 'Crawler', 'RPA', 'WebDriver', 'CrossPlatform', 'Linux', 'macOS', 'Windows')
            LicenseUri = 'https://github.com/n7on/Pup/blob/main/LICENSE'
            ProjectUri = 'https://github.com/n7on/Pup'
            IconUri = ''
            ReleaseNotes = @'
v0.4.4
- Select-PupText: extract text from a page or frame using a regex pattern
- Major stealth improvements — scores 100% on browserscan.net fingerprint authenticity
- User-agent now built dynamically from the actual installed Chrome version and OS
- CDP-level client hints override (sec-ch-ua headers) to hide HeadlessChrome
- WebGL, screen dimensions, devicePixelRatio, plugins, and other fingerprints patched for headless
- Uses full Chrome instead of ChromeHeadlessShell for better stealth
- Removed --enable-automation flag, added --disable-blink-features=AutomationControlled

See CHANGELOG.md for full version history.
'@
        }
    }
}
