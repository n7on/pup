# Exercise 1: Static Paging
# https://web-scraping.dev/products
# Scrape all products across paginated pages where each page has its own URL (?page=N)

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/products" -WaitForLoad

$AllProducts = @()
$PageNum = 1

while ($true) {
    Write-Host "Scraping page $PageNum..."

    $Products = $Page | Find-PupElements -Selector ".product-thumb"
    foreach ($Product in $Products) {
        $Title = $Product | Find-PupElements -Selector "h3" -First
        $TitleText = $Title | Get-PupElementAttribute -Name "innerText"

        $PriceEl = $Product | Find-PupElements -Selector ".product-thumb-price" -First
        $Price = $PriceEl | Get-PupElementAttribute -Name "innerText"

        $Link = $Product | Find-PupElements -Selector "a" -First
        $Href = $Link | Get-PupElementAttribute -Name "href"

        $AllProducts += [PSCustomObject]@{
            Title = $TitleText
            Price = $Price
            Url   = $Href
        }
    }

    # Check if there is a next page link
    $NextLink = $Page | Find-PupElements -Selector "a.page-link" -TextContains ">"
    if (-not $NextLink) { break }

    $PageNum++
    $Page = $Page | Move-PupPage -Url "https://web-scraping.dev/products?page=$PageNum" -WaitForLoad
}

Write-Host "`nFound $($AllProducts.Count) products across $PageNum pages:"
$AllProducts | Format-Table -AutoSize

$Browser | Stop-PupBrowser
