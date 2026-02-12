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
        'Clear-PupPageStorage',
        'Clear-PupRecording',
        'ConvertTo-PupScript',
        'Enter-PupConsole',
        'Export-PupPagePdf',
        'Export-PupPageSession',
        'Find-PupElements',
        'Get-PupBrowser',
        'Get-PupCertificate',
        'Get-PupElementAttribute',
        'Get-PupElementPattern',
        'Get-PupElementScreenshot',
        'Get-PupElementSelector',
        'Get-PupElementValue',
        'Get-PupPage',
        'Get-PupPageConsole',
        'Get-PupPageCookie',
        'Get-PupPageNetwork',
        'Get-PupPageScreenshot',
        'Get-PupPageSource',
        'Get-PupPageStorage',
        'Get-PupRecording',
        'Get-PupWebSocket',
        'Import-PupPageSession',
        'Install-PupBrowser',
        'Invoke-PupElementClick',
        'Invoke-PupElementFocus',
        'Invoke-PupElementHover',
        'Invoke-PupElementScroll',
        'Invoke-PupCdpMessage',
        'Invoke-PupHttpFetch',
        'Invoke-PupPageBack',
        'Invoke-PupPageForward',
        'Invoke-PupPageReload',
        'Invoke-PupPageScript',
        'Invoke-PupPageScroll',
        'Move-PupPage',
        'New-PupPage',
        'Remove-PupPage',
        'Remove-PupPageCookie',
        'Select-PupElementOption',
        'Send-PupFile',
        'Send-PupKey',
        'Send-PupWebSocketMessage',
        'Set-PupDialogHandler',
        'Set-PupElement',
        'Set-PupElementAttribute',
        'Set-PupElementValue',
        'Set-PupHttpAuth',
        'Set-PupHttpHeader',
        'Set-PupPageCookie',
        'Set-PupPageStorage',
        'Set-PupPageViewport',
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
