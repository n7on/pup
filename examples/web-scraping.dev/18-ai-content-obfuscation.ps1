# Exercise 18: AI Content Obfuscation
# https://web-scraping.dev/ai-content-obfuscation
# Extract clean text from a page that uses invisible Unicode characters to confuse scrapers

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/ai-content-obfuscation" -WaitForLoad

# Approach 1: Use innerText which the browser strips of invisible characters
$CleanText = $Page | Invoke-PupScript -Script @"
() => {
    const content = document.querySelector('.content, main, article');
    return content ? content.innerText : document.body.innerText;
}
"@ -AsString

Write-Host "Clean text via innerText:"
Write-Host $CleanText

# Approach 2: Get raw HTML source and clean invisible characters in PowerShell
Write-Host "`n--- Raw source analysis ---"
$RawHtml = $Page | Get-PupSource

# Count invisible characters
$InvisiblePattern = '[\u200B\u200C\u200D\uFEFF\u2060-\u2069\u202A-\u202E\u00AD\u034F\u061C\u180E]'
$InvisibleMatches = [regex]::Matches($RawHtml, $InvisiblePattern)
Write-Host "Invisible characters found in source: $($InvisibleMatches.Count)"

# Strip invisible characters from raw text
$BodyText = $Page | Invoke-PupScript -Script "document.body.textContent" -AsString
$Cleaned = [regex]::Replace($BodyText, $InvisiblePattern, '')
Write-Host "`nCleaned textContent (invisible chars removed):"
Write-Host $Cleaned

$Browser | Stop-PupBrowser
