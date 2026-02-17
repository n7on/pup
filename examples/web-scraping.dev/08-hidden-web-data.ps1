# Exercise 8: Hidden Web Data
# https://web-scraping.dev/product/1
# Extract hidden JSON data from #reviews-data element and <script type="application/ld+json">

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/product/1" -WaitForLoad

# Extract reviews from hidden #reviews-data element
$ReviewsJson = $Page | Invoke-PupScript -Script @"
() => {
    const el = document.querySelector('#reviews-data');
    return el ? el.textContent : null;
}
"@ -AsString

if ($ReviewsJson) {
    $Reviews = $ReviewsJson | ConvertFrom-Json
    Write-Host "Hidden reviews data ($($Reviews.Count) reviews):"
    $Reviews | ForEach-Object {
        Write-Host "  [$($_.rating)/5] $($_.text)"
    }
} else {
    Write-Host "No #reviews-data element found."
}

# Extract structured data from ld+json script
$LdJson = $Page | Invoke-PupScript -Script @"
() => {
    const scripts = document.querySelectorAll('script[type="application/ld+json"]');
    return Array.from(scripts).map(s => s.textContent);
}
"@

Write-Host "`nStructured data (ld+json):"
foreach ($JsonStr in $LdJson) {
    $Data = $JsonStr | ConvertFrom-Json
    Write-Host "  Type: $($Data.'@type')"
    if ($Data.name) { Write-Host "  Name: $($Data.name)" }
    if ($Data.aggregateRating) {
        Write-Host "  Rating: $($Data.aggregateRating.ratingValue) ($($Data.aggregateRating.reviewCount) reviews)"
    }
}

$Browser | Stop-PupBrowser
