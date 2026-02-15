@{
    RootModule = 'Pup.dll'
    ModuleVersion = '0.3.2'
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
        'Get-PupPageScreenshot',
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
        'Invoke-PupPageBack',
        'Invoke-PupPageForward',
        'Invoke-PupPageReload',
        'Invoke-PupPageScroll',
        'Invoke-PupScript',
        'Move-PupPage',
        'New-PupPage',
        'Remove-PupCookie',
        'Remove-PupPage',
        'Select-PupElementOption',
        'Send-PupFile',
        'Send-PupKey',
        'Send-PupWebSocketMessage',
        'Set-PupCookie',
        'Set-PupDialogHandler',
        'Set-PupElement',
        'Set-PupElementAttribute',
        'Set-PupElementValue',
        'Set-PupHttpAuth',
        'Set-PupHttpHeader',
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
v0.3.2
- Added -Fullscreen and -Maximized options to Start-PupBrowser
- Browser storage moved to module-scoped static storage (no more variable collision)
- Added HelpMessage to cmdlet parameters for better documentation

See CHANGELOG.md for full version history.
'@
        }
    }
}
