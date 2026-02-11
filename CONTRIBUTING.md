# Contributing to Pup

If you want to contribute to the source you're highly welcome!

## Prerequisites

* Dotnet 8
* Pester

## Build

```powershell
dotnet build src/Pup.csproj
```

## Test

DLLs can't be unloaded from PowerShell, so you need to run tests in a different process, as below.

```powershell
pwsh -Command "Invoke-Pester ./tests/Browser.Tests.ps1 -Output Detailed"
pwsh -Command "Invoke-Pester ./tests/Page.Tests.ps1 -Output Detailed"
pwsh -Command "Invoke-Pester ./tests/Element.Tests.ps1 -Output Detailed"

# or all tests
# PowerShell 7+
pwsh -Command "Invoke-Pester ./tests/ -Output Detailed"
# PowerShell 5
powershell.exe -Command "Invoke-Pester ./tests/ -Output Detailed"
```

## Generating Documentation

Documentation is generated using [PlatyPS](https://github.com/PowerShell/platyPS).

```powershell
Install-Module PlatyPS -Scope CurrentUser

# Generate markdown from loaded module
Import-Module ./output/Pup/Pup.psd1
New-MarkdownHelp -Module Pup -OutputFolder ./docs/commands -Force

# Generate MAML from markdown (for Get-Help)
New-ExternalHelp -Path ./docs/commands -OutputPath ./src/en-US -Force
```

# Troubleshooting

## Windows

Uninstall the old `Pester` that is shipped with Windows, and never updated.

```powershell
$module = "C:\Program Files\WindowsPowerShell\Modules\Pester"
takeown /F $module /A /R
icacls $module /reset
icacls $module /grant "*S-1-5-32-544:F" /inheritance:d /T
Remove-Item -Path $module -Recurse -Force -Confirm:$false

Install-Module -Name Pester -MaximumVersion 4.99 -Force
```
