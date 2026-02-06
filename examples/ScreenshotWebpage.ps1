# Take a full-page screenshot of a website
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url,
    [string]$OutputPath = "screenshot.png"
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad

$Page | Get-PupPageScreenshot -FilePath $OutputPath -FullPage
Write-Host "Screenshot saved to $OutputPath"

$Browser | Stop-PupBrowser
