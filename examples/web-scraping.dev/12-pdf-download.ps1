# Exercise 12: PDF Downloads
# https://web-scraping.dev/login
# Download PDF files (Terms of Service, EULA) using Invoke-PupHttpFetch -OutFile

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/login" -WaitForLoad

$OutputDir = Join-Path $PSScriptRoot "downloads"
if (-not (Test-Path $OutputDir)) {
    New-Item -Path $OutputDir -ItemType Directory | Out-Null
}

# Find PDF download links
$PdfLinks = $Page | Find-PupElements -Selector "a[href*='.pdf']"

if ($PdfLinks) {
    foreach ($Link in $PdfLinks) {
        $Href = $Link | Get-PupElementAttribute -Name "href"
        $LinkText = $Link | Get-PupElementAttribute -Name "innerText"
        $FileName = [System.IO.Path]::GetFileName($Href)
        $OutPath = Join-Path $OutputDir $FileName

        Write-Host "Downloading '$LinkText' from $Href..."
        $Page | Invoke-PupHttpFetch -Url $Href -OutFile $OutPath
        Write-Host "  Saved to $OutPath"
    }
} else {
    Write-Host "No PDF links found, downloading known PDF directly..."
    $OutPath = Join-Path $OutputDir "tos.pdf"
    $Page | Invoke-PupHttpFetch -Url "https://web-scraping.dev/assets/pdf/tos.pdf" -OutFile $OutPath
    Write-Host "  Saved to $OutPath"
}

Write-Host "`nDownloaded files:"
Get-ChildItem $OutputDir -Filter "*.pdf" | Format-Table Name, Length

$Browser | Stop-PupBrowser
