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

        # Wait for the detection tests to complete and the score to appear
        Start-Sleep -Seconds 10

        # Extract the fingerprint authenticity percentage from the page
        $score = Invoke-PupScript -Page $script:page -Script @"
() => {
    const text = document.body.innerText;
    const match = text.match(/Browser\s*fingerprint\s*authenticity[:\s]*(\d+)%/i);
    return match ? parseInt(match[1], 10) : -1;
}
"@

        $score | Should -BeGreaterOrEqual 90 -Because "browser fingerprint authenticity should be at least 90%"
    }

    It "Passes iphey.com trust check" {
        Move-PupPage -Page $script:page -Url "https://iphey.com" -WaitForLoad

        # Wait for the fingerprint analysis to complete
        Start-Sleep -Seconds 15

        # Extract the trust verdict from the page
        $verdict = Invoke-PupScript -Page $script:page -Script @"
() => {
    const text = document.body.innerText;
    const match = text.match(/\b(Trustworthy|Suspicious|Unreliable)\b/i);
    return match ? match[1] : 'not found';
}
"@

        $verdict | Should -Be "Trustworthy" -Because "iphey.com should report the browser as Trustworthy"
    }
}
