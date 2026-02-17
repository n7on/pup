# Exercise 15: Blocking Redirect for Invalid Referer
# https://web-scraping.dev/credentials
# Bypass referer-based blocking by setting a valid Referer header

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/" -WaitForLoad

# First attempt: navigate directly (will likely be blocked due to missing/invalid referer)
Write-Host "Attempt 1: Direct navigation to /credentials"
$Page = $Page | Move-PupPage -Url "https://web-scraping.dev/credentials" -WaitForLoad
$Url = $Page | Invoke-PupScript -Script "window.location.href" -AsString
$PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString
Write-Host "  URL: $Url"
Write-Host "  Blocked: $($PageText -match 'blocked')"

# Second attempt: set a valid Referer header from the same site
Write-Host "`nAttempt 2: With valid Referer header"
$Page | Set-PupHttpHeader -Name "Referer" -Value "https://web-scraping.dev/"
$Page = $Page | Move-PupPage -Url "https://web-scraping.dev/credentials" -WaitForLoad
$Url = $Page | Invoke-PupScript -Script "window.location.href" -AsString
$PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString
Write-Host "  URL: $Url"

if ($PageText -notmatch 'blocked') {
    Write-Host "  Success! Credentials page content:"
    Write-Host "  $($PageText.Substring(0, [Math]::Min(300, $PageText.Length)))"
} else {
    Write-Host "  Still blocked. Trying alternative: navigate from valid page..."
    $Page | Set-PupHttpHeader -Clear
    $Page = $Page | Move-PupPage -Url "https://web-scraping.dev/" -WaitForLoad
    # Click a link to /credentials from the homepage (natural referer)
    $CredLink = $Page | Find-PupElements -Selector "a[href*='credentials']" -First
    if ($CredLink) {
        $CredLink | Invoke-PupElementClick -WaitForLoad
        $PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString
        Write-Host "  Content via click: $($PageText.Substring(0, [Math]::Min(300, $PageText.Length)))"
    }
}

$Browser | Stop-PupBrowser
