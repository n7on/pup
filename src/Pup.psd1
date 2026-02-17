@{
    RootModule = 'Pup.dll'
    ModuleVersion = '0.4.3'
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
v0.4.3
- Invoke-PupRecording: replay recorded interactions on a page
- Set-PupDownloadPath: configure browser download directory
- Invoke-PupHttpFetch -OutFile: binary-safe file downloads (images, PDFs, ZIPs)
- Download event handler now fully functional with state tracking and file renaming
- Set-PupViewport now also resizes the browser window to match the viewport
- -Width/-Height on Start-PupBrowser and New-PupPage are now optional (viewport auto-resizes with window)
- -Fullscreen and -Maximized on Start-PupBrowser now work correctly with auto-resizing viewport
- Recording fixes: better input capture, suppressed synthetic clicks from Enter key, AltGr handling

See CHANGELOG.md for full version history.
'@
        }
    }
}
