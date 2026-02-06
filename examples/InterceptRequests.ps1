# Add custom headers to all requests (useful for auth tokens)
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$Url,
    [string]$HeaderName = "X-Custom-Header",
    [string]$HeaderValue = "CustomValue123"
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage

# Set custom header for all requests
$Page | Set-PupHttpHeader -Name $HeaderName -Value $HeaderValue

# Navigate - all requests will include the custom header
$Page = $Page | Move-PupPage -Url $Url -WaitForLoad

# Check network log to verify header was sent
$Entry = ($Page | Get-PupPageNetwork)[0]
$Entry.RequestHeaders

$Browser | Stop-PupBrowser
