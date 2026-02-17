# Exercise 14: Example Block Page
# https://web-scraping.dev/blocked
# Navigate to a blocked page, detect the block, and click the unblock button

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/blocked" -WaitForLoad

# Check if page shows blocked message
$PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString

if ($PageText -match "blocked") {
    Write-Host "Block page detected!"
    Write-Host "Page message: $($PageText.Substring(0, [Math]::Min(200, $PageText.Length)))..."

    # Find and click the unblock button
    $UnblockBtn = $Page | Find-PupElements -TextContains "Unblock" -First
    if ($UnblockBtn) {
        Write-Host "`nClicking Unblock button..."
        $UnblockBtn | Invoke-PupElementClick -WaitForLoad
        Start-Sleep -Milliseconds 1000

        # Verify we're no longer blocked
        $NewUrl = $Page | Invoke-PupScript -Script "window.location.href" -AsString
        Write-Host "Redirected to: $NewUrl"
    } else {
        Write-Host "No Unblock button found."
    }
} else {
    Write-Host "Page is not blocked."
}

$Browser | Stop-PupBrowser
