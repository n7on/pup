# Exercise 11: Cookies Based Login
# https://web-scraping.dev/login
# Log in using form submission and inspect the session cookies

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/login" -WaitForLoad

# Fill in the login form
$UsernameInput = $Page | Find-PupElements -Selector "input[name='username'], #username" -First
$PasswordInput = $Page | Find-PupElements -Selector "input[name='password'], #password" -First

if ($UsernameInput -and $PasswordInput) {
    $UsernameInput | Set-PupElementValue -Value "user123"
    $PasswordInput | Set-PupElementValue -Value "pass123"
    Write-Host "Filled login form"

    # Submit the form
    $SubmitBtn = $Page | Find-PupElements -Selector "button[type='submit'], input[type='submit']" -First
    if (-not $SubmitBtn) {
        $SubmitBtn = $Page | Find-PupElements -Text "Submit" -First
    }

    if ($SubmitBtn) {
        $SubmitBtn | Invoke-PupElementClick -WaitForLoad
        Write-Host "Submitted login form"
        Start-Sleep -Milliseconds 1000
    }
} else {
    Write-Host "Login form fields not found"
}

# Inspect cookies after login
$Cookies = $Page | Get-PupCookie
Write-Host "`nCookies after login:"
$Cookies | Format-Table Name, Value, Domain, HttpOnly, Secure -AutoSize

# Check if login was successful by looking for session cookie
$SessionCookie = $Cookies | Where-Object { $_.Name -match "session|auth|token" }
if ($SessionCookie) {
    Write-Host "Session cookie found: $($SessionCookie.Name) = $($SessionCookie.Value)"
} else {
    Write-Host "No session cookie found - login may have failed"
}

$Browser | Stop-PupBrowser
