# Monitor all network requests made by a page
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url $Url -WaitForLoad

# Get all captured network requests
$Page | Get-PupPageNetwork | ForEach-Object {
    [PSCustomObject]@{
        Method = $_.Method
        Url    = $_.Url
        Status = $_.Status
        Type   = $_.MimeType
    }
}

$Browser | Stop-PupBrowser
