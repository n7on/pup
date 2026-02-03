BeforeAll {
    Import-Module "$PSScriptRoot\..\output\Pup\Pup.psd1" -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = Join-Path $PSScriptRoot "fixtures" "test-page.html"
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Find Elements" {
    It "Finds by ID" {
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "H1"
    }

    It "Finds by class" {
        $els = Find-PupElements -Page $script:page -Selector ".item"
        $els.Count | Should -Be 3
    }

    It "Finds by XPath" {
        $el = Find-PupElements -Page $script:page -Selector "//h1" -XPath -First
        $el.TagName | Should -Be "H1"
    }

    It "Returns null for non-existent" {
        $el = Find-PupElements -Page $script:page -Selector "#nope" -First
        $el | Should -BeNullOrEmpty
    }

    It "Finds within parent element" {
        $list = Find-PupElements -Page $script:page -Selector "#items" -First
        $items = Find-PupElements -Element $list -Selector "li"
        $items.Count | Should -Be 3
    }
}

Describe "Element Properties" {
    It "Has InnerText" {
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        $el.InnerText | Should -Be "Test Page"
    }

    It "Has Id" {
        $el = Find-PupElements -Page $script:page -Selector "h1" -First
        $el.Id | Should -Be "title"
    }
}

Describe "Element Attributes" {
    It "Gets attribute" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        $val = Get-PupElementAttribute -Element $el -Name "placeholder"
        $val | Should -Be "Username"
    }

    It "Gets data attribute" {
        $el = Find-PupElements -Page $script:page -Selector ".item" -First
        $val = Get-PupElementAttribute -Element $el -Name "data-id"
        $val | Should -Be "1"
    }

    It "Sets attribute" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElementAttribute -Element $el -Name "data-test" -Value "xyz"
        $val = Get-PupElementAttribute -Element $el -Name "data-test"
        $val | Should -Be "xyz"
    }
}

Describe "Element Interaction" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Types text" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElement -Element $el -Text "testuser"
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('username').value" -AsString
        $val | Should -Be "testuser"
    }

    It "Clears and types" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElement -Element $el -Text "first"
        Set-PupElement -Element $el -Text "second" -Clear
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('username').value" -AsString
        $val | Should -Be "second"
    }

    It "Sets value directly" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElement -Element $el -Value "direct"
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('username').value" -AsString
        $val | Should -Be "direct"
    }

    It "Focuses element" {
        $el = Find-PupElements -Page $script:page -Selector "#password" -First
        $el | Invoke-PupElementFocus
        $activeId = Invoke-PupPageScript -Page $script:page -Script "() => document.activeElement.id" -AsString
        $activeId | Should -Be "password"
    }

    It "Hovers element" {
        $el = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        { $el | Invoke-PupElementHover } | Should -Not -Throw
    }
}

Describe "Wait Element" {
    It "Waits for existing element" {
        { Wait-PupElement -Page $script:page -Selector "#title" -Timeout 5000 } | Should -Not -Throw
    }

    It "Returns element with PassThru" {
        $el = Wait-PupElement -Page $script:page -Selector "#title" -PassThru
        $el | Should -Not -BeNullOrEmpty
    }
}

Describe "Select Element" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Lists options" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        $opts = Select-PupOption -Element $sel -List
        $opts.Count | Should -Be 4
    }

    It "Selects by value" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        Select-PupOption -Element $sel -Value "uk"
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('country').value" -AsString
        $val | Should -Be "uk"
    }

    It "Selects by text" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        Select-PupOption -Element $sel -Text "Canada"
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('country').value" -AsString
        $val | Should -Be "ca"
    }

    It "Selects by index" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        Select-PupOption -Element $sel -Index 1
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('country').value" -AsString
        $val | Should -Be "us"
    }

    It "Multi-selects" {
        $sel = Find-PupElements -Page $script:page -Selector "#colors" -First
        Select-PupOption -Element $sel -Value "red", "blue"
        $vals = Invoke-PupPageScript -Page $script:page -Script "() => Array.from(document.getElementById('colors').selectedOptions).map(o => o.value).join(',')" -AsString
        $vals | Should -BeLike "*red*"
        $vals | Should -BeLike "*blue*"
    }
}

Describe "Element Selector" {
    It "Gets selector for element" {
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        $selector = Get-PupElementSelector -Element $el
        $selector | Should -BeLike "*title*"
    }
}

Describe "Element Screenshot" {
    It "Takes screenshot as bytes" {
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        $bytes = Get-PupElementScreenshot -Element $el -PassThru
        $bytes.Length | Should -BeGreaterThan 100
    }

    It "Saves screenshot to file" {
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        $path = Join-Path ([IO.Path]::GetTempPath()) "element-screenshot.png"
        Get-PupElementScreenshot -Element $el -FilePath $path
        Test-Path $path | Should -BeTrue
        Remove-Item $path
    }
}
