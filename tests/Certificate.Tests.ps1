BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url "https://example.com" -WaitForLoad
}

AfterAll {
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Get-PupCertificate Passive Mode" {
    It "Captures security details from HTTPS requests" {
        $certs = Get-PupCertificate -Page $script:page
        $certs.Count | Should -BeGreaterThan 0
    }

    It "Has protocol information" {
        $certs = Get-PupCertificate -Page $script:page
        $certs[0].Protocol | Should -BeLike "TLS*"
    }

    It "Has subject name" {
        $certs = Get-PupCertificate -Page $script:page
        $certs[0].SubjectName | Should -Not -BeNullOrEmpty
    }

    It "Has validity dates" {
        $certs = Get-PupCertificate -Page $script:page
        $certs[0].ValidFrom | Should -Not -BeNullOrEmpty
        $certs[0].ValidTo | Should -Not -BeNullOrEmpty
    }

    It "Has cipher information" {
        $certs = Get-PupCertificate -Page $script:page
        $certs[0].Cipher | Should -Not -BeNullOrEmpty
    }

    It "Calculates DaysUntilExpiry" {
        $certs = Get-PupCertificate -Page $script:page
        $certs[0].DaysUntilExpiry | Should -BeGreaterThan 0
    }

    It "Filters by URL pattern" {
        $certs = Get-PupCertificate -Page $script:page -UrlFilter "example.com"
        $certs.Count | Should -BeGreaterThan 0
        $certs | ForEach-Object { $_.Url | Should -BeLike "*example.com*" }
    }

    It "Returns empty for non-matching URL filter" {
        $certs = Get-PupCertificate -Page $script:page -UrlFilter "nonexistent-domain-xyz"
        $certs.Count | Should -Be 0
    }

    It "Deduplicates with -Unique" {
        $all = Get-PupCertificate -Page $script:page
        $unique = Get-PupCertificate -Page $script:page -Unique
        $unique.Count | Should -BeLessOrEqual $all.Count
    }

    It "Filters by ExpiringWithin" {
        # Certificates expiring within 365 days should include most valid certs
        $certs = Get-PupCertificate -Page $script:page -ExpiringWithin 365
        # May or may not have results depending on cert validity
        { Get-PupCertificate -Page $script:page -ExpiringWithin 365 } | Should -Not -Throw
    }
}

Describe "Get-PupCertificate Active Mode" {
    It "Fetches certificate for origin" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert | Should -Not -BeNullOrEmpty
    }

    It "Returns certificate chain" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.CertificateChain.Count | Should -BeGreaterThan 0
    }

    It "Parses subject name from certificate" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.SubjectName | Should -Not -BeNullOrEmpty
    }

    It "Parses issuer from certificate" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.Issuer | Should -Not -BeNullOrEmpty
    }

    It "Parses validity dates" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.ValidFrom | Should -Not -BeNullOrEmpty
        $cert.ValidTo | Should -Not -BeNullOrEmpty
    }

    It "Calculates IsValid" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.IsValid | Should -BeTrue
    }

    It "Calculates DaysUntilExpiry" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.DaysUntilExpiry | Should -BeGreaterThan 0
    }

    It "Has thumbprint" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.Thumbprint | Should -Not -BeNullOrEmpty
    }

    It "Has serial number" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.SerialNumber | Should -Not -BeNullOrEmpty
    }

    It "Sets origin property" {
        $cert = Get-PupCertificate -Page $script:page -Origin "https://example.com"
        $cert.Origin | Should -Be "https://example.com"
    }
}

Describe "SecurityDetails in Network Entries" {
    It "Network entries have SecurityDetails for HTTPS" {
        $network = Get-PupPageNetwork -Page $script:page
        $httpsEntry = $network | Where-Object { $_.Url -like "https://*" } | Select-Object -First 1
        $httpsEntry.SecurityDetails | Should -Not -BeNullOrEmpty
    }

    It "SecurityDetails has expected properties" {
        $network = Get-PupPageNetwork -Page $script:page
        $httpsEntry = $network | Where-Object { $_.Url -like "https://*" } | Select-Object -First 1
        $httpsEntry.SecurityDetails.Protocol | Should -Not -BeNullOrEmpty
        $httpsEntry.SecurityDetails.SubjectName | Should -Not -BeNullOrEmpty
    }
}
