BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
}

Describe "Stealth" {
    BeforeAll {
        Install-PupBrowser -BrowserType Chrome
        $script:browser = Start-PupBrowser -Headless
        $script:page = New-PupPage -Browser $script:browser
    }

    AfterAll {
        if ($script:page.Running) { Remove-PupPage -Page $script:page }
        if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
    }

    It "Passes browserscan.net fingerprint authenticity check" {
        Move-PupPage -Page $script:page -Url "https://www.browserscan.net" -WaitForLoad

        # Poll until the score appears (up to 30 seconds)
        $score = $null
        for ($i = 0; $i -lt 15; $i++) {
            Start-Sleep -Seconds 2
            $match = Select-PupText -Page $script:page -Pattern 'Browser\s*fingerprint\s*authenticity[:\s]*(\d+)%'
            if ($match) { $score = [int]$match; break }
        }

        $score | Should -Not -BeNullOrEmpty -Because "the fingerprint score should appear within 30 seconds"
        $score | Should -BeGreaterOrEqual 90 -Because "browser fingerprint authenticity should be at least 90%"
    }

    It "Passes iphey.com trust check" {
        Move-PupPage -Page $script:page -Url "https://iphey.com" -WaitForLoad

        # Poll until the verdict appears (up to 30 seconds)
        $verdict = $null
        for ($i = 0; $i -lt 15; $i++) {
            Start-Sleep -Seconds 2
            $match = Select-PupText -Page $script:page -Pattern '\b(Trustworthy|Suspicious|Unreliable)\b'
            if ($match) { $verdict = $match; break }
        }

        $verdict | Should -Be "Trustworthy" -Because "iphey.com should report the browser as Trustworthy"
    }
}
