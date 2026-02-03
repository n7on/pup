BeforeAll {
    Import-Module "$PSScriptRoot\..\output\Pup\Pup.psd1" -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = Join-Path $PSScriptRoot "fixtures" "test-page.html"
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
        $bytes = Export-PupPagePdf -Page $script:page -PassThru
        $bytes.Length | Should -BeGreaterThan 1000
    }

    It "Saves PDF to file" {
        $path = Join-Path ([IO.Path]::GetTempPath()) "test-export.pdf"
        Export-PupPagePdf -Page $script:page -FilePath $path
        Test-Path $path | Should -BeTrue
        Remove-Item $path
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
        $result = Invoke-PupPageScript -Page $script:page -Script "() => 2 + 2" -AsNumber
        $result | Should -Be 4
    }

    It "Executes script returning string" {
        $result = Invoke-PupPageScript -Page $script:page -Script "() => document.title" -AsString
        $result | Should -Be "Pup Test Page"
    }

    It "Executes script returning boolean" {
        $result = Invoke-PupPageScript -Page $script:page -Script "() => true" -AsBoolean
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
        Set-PupPageCookie -Page $script:page -Name "test" -Value "value123" -Domain "example.com"
        $cookies = Get-PupPageCookie -Page $script:page -Name "test"
        $cookies.Value | Should -Be "value123"
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
        $value = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('username').value" -AsString
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
        Set-PupDialogHandler -Page $script:page -Remove
        Remove-PupPage -Page $script:page
    }

    It "Accepts alert dialogs" {
        Set-PupDialogHandler -Page $script:page -Action Accept
        $btn = Find-PupElements -Page $script:page -Selector "#btn-alert" -First
        { $btn | Invoke-PupElementClick; Start-Sleep -Milliseconds 200 } | Should -Not -Throw
    }

    It "Dismisses confirm dialogs" {
        Set-PupDialogHandler -Page $script:page -Action Dismiss
        $btn = Find-PupElements -Page $script:page -Selector "#btn-confirm" -First
        { $btn | Invoke-PupElementClick; Start-Sleep -Milliseconds 200 } | Should -Not -Throw
    }
}
