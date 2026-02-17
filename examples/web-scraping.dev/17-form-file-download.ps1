# Exercise 17: Form File Attachment Download
# https://web-scraping.dev/file-download
# Download a file triggered by form submission using Invoke-PupHttpFetch

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/file-download" -WaitForLoad

$OutputDir = Join-Path $PSScriptRoot "downloads"
if (-not (Test-Path $OutputDir)) {
    New-Item -Path $OutputDir -ItemType Directory | Out-Null
}

# Approach 1: Use Invoke-PupHttpFetch to POST directly to the download API
Write-Host "Downloading file via API POST..."
$OutPath = Join-Path $OutputDir "download-sample.pdf"

$Response = $Page | Invoke-PupHttpFetch `
    -Url "https://web-scraping.dev/api/download-file" `
    -Method POST `
    -OutFile $OutPath

Write-Host "File saved to: $OutPath"
if (Test-Path $OutPath) {
    $FileInfo = Get-Item $OutPath
    Write-Host "File size: $($FileInfo.Length) bytes"
}

# Approach 2: Click the download button (triggers form POST in new tab)
Write-Host "`nAlternative: Clicking the download button..."
$DownloadBtn = $Page | Find-PupElements -Selector "#download-btn" -First
if ($DownloadBtn) {
    Write-Host "Download button found: #download-btn"
    # For button-triggered downloads, the direct API approach above is more reliable
    Write-Host "  (Using direct API call is recommended for automation)"
}

$Browser | Stop-PupBrowser
