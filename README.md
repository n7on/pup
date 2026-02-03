# Pup
Pup is a native PowerShell module made for browser automation. It's build upon PuppeteerSharp, which is a Dotnet library using DevTools API in order to automate the browser. It targets the netstandard 2.0, so it's fully supported on all powershell versions. 

# Examples

# Development

## Prerequisites

* Dotnet 8
* Pester

``` powershell
Install-Module -Name Pester

```

## Test
DLLs can't be unloaded from PowerShell, so you need to run tests in a different process, as below.
``` powershell
pwsh -Command "Import-Module Pester; Invoke-Pester .\tests\Browser.Tests.ps1 -Output Detailed"
pwsh -Command "Import-Module Pester; Invoke-Pester .\tests\Page.Tests.ps1 -Output Detailed"
pwsh -Command "Import-Module Pester; Invoke-Pester .\tests\Element.Tests.ps1 -Output Detailed"

``` 

# Troubleshooting

## Windows
Uninstall the old `Pester`that is shipped with Windows, and never updated.

``` PowerShell
$module = "C:\Program Files\WindowsPowerShell\Modules\Pester"
takeown /F $module /A /R
icacls $module /reset
icacls $module /grant "*S-1-5-32-544:F" /inheritance:d /T
Remove-Item -Path $module -Recurse -Force -Confirm:$false

Install-Module -Name Pester  -MaximumVersion 4.99 -Force

```