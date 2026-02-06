# Export a browser session (cookies, storage) and import it later
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url,
    [string]$SessionFile = "session.json"
)

Import-Module Pup
Install-PupBrowser

# First session - browse and export
$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad

Write-Host "Exporting session from $Url..."
$Session = $Page | Export-PupPageSession
$Session | ConvertTo-Json -Depth 10 | Set-Content $SessionFile
Write-Host "Session saved to $SessionFile"
Write-Host "  Cookies: $($Session.Cookies.Count)"
Write-Host "  LocalStorage: $($Session.LocalStorage.Count) items"
Write-Host "  SessionStorage: $($Session.SessionStorage.Count) items"

$Browser | Stop-PupBrowser

# To import the session later:
# $Session = Get-Content $SessionFile | ConvertFrom-Json
# $Page | Import-PupPageSession -Session $Session
