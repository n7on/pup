# Exercise 16: Persistent Cookie-Based Blocking
# https://web-scraping.dev/blocked?persist=
# Handle persistent blocking that sets a cookie to remember the block

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/blocked?persist=" -WaitForLoad

# Check the block page
$PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString
Write-Host "Initial page (blocked):"
Write-Host "  $($PageText.Substring(0, [Math]::Min(200, $PageText.Length)))..."

# Show blocking cookies
$Cookies = $Page | Get-PupCookie
Write-Host "`nCookies set by block page:"
$Cookies | Format-Table Name, Value, Domain -AutoSize

# Remove all blocking cookies
Write-Host "Removing blocking cookies..."
$Page | Remove-PupCookie -All

# Verify cookies are cleared
$CookiesAfter = $Page | Get-PupCookie
Write-Host "Cookies after removal: $(@($CookiesAfter).Count)"

# Navigate again - should no longer be blocked
Write-Host "`nNavigating to the site again..."
$Page = $Page | Move-PupPage -Url "https://web-scraping.dev/" -WaitForLoad
$Url = $Page | Invoke-PupScript -Script "window.location.href" -AsString
$PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString
Write-Host "URL: $Url"
Write-Host "Blocked: $($PageText -match 'blocked')"

$Browser | Stop-PupBrowser
