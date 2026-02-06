# Get localStorage and sessionStorage from a page
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad

# Get localStorage
$LocalStorage = $Page | Get-PupPageStorage -Type Local
Write-Host "LocalStorage ($($LocalStorage.Count) items):"
$LocalStorage | Format-Table -AutoSize

# Get sessionStorage
$SessionStorage = $Page | Get-PupPageStorage -Type Session
Write-Host "SessionStorage ($($SessionStorage.Count) items):"
$SessionStorage | Format-Table -AutoSize

$Browser | Stop-PupBrowser
