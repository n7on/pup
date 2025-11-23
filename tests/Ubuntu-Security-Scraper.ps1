param($Date = "2024-01-01")

# Setup browser and navigate to Ubuntu security notices
$browser = Start-PupBrowser
try {
    $page = New-PupPage -Browser $browser -Url 'https://ubuntu.com/security/notices'
    Start-Sleep 5
} catch {
    Write-Error "Failed to load page: $_"
    Stop-PupBrowser -Browser $browser
    return
}

# Accept cookies if present
try { Find-PupElements -Page $page -Selector "button[data-js='cookie-policy-agree']" -First -Timeout 1000 | ForEach-Object { Invoke-PupElementClick -Element $_ } } catch {}

# Scrape security notices newer than cutoff date
$cutoff = [DateTime]$Date; $notices = @()
do {
    Find-PupElements -Page $page -Selector "article" | ForEach-Object {
        $link = Find-PupElements -Element $_ -Selector "a" -First
        $date = Find-PupElements -Element $_ -Selector "p, time" -First
        if ($link -and $date -and $date.InnerText) {
            try { 
                $parsed = [DateTime]::Parse($date.InnerText)
                if ($parsed -ge $cutoff) { 
                    $notices += @{Title=$link.InnerText; Date=$parsed.ToString('yyyy-MM-dd'); URL=(Get-PupElementAttribute -Element $link -Name "href")} 
                } else { $script:stop = $true } 
            } catch {} 
        }
    }
    # Navigate to next page if available
    $next = try { Find-PupElements -Page $page -Selector ".p-pagination__link--next" -First -Timeout 1000 } catch {}
    if ($next) { Invoke-PupElementClick -Element $next; Start-Sleep 2 }
} while ($next -and !$stop)

Stop-PupBrowser -Browser $browser
$notices