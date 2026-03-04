@{
    RootModule = 'Pup.dll'
    ModuleVersion = '0.4.5'
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
v0.4.5
- Fixed browser crash when closing pages (CDP session leak)
- Fixed browser crash from closing last page (Chrome exits with 0 pages)
- Fixed Metal GPU crash in headless mode on macOS
- Added -Visible flag to Find-PupElements
- Fixed IsVisible always returning false (replaced unreliable IntersectionObserver with layout check)
- Stale element handling in pipeline commands
- Default dialog handler prevents page freezes
- Capture list trimming prevents unbounded memory growth

See CHANGELOG.md for full version history.
'@
        }
    }
}
