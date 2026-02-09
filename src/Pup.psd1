@{
    RootModule = 'Pup.dll'
    ModuleVersion = '0.2.2'
    GUID = '17b431d1-d9da-44e6-b740-8ad3bfb4c0cf'
    Author = 'Anton Lindström'
    CompanyName = 'Anton Lindström'
    Copyright = '(c) 2026 Anton Lindström. All rights reserved.'
    Description = 'Browser automation for PowerShell. Control headless or visible Chrome/Chromium browsers via the DevTools Protocol. Automate web scraping, form filling, screenshots, PDF export, network interception, WebSocket monitoring, and session management. Convert Chrome DevTools recordings to PowerShell scripts. Cross-platform support for Windows, Linux, and macOS.'
    
    PowerShellVersion = '5.1'
    DotNetFrameworkVersion = '4.6.1'
    
    FunctionsToExport = @()
    CmdletsToExport = @(
        'Clear-PupPageStorage',
        'Convert-PupRecording',
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
        'Stop-PupBrowser',
        'Uninstall-PupBrowser',
        'Wait-PupElement'
    )
    VariablesToExport = @()
    AliasesToExport = @()
    
    PrivateData = @{
        PSData = @{
            Tags = @('Browser', 'Automation', 'WebScraping', 'Puppeteer', 'Chrome', 'Chromium', 'Headless', 'Selenium', 'CDP', 'DevTools', 'Screenshot', 'PDF', 'Testing', 'Web')
            LicenseUri = 'https://github.com/n7on/Pup/blob/main/LICENSE'
            ProjectUri = 'https://github.com/n7on/Pup'
            IconUri = ''
            ReleaseNotes = @'
Pup v0.2.0

Browser Automation:
- Install, launch, and control Chrome/Chromium (headless or visible)
- Multi-page/tab management with navigation controls

Web Interaction:
- Find elements via CSS selectors or XPath
- Click, hover, scroll, focus, and fill forms
- Handle dialogs, file uploads, and keyboard input

Data Extraction:
- Screenshots (page or element), PDF export
- Extract HTML, text, attributes, and form values
- Cookie and local/session storage management

Advanced Features:
- Network interception with custom headers and HTTP auth
- WebSocket monitoring and messaging
- Export/import browser sessions
- Convert Chrome DevTools recordings to PowerShell scripts
- Proxy support and stealth mode
- Interactive console mode

Cross-platform: Windows, Linux, macOS | PowerShell 5.1+
'@
        }
    }
}
