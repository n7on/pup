# Exercise 13: Cookie Popup
# https://web-scraping.dev/login?cookies=
# Dismiss the cookie consent popup to access the page content

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/login?cookies=" -WaitForLoad

# Wait for the cookie consent modal to appear
$CookieBtn = $Page | Find-PupElements -Selector "#cookie-ok" -First
if (-not $CookieBtn) {
    # Try waiting a moment for the modal to appear
    Start-Sleep -Milliseconds 1000
    $CookieBtn = $Page | Find-PupElements -Selector "#cookie-ok" -First
}

if ($CookieBtn) {
    Write-Host "Cookie consent popup detected. Clicking Accept..."
    $CookieBtn | Invoke-PupElementClick
    Start-Sleep -Milliseconds 500
    Write-Host "Cookie popup dismissed."
} else {
    Write-Host "No cookie popup found."
}

# Now interact with the page content behind the popup
$PageTitle = $Page | Invoke-PupScript -Script "document.title" -AsString
Write-Host "`nPage title: $PageTitle"

$FormElements = $Page | Find-PupElements -Selector "form input"
Write-Host "Form inputs found: $(@($FormElements).Count)"

$Browser | Stop-PupBrowser
