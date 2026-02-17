# Exercise 7: Product HTML Markup
# https://web-scraping.dev/product/1
# Extract product data from standard HTML markup using CSS selectors

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/product/1" -WaitForLoad

# Extract product title
$TitleEl = $Page | Find-PupElements -Selector "h3" -First
$Title = $TitleEl | Get-PupElementAttribute -Name "innerText"

# Extract price
$PriceEl = $Page | Find-PupElements -Selector ".product-price" -First
$Price = if ($PriceEl) { $PriceEl | Get-PupElementAttribute -Name "innerText" } else { "N/A" }

# Extract description
$DescEl = $Page | Find-PupElements -Selector ".product-description" -First
$Description = if ($DescEl) { $DescEl | Get-PupElementAttribute -Name "innerText" } else { "N/A" }

# Extract product images
$Images = $Page | Find-PupElements -Selector ".product-image img, .product-gallery img"
$ImageUrls = foreach ($Img in $Images) {
    $Img | Get-PupElementAttribute -Name "src"
}

# Extract variant options
$Variants = $Page | Find-PupElements -Selector ".variant, [data-variant]"
$VariantNames = foreach ($V in $Variants) {
    $V | Get-PupElementAttribute -Name "innerText"
}

# Build product object
$Product = [PSCustomObject]@{
    Title       = $Title
    Price       = $Price
    Description = $Description
    Images      = $ImageUrls -join ", "
    Variants    = $VariantNames -join ", "
}

Write-Host "Product data extracted:"
$Product | Format-List

$Browser | Stop-PupBrowser
