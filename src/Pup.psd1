@{
    RootModule = 'bin/Debug/netstandard2.0/Pup.dll'
    ModuleVersion = '1.0.0'
    GUID = '17b431d1-d9da-44e6-b740-8ad3bfb4c0cf'
    Author = 'Anton Lindström'
    CompanyName = 'Anton Lindström'
    Copyright = '(c) 2025 Anton Lindström. All rights reserved.'
    Description = 'PowerShell module for browser automation powered by PuppeteerSharp. Install browsers, start/stop them, create/manage pages, and automate web interactions.'
    
    PowerShellVersion = '5.1'
    DotNetFrameworkVersion = '4.6.1'
    
    # Include PowerShell script modules
    NestedModules = @('Pup.Functions.psm1')
    
    FunctionsToExport = @('Invoke-PupPageSequence', 'Start-PupSession', 'Stop-PupSession', 'Get-PupElementText', 'Get-PupBestSelector', 'Find-PupSimilarElements')
    CmdletsToExport = @('*')
    VariablesToExport = @()
    AliasesToExport = @('*')
    
    PrivateData = @{
        PSData = @{
            Tags = @('Browser', 'Automation', 'PowerShell', 'WebScraping', 'Testing', 'Puppeteer', 'Chrome')
            LicenseUri = 'https://github.com/yourusername/Pup/blob/main/LICENSE'
            ProjectUri = 'https://github.com/yourusername/Pup'
            IconUri = ''
            ReleaseNotes = @'
Initial release of Pup v1.0.0

Features:
- Install and launch Chrome/Chromium browsers
- Create and manage browser pages
- Navigate to URLs
- Take screenshots
- Find and interact with page elements
- Click elements and fill forms
- Extract page content

Powered by PuppeteerSharp
'@
        }
    }
}