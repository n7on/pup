# Exercise 6: Forced New Tab Links
# https://web-scraping.dev/reviews
# Handle links that open in new tabs using browser popup event handlers

Import-Module Pup

$Browser = Start-PupBrowser -Headless

# Track new pages opened by the browser
$global:NewPages = @()
Set-PupBrowserHandler -Browser $Browser -Event PopupCreated -ScriptBlock {
    param($e)
    $global:NewPages += $e.Page
    Write-Host "New tab opened: $($e.Page.Url)"
}

$Page = New-PupPage -Url "https://web-scraping.dev/reviews" -WaitForLoad

# Find the link that opens in a new tab (e.g., code of conduct)
$NewTabLinks = $Page | Find-PupElements -Selector "a[target='_blank']"

if (-not $NewTabLinks) {
    # Some links use JavaScript window.open instead of target="_blank"
    $NewTabLinks = $Page | Find-PupElements -Selector "a[onclick*='window.open']"
}

if ($NewTabLinks) {
    Write-Host "Found $(@($NewTabLinks).Count) new-tab link(s). Clicking first one..."
    $NewTabLinks[0] | Invoke-PupElementClick
    Start-Sleep -Milliseconds 2000
}

# Process captured new tab pages
if ($global:NewPages.Count -gt 0) {
    foreach ($NewPage in $global:NewPages) {
        Write-Host "`nNew tab content:"
        Write-Host "  URL: $($NewPage.Url)"
        $Title = $NewPage | Invoke-PupScript -Script "document.title" -AsString
        Write-Host "  Title: $Title"
    }
} else {
    Write-Host "No new tabs were opened."
}

$Browser | Stop-PupBrowser
