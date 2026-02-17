# Exercise 3: Secret API Token
# https://web-scraping.dev/testimonials
# Discover the X-Secret-Token from network requests, then use it to call the API directly

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/testimonials" -WaitForLoad

# Scroll once to trigger an API request
$Page | Invoke-PupScript -Script "window.scrollTo(0, document.body.scrollHeight)" -AsVoid
Start-Sleep -Milliseconds 2000

# Inspect network requests to find the secret token
$Requests = $Page | Get-PupNetwork
$ApiRequests = $Requests | Where-Object { $_.Url -like "*api*" -or $_.Url -like "*testimonial*" }

Write-Host "API requests found:"
$ApiRequests | ForEach-Object {
    Write-Host "  $($_.Method) $($_.Url)"
    if ($_.RequestHeaders) {
        $SecretHeader = $_.RequestHeaders | Where-Object { $_.Key -like "*secret*" -or $_.Key -like "*token*" }
        if ($SecretHeader) {
            Write-Host "  Secret header: $($SecretHeader.Key) = $($SecretHeader.Value)"
        }
    }
}

# Use the discovered token to fetch data directly
$Token = "secret123"
Write-Host "`nUsing discovered token to fetch testimonials via API..."

$Response = $Page | Invoke-PupHttpFetch -Url "https://web-scraping.dev/api/testimonials" -Headers @{
    "X-Secret-Token" = $Token
} -AsJson

if ($Response.Ok) {
    Write-Host "API response:"
    $Response.Body | ConvertTo-Json -Depth 5
} else {
    Write-Host "API request failed: $($Response.Status) $($Response.StatusText)"
}

$Browser | Stop-PupBrowser
