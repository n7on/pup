# Exercise 2: Endless Scroll Paging
# https://web-scraping.dev/testimonials
# Scrape all testimonials by scrolling until no new content loads

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/testimonials" -WaitForLoad

$PreviousCount = 0
$MaxScrollAttempts = 50

for ($i = 0; $i -lt $MaxScrollAttempts; $i++) {
    $Testimonials = $Page | Find-PupElements -Selector ".testimonial"
    $CurrentCount = @($Testimonials).Count

    if ($CurrentCount -eq $PreviousCount) {
        Write-Host "No new testimonials loaded after scroll. Done."
        break
    }

    Write-Host "Scroll $($i + 1): $CurrentCount testimonials loaded"
    $PreviousCount = $CurrentCount

    # Scroll to bottom to trigger lazy loading
    $Page | Invoke-PupScript -Script "window.scrollTo(0, document.body.scrollHeight)" -AsVoid
    Start-Sleep -Milliseconds 1000
}

# Extract all testimonial data
$Testimonials = $Page | Find-PupElements -Selector ".testimonial"
$Results = foreach ($T in $Testimonials) {
    $Text = $T | Get-PupElementAttribute -Name "innerText"
    [PSCustomObject]@{ Text = $Text }
}

Write-Host "`nCollected $($Results.Count) testimonials"
$Results | Format-Table -Wrap

$Browser | Stop-PupBrowser
