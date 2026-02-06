# Execute JavaScript on a page and get results
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad
# Get page title via JavaScript
$Title = $Page | Invoke-PupPageScript -Script "document.title"

# Get all links on the page
$Links = $Page | Invoke-PupPageScript -Script "() => Array.from(document.querySelectorAll('a')).map(a => ({text: a.innerText.trim(), href: a.href})).filter(a => a.href && a.text)"

Write-Host "Page title: $Title"
Write-Host "Found $($Links.Count) links"
$Links | ForEach-Object { Write-Host $_.href }

$Browser | Stop-PupBrowser
