# Exercise 10: CSRF Token Locks
# https://web-scraping.dev/product/1
# Extract the CSRF token from a meta tag and use it for authenticated API requests

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/product/1" -WaitForLoad

# Extract CSRF token from meta tag
$CsrfToken = $Page | Invoke-PupScript -Script @"
() => {
    const meta = document.querySelector('meta[name="csrf-token"]');
    return meta ? meta.getAttribute('content') : null;
}
"@ -AsString

Write-Host "CSRF token: $CsrfToken"

if ($CsrfToken) {
    # Use the CSRF token to make an authenticated API request (e.g., add to cart)
    $Response = $Page | Invoke-PupHttpFetch `
        -Url "https://web-scraping.dev/api/product/1/cart" `
        -Method POST `
        -Headers @{ "x-csrf-token" = $CsrfToken } `
        -Body @{ quantity = 1 } `
        -AsJson

    Write-Host "`nAPI response (status $($Response.Status)):"
    if ($Response.Ok) {
        $Response.Body | ConvertTo-Json -Depth 3
    } else {
        Write-Host "Request failed: $($Response.StatusText)"
        Write-Host $Response.Body
    }
} else {
    Write-Host "No CSRF token found on the page."
}

$Browser | Stop-PupBrowser
