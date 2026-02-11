BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url "https://example.com" -WaitForLoad
}

AfterAll {
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Invoke-PupCdpMessage" {
    It "Gets browser version" {
        $result = Invoke-PupCdpMessage -Page $script:page -Method "Browser.getVersion"
        $result.product | Should -BeLike "*Chrome*"
        $result.protocolVersion | Should -Not -BeNullOrEmpty
    }

    It "Gets DOM document" {
        $result = Invoke-PupCdpMessage -Page $script:page -Method "DOM.getDocument" -Parameters @{ depth = 1 }
        $result.root.nodeId | Should -BeGreaterThan 0
        $result.root.nodeName | Should -Be "#document"
    }

    It "Gets page frame tree" {
        $result = Invoke-PupCdpMessage -Page $script:page -Method "Page.getFrameTree"
        $result.frameTree.frame.url | Should -BeLike "*example.com*"
    }

    It "Returns raw JSON with -AsJson" {
        $result = Invoke-PupCdpMessage -Page $script:page -Method "Browser.getVersion" -AsJson
        $result | Should -BeOfType [string]
        $result | Should -BeLike "*protocolVersion*"
    }

    It "Handles parameters correctly" {
        $result = Invoke-PupCdpMessage -Page $script:page -Method "DOM.getDocument" -Parameters @{ depth = 2; pierce = $false }
        $result.root | Should -Not -BeNullOrEmpty
    }

    It "Reports error for invalid method" {
        { Invoke-PupCdpMessage -Page $script:page -Method "Invalid.method" -ErrorAction Stop } | Should -Throw
    }
}
