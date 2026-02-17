# Exercise 19: Challenge + File Download
# https://web-scraping.dev/challenge-download
# Solve the challenge on the page and download the resulting file

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/challenge-download" -WaitForLoad

$OutputDir = Join-Path $PSScriptRoot "downloads"
if (-not (Test-Path $OutputDir)) {
    New-Item -Path $OutputDir -ItemType Directory | Out-Null
}

# Inspect the challenge page
$PageText = $Page | Invoke-PupScript -Script "document.body.innerText" -AsString
Write-Host "Challenge page content:"
Write-Host $PageText

# Look for challenge elements (buttons, forms, puzzles)
$Buttons = $Page | Find-PupElements -Selector "button"
$Forms = $Page | Find-PupElements -Selector "form"
$Inputs = $Page | Find-PupElements -Selector "input"

Write-Host "`nPage elements: $(@($Buttons).Count) buttons, $(@($Forms).Count) forms, $(@($Inputs).Count) inputs"

# Attempt to solve the challenge by interacting with page elements
foreach ($Btn in $Buttons) {
    $BtnText = $Btn | Get-PupElementAttribute -Name "innerText"
    Write-Host "Found button: '$BtnText'"
}

# Try clicking challenge buttons/solving via JavaScript
$ChallengeBtn = $Page | Find-PupElements -Selector "button" -First
if ($ChallengeBtn) {
    $ChallengeBtn | Invoke-PupElementClick
    Write-Host "Clicked challenge button"
    Start-Sleep -Milliseconds 2000
}

# Check for download links after solving
$DownloadLinks = $Page | Find-PupElements -Selector "a[href*='download'], a[download]"
if ($DownloadLinks) {
    foreach ($Link in $DownloadLinks) {
        $Href = $Link | Get-PupElementAttribute -Name "href"
        $FileName = [System.IO.Path]::GetFileName($Href)
        if (-not $FileName) { $FileName = "challenge-download.pdf" }
        $OutPath = Join-Path $OutputDir $FileName

        Write-Host "Downloading: $Href"
        $Page | Invoke-PupHttpFetch -Url $Href -OutFile $OutPath
        Write-Host "Saved to: $OutPath"
    }
} else {
    Write-Host "No download links found yet. The challenge may require additional steps."
    # Try to find any dynamically generated download
    $AllLinks = $Page | Find-PupElements -Selector "a[href]"
    Write-Host "All links on page:"
    foreach ($Link in $AllLinks) {
        $Href = $Link | Get-PupElementAttribute -Name "href"
        $Text = $Link | Get-PupElementAttribute -Name "innerText"
        Write-Host "  [$Text] -> $Href"
    }
}

$Browser | Stop-PupBrowser
