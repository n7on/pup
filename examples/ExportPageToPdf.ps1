# Export a webpage to PDF
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url,
    [string]$OutputPath = "page.pdf"
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad

$Page | Export-PupPagePdf -FilePath $OutputPath -Format A4
Write-Host "PDF saved to $OutputPath"

$Browser | Stop-PupBrowser
