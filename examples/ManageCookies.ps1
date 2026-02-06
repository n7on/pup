# Get and manage cookies from a page
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad

# Get all cookies
$Cookies = $Page | Get-PupPageCookie
Write-Host "Found $($Cookies.Count) cookies:"
$Cookies | Format-Table Name, Value, Domain, Expires

$Browser | Stop-PupBrowser
