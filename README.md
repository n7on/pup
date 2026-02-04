# Pup
Pup is a native PowerShell module made for browser automation. It's build upon PuppeteerSharp, which is a Dotnet library using DevTools API in order to automate the browser. It targets the netstandard 2.0, so it's fully supported on all powershell versions. 

# Examples
This example scrape Ubunty security notices. And return the date and link to security issues.

```powershell
[CmdletBinding()]
param(
    [datetime]$FromDate = (Get-Date).AddMonths(-1)
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://ubuntu.com/security/notices" 

$Date = Get-Date
while ($Date -gt $FromDate) {

    $Page | Find-PupElements -WaitForLoad -Selector "#notices-list > section" | ForEach-Object {
        $Date = [datetime]($_ | Find-PupElements -Selector ".row > div.col-6 > p:first-child").InnerHTML.Trim()
        $Link = ($_ | Find-PupElements -Selector ".u-fixed-width > h3").InnerHTML.Trim()
        [PSCustomObject]@{
            Date  = $Date
            Link  = $Link
        }
    }
    $Page | Find-PupElements -Selector "a.p-pagination__link--next" | Invoke-PupElementClick
}
$Browser | Stop-PupBrowser

```

# Development

## Prerequisites

* Dotnet 8
* Pester

## Test
DLLs can't be unloaded from PowerShell, so you need to run tests in a different process, as below.
``` powershell
pwsh -Command "Invoke-Pester ./tests/Browser.Tests.ps1 -Output Detailed"
pwsh -Command "Invoke-Pester ./tests/Page.Tests.ps1 -Output Detailed"
pwsh -Command "Invoke-Pester ./tests/Element.Tests.ps1 -Output Detailed"

# or all tests
pwsh -Command "Invoke-Pester ./tests/ -Output Detailed"
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