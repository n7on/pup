BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = "file://" + [System.IO.Path]::Combine($PSScriptRoot, "fixtures", "test-page.html")
    $script:browser = Start-PupBrowser -Headless
}

AfterAll {
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Page Creation" {
    It "Creates a new page" {
        $page = New-PupPage -Browser $script:browser
        $page | Should -Not -BeNullOrEmpty
        $page.Running | Should -BeTrue
        Remove-PupPage -Page $page
    }

    It "Creates page with URL" {
        $page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
        $page.Page.Url | Should -BeLike "*test-page.html*"
        Remove-PupPage -Page $page
    }
}

Describe "Page Navigation" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Remove-PupPage -Page $script:page
    }

    It "Navigates to URL" {
        Move-PupPage -Page $script:page -Url "https://example.com" -WaitForLoad
        $script:page.Page.Url | Should -BeLike "*example.com*"
    }

    It "Navigates back" {
        Invoke-PupPageBack -Page $script:page -WaitForLoad
        $script:page.Page.Url | Should -BeLike "*test-page.html*"
    }

    It "Navigates forward" {
        Invoke-PupPageForward -Page $script:page -WaitForLoad
        $script:page.Page.Url | Should -BeLike "*example.com*"
    }

    It "Reloads page" {
        Move-PupPage -Page $script:page -Url $script:testUrl -WaitForLoad
        Invoke-PupPageReload -Page $script:page -WaitForLoad
        $script:page.Page.Url | Should -BeLike "*test-page.html*"
    }
}

Describe "Page Screenshot" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Remove-PupPage -Page $script:page
    }

    It "Takes screenshot as bytes" {
        $bytes = Get-PupPageScreenshot -Page $script:page -PassThru
        $bytes.Length | Should -BeGreaterThan 1000
    }

    It "Saves screenshot to file" {
        $path = Join-Path ([IO.Path]::GetTempPath()) "test-screenshot.png"
        Get-PupPageScreenshot -Page $script:page -FilePath $path
        Test-Path $path | Should -BeTrue
        Remove-Item $path
    }
}

Describe "Page PDF Export" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Remove-PupPage -Page $script:page
    }

    It "Exports PDF as bytes" {
        $bytes = Export-PupPdf -Page $script:page -PassThru
        $bytes.Length | Should -BeGreaterThan 1000
    }

    It "Saves PDF to file" {
        $path = Join-Path ([IO.Path]::GetTempPath()) "test-export.pdf"
        Export-PupPdf -Page $script:page -FilePath $path
        Test-Path $path | Should -BeTrue
        Remove-Item $path
    }

    It "Gets page source" {
        $html = Get-PupSource -Page $script:page
        $html | Should -BeLike "*Pup Test Page*"
        $html | Should -Match 'id=["'']title["'']'
    }
}

Describe "Page JavaScript" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Remove-PupPage -Page $script:page
    }

    It "Executes script returning number" {
        $result = Invoke-PupScript -Page $script:page -Script "() => 2 + 2" -AsNumber
        $result | Should -Be 4
    }

    It "Executes script returning string" {
        $result = Invoke-PupScript -Page $script:page -Script "() => document.title" -AsString
        $result | Should -Be "Pup Test Page"
    }

    It "Executes script returning boolean" {
        $result = Invoke-PupScript -Page $script:page -Script "() => true" -AsBoolean
        $result | Should -BeTrue
    }
}

Describe "Page Cookies" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url "https://example.com" -WaitForLoad
    }

    AfterAll {
        Remove-PupPage -Page $script:page
    }

    It "Sets and gets cookie" {
        Set-PupCookie -Page $script:page -Name "test" -Value "value123" -Domain "example.com"
        $cookies = Get-PupCookie -Page $script:page -Name "test"
        $cookies.Value | Should -Be "value123"
    }

    It "Removes cookie" {
        Set-PupCookie -Page $script:page -Name "toremove" -Value "value456" -Domain "example.com"
        Get-PupCookie -Page $script:page -Name "toremove" | Should -Not -BeNullOrEmpty

        Remove-PupCookie -Page $script:page -Name "toremove" -Domain "example.com"

        $after = Get-PupCookie -Page $script:page -Name "toremove"
        $after | Should -BeNullOrEmpty
    }

    It "Removes cookies by wildcard and domain" {
        Set-PupCookie -Page $script:page -Name "session1" -Value "abc" -Domain "example.com"
        Set-PupCookie -Page $script:page -Name "session2" -Value "def" -Domain "example.com"
        Set-PupCookie -Page $script:page -Name "other" -Value "ghi" -Domain "example.com"

        Remove-PupCookie -Page $script:page -Name "session*" -Domain "example.com"

        (Get-PupCookie -Page $script:page -Name "session*").Count | Should -Be 0
        (Get-PupCookie -Page $script:page -Name "other").Count | Should -Be 1
    }
}

Describe "Network Capture" {
    BeforeAll {
        $script:netPage = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        if ($script:netPage.Running) { Remove-PupPage -Page $script:netPage }
    }

    It "captures requests and saves to file" {
        Invoke-PupPageReload -Page $script:netPage -WaitForLoad
        $entries = Get-PupNetwork -Page $script:netPage

        $entries.Count | Should -BeGreaterThan 0
        ($entries | Where-Object { $_.Url -like "*test-page.html*" }).Count | Should -BeGreaterThan 0
        ($entries | Where-Object { $_.Url -like "*network.js*" }).Count | Should -BeGreaterThan 0
    }
}

Describe "Console Capture" {
    BeforeAll {
        $script:consolePage = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        if ($script:consolePage.Running) { Remove-PupPage -Page $script:consolePage }
    }

    It "captures console output" {
        Invoke-PupScript -Page $script:consolePage -Script "() => { console.log('hello'); console.error('boom'); }" -AsVoid
        $logs = Get-PupConsole -Page $script:consolePage

        $logs | Where-Object { $_.Type -eq "Log" -and $_.Text -like "*hello*" } | Should -Not -BeNullOrEmpty
        $logs | Where-Object { $_.Type -eq "Error" -and $_.Text -like "*boom*" } | Should -Not -BeNullOrEmpty
    }
}

Describe "Page Storage" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        if ($script:page.Running) { Remove-PupPage -Page $script:page }
    }

    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
        Clear-PupStorage -Page $script:page -Type Local
        Clear-PupStorage -Page $script:page -Type Session
    }

    It "Sets and gets local storage entry" {
        Set-PupStorage -Page $script:page -Type Local -Key "foo" -Value "bar"
        $entry = Get-PupStorage -Page $script:page -Type Local -Key "foo"
        $entry.Value | Should -Be "bar"
    }

    It "Clears single local storage entry" {
        Set-PupStorage -Page $script:page -Type Local -Key "foo" -Value "bar"
        Clear-PupStorage -Page $script:page -Type Local -Key "foo"
        Get-PupStorage -Page $script:page -Type Local -Key "foo" | Should -BeNullOrEmpty
    }

    It "Sets multiple session storage entries" {
        Set-PupStorage -Page $script:page -Type Session -Items @{ a = "1"; b = "2" }
        $all = Get-PupStorage -Page $script:page -Type Session
        ($all | Where-Object { $_.Key -eq "a" }).Value | Should -Be "1"
        ($all | Where-Object { $_.Key -eq "b" }).Value | Should -Be "2"
    }
}

Describe "Page Keyboard" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Remove-PupPage -Page $script:page
    }

    It "Types text" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        $el | Invoke-PupElementFocus
        Send-PupKey -Page $script:page -Text "hello"
        $value = Invoke-PupScript -Page $script:page -Script "() => document.getElementById('username').value" -AsString
        $value | Should -Be "hello"
    }

    It "Sends key with modifier" {
        Send-PupKey -Page $script:page -Key "a" -Modifiers "Control"
        { Send-PupKey -Page $script:page -Key "Backspace" } | Should -Not -Throw
    }
}

Describe "Page Dialog Handler" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Remove-PupPageHandler -Page $script:page -Event Dialog -ErrorAction SilentlyContinue
        Remove-PupPage -Page $script:page
    }

    It "Accepts alert dialogs" {
        Set-PupPageHandler -Page $script:page -Event Dialog -Action Accept
        $btn = Find-PupElements -Page $script:page -Selector "#btn-alert" -First
        { $btn | Invoke-PupElementClick; Start-Sleep -Milliseconds 200 } | Should -Not -Throw
    }

    It "Dismisses confirm dialogs" {
        Set-PupPageHandler -Page $script:page -Event Dialog -Action Dismiss
        $btn = Find-PupElements -Page $script:page -Selector "#btn-confirm" -First
        { $btn | Invoke-PupElementClick; Start-Sleep -Milliseconds 200 } | Should -Not -Throw
    }
}

Describe "Request Headers" {
    BeforeAll {
        $script:headerPage = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Set-PupHttpHeader -Page $script:headerPage -Clear
        Remove-PupPage -Page $script:headerPage
    }

    It "Sets single header" {
        { Set-PupHttpHeader -Page $script:headerPage -Name "X-Custom-Header" -Value "test-value" } | Should -Not -Throw
    }

    It "Sets multiple headers via hashtable" {
        { Set-PupHttpHeader -Page $script:headerPage -Headers @{ "X-Header-One" = "value1"; "X-Header-Two" = "value2" } } | Should -Not -Throw
    }

    It "Clears headers" {
        { Set-PupHttpHeader -Page $script:headerPage -Clear } | Should -Not -Throw
    }
}

Describe "HTTP Authentication" {
    BeforeAll {
        $script:authPage = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        Set-PupHttpAuth -Page $script:authPage -Clear
        Remove-PupPage -Page $script:authPage
    }

    It "Sets authentication with username and password" {
        { Set-PupHttpAuth -Page $script:authPage -Username "testuser" -Password "testpass" } | Should -Not -Throw
    }

    It "Sets authentication with PSCredential" {
        $securePass = ConvertTo-SecureString "testpass" -AsPlainText -Force
        $cred = New-Object PSCredential("testuser", $securePass)
        { Set-PupHttpAuth -Page $script:authPage -Credential $cred } | Should -Not -Throw
    }

    It "Clears authentication" {
        { Set-PupHttpAuth -Page $script:authPage -Clear } | Should -Not -Throw
    }
}

Describe "Interactive Console" {
    It "Command exists with correct parameters" {
        $cmd = Get-Command Enter-PupConsole -ErrorAction SilentlyContinue
        $cmd | Should -Not -BeNullOrEmpty
        $cmd.Parameters.Keys | Should -Contain "Page"
        $cmd.Parameters.Keys | Should -Contain "Prompt"
    }
}
