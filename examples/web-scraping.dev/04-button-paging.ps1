# Exercise 4: Endless Button Paging
# https://web-scraping.dev/reviews
# Load all reviews by clicking the "Load More" button until all content is loaded

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/reviews" -WaitForLoad

$MaxClicks = 50

for ($i = 0; $i -lt $MaxClicks; $i++) {
    $LoadMoreBtn = $Page | Find-PupElements -Selector "#page-load-more" -First
    if (-not $LoadMoreBtn) {
        Write-Host "No more 'Load More' button found. Done."
        break
    }

    # Check if button is hidden (all content loaded)
    $IsHidden = $LoadMoreBtn | Get-PupElementAttribute -Name "className"
    if ($IsHidden -like "*d-none*") {
        Write-Host "Load More button is hidden. All content loaded."
        break
    }

    $LoadMoreBtn | Invoke-PupElementClick
    Write-Host "Clicked Load More (attempt $($i + 1))"

    # Wait for spinner to disappear
    Start-Sleep -Milliseconds 1000
}

# Extract all loaded reviews
$Reviews = $Page | Find-PupElements -Selector "[data-testid='review']"
$Results = foreach ($Review in $Reviews) {
    $Text = $Review | Find-PupElements -Selector "[data-testid='review-text']" -First
    $Date = $Review | Find-PupElements -Selector "[data-testid='review-date']" -First

    [PSCustomObject]@{
        Date = if ($Date) { $Date | Get-PupElementAttribute -Name "innerText" } else { "" }
        Text = if ($Text) { $Text | Get-PupElementAttribute -Name "innerText" } else { "" }
    }
}

Write-Host "`nLoaded $($Results.Count) reviews:"
$Results | Format-Table -Wrap

$Browser | Stop-PupBrowser
