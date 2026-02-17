# Exercise 9: Local Storage
# https://web-scraping.dev/product/1
# Add a product to cart and read the cart data from localStorage

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/product/1" -WaitForLoad

# Check localStorage before adding to cart
$StorageBefore = $Page | Get-PupStorage -Type Local
Write-Host "LocalStorage before:"
if ($StorageBefore) {
    $StorageBefore | Format-Table -AutoSize
} else {
    Write-Host "  (empty)"
}

# Click "Add to Cart" button
$AddToCartBtn = $Page | Find-PupElements -Text "Add to Cart" -First
if ($AddToCartBtn) {
    $AddToCartBtn | Invoke-PupElementClick
    Write-Host "`nClicked 'Add to Cart'"
    Start-Sleep -Milliseconds 1000
} else {
    Write-Host "Add to Cart button not found"
}

# Read localStorage after adding to cart
$StorageAfter = $Page | Get-PupStorage -Type Local
Write-Host "`nLocalStorage after:"
$StorageAfter | Format-Table -AutoSize

# Parse cart data if present
$CartData = $Page | Get-PupStorage -Type Local -Key "cart"
if ($CartData) {
    Write-Host "Cart contents:"
    $Cart = $CartData | ConvertFrom-Json
    $Cart | ConvertTo-Json -Depth 5
}

$Browser | Stop-PupBrowser
