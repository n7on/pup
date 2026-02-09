BeforeAll {
    Import-Module (Join-Path $PSScriptRoot "../" "output" "Pup" "Pup.psd1") -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = "file://" + (Join-Path $PSScriptRoot "fixtures" "test-page.html")
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

Describe "Element Value Helpers" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Gets value from input" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElementValue -Element $el -Value "quick"
        Get-PupElementValue -Element $el | Should -Be "quick"
    }

    It "Sets checkbox checked/unchecked" {
        $el = Find-PupElements -Page $script:page -Selector "#subscribe" -First
        Set-PupElementValue -Element $el -Uncheck
        Get-PupElementValue -Element $el | Should -BeFalse

        Set-PupElementValue -Element $el -Check
        Get-PupElementValue -Element $el | Should -BeTrue
    }

    It "Sets multi-select values" {
        $el = Find-PupElements -Page $script:page -Selector "#colors" -First
        Set-PupElementValue -Element $el -Values "red","blue"
        $vals = Get-PupElementValue -Element $el
        $vals | Should -Contain "red"
        $vals | Should -Contain "blue"
    }

    It "Switches radio option" {
        $pro = Find-PupElements -Page $script:page -Selector "#plan-pro" -First
        $free = Find-PupElements -Page $script:page -Selector "#plan-free" -First

        Set-PupElementValue -Element $pro -Check

        Get-PupElementValue -Element $pro | Should -BeTrue
        Get-PupElementValue -Element $free | Should -BeFalse
    }
}

Describe "Wait Element" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Waits for existing element" {
        { Wait-PupElement -Page $script:page -Selector "#title" -Timeout 5000 } | Should -Not -Throw
    }

    It "Returns element with PassThru" {
        $el = Wait-PupElement -Page $script:page -Selector "#title" -PassThru
        $el | Should -Not -BeNullOrEmpty
    }

    It "Waits for element to become visible" {
        Invoke-PupPageScript -Page $script:page -Script "() => { document.getElementById('status').style.display='none'; setTimeout(function(){ document.getElementById('status').style.display='block';}, 100); }" -AsVoid
        { Wait-PupElement -Page $script:page -Selector "#status" -Visible -Timeout 2000 -PollingInterval 50 } | Should -Not -Throw
    }

    It "Waits for element to become enabled" {
        Invoke-PupPageScript -Page $script:page -Script "() => { setTimeout(function(){ document.getElementById('delayed-btn').disabled=false;}, 100); }" -AsVoid
        { Wait-PupElement -Page $script:page -Selector "#delayed-btn" -Enabled -Timeout 2000 -PollingInterval 50 } | Should -Not -Throw
    }

    It "Waits for element to become disabled" {
        Invoke-PupPageScript -Page $script:page -Script "() => { setTimeout(function(){ document.getElementById('toggle-btn').disabled=true;}, 100); }" -AsVoid
        { Wait-PupElement -Page $script:page -Selector "#toggle-btn" -Disabled -Timeout 2000 -PollingInterval 50 } | Should -Not -Throw
    }

    It "Waits for text to appear" {
        Invoke-PupPageScript -Page $script:page -Script "() => { setTimeout(function(){ document.getElementById('status').innerText='ready';}, 100); }" -AsVoid
        $el = Wait-PupElement -Page $script:page -Selector "#status" -TextContains "ready" -Timeout 2000 -PollingInterval 50 -PassThru
        $el | Should -Not -BeNullOrEmpty
    }

    It "Waits for attribute value" {
        Invoke-PupPageScript -Page $script:page -Script "() => { setTimeout(function(){ document.getElementById('attr-target').setAttribute('data-state','ready');}, 100); }" -AsVoid
        { Wait-PupElement -Page $script:page -Selector "#attr-target" -AttributeName "data-state" -AttributeValue "ready" -Timeout 2000 -PollingInterval 50 } | Should -Not -Throw
    }
}

Describe "Select Element" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Lists options" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        $opts = Select-PupElementOption -Element $sel -List
        $opts.Count | Should -Be 4
    }

    It "Selects by value" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        Select-PupElementOption -Element $sel -Value "uk"
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('country').value" -AsString
        $val | Should -Be "uk"
    }

    It "Selects by text" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        Select-PupElementOption -Element $sel -Text "Canada"
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('country').value" -AsString
        $val | Should -Be "ca"
    }

    It "Selects by index" {
        $sel = Find-PupElements -Page $script:page -Selector "#country" -First
        Select-PupElementOption -Element $sel -Index 1
        $val = Invoke-PupPageScript -Page $script:page -Script "() => document.getElementById('country').value" -AsString
        $val | Should -Be "us"
    }

    It "Multi-selects" {
        $sel = Find-PupElements -Page $script:page -Selector "#colors" -First
        Select-PupElementOption -Element $sel -Value "red", "blue"
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

Describe "Element Click Options" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Performs double-click with DoubleClick switch" {
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        { $btn | Invoke-PupElementClick -DoubleClick } | Should -Not -Throw
    }

    It "Performs double-click with ClickCount" {
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        { $btn | Invoke-PupElementClick -ClickCount 2 } | Should -Not -Throw
    }
}

Describe "Element Scroll" {
    It "Scrolls element into view" {
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        { $el | Invoke-PupElementScroll } | Should -Not -Throw
    }
}

Describe "Page Viewport" {
    It "Sets viewport size" {
        { Set-PupPageViewport -Page $script:page -Width 1920 -Height 1080 } | Should -Not -Throw
    }

    It "Sets mobile viewport" {
        { Set-PupPageViewport -Page $script:page -Width 375 -Height 667 -IsMobile -HasTouch } | Should -Not -Throw
    }
}

Describe "Find Elements by Text" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Finds element by exact text" {
        $el = Find-PupElements -Page $script:page -Text "Test Page" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "H1"
    }

    It "Finds element by text contains (case-insensitive)" {
        $el = Find-PupElements -Page $script:page -Selector "h1" -TextContains "test page" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "H1"
    }

    It "Finds button by text" {
        $el = Find-PupElements -Page $script:page -Text "Submit" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "BUTTON"
    }

    It "Finds multiple elements containing text" {
        $els = Find-PupElements -Page $script:page -TextContains "Item"
        $els.Count | Should -Be 3
    }

    It "Combines text search with selector" {
        $el = Find-PupElements -Page $script:page -Selector "button" -TextContains "Submit" -First
        $el | Should -Not -BeNullOrEmpty
        $el.Id | Should -Be "btn-submit"
    }

    It "Returns most specific element (not parent)" {
        # "Item 1" appears in the li, not the parent ul
        $el = Find-PupElements -Page $script:page -Text "Item 1" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "LI"
    }

    It "Returns empty for non-matching text" {
        $el = Find-PupElements -Page $script:page -Text "NonExistentText12345" -First
        $el | Should -BeNullOrEmpty
    }

    It "Exact match does not match partial text" {
        $el = Find-PupElements -Page $script:page -Text "Item" -First
        $el | Should -BeNullOrEmpty
    }

    It "Finds text within parent element" {
        $list = Find-PupElements -Page $script:page -Selector "#items" -First
        $item = Find-PupElements -Element $list -Text "Item 2" -First
        $item | Should -Not -BeNullOrEmpty
        $item.TagName | Should -Be "LI"
    }

    It "Text search within element respects scope" {
        $form = Find-PupElements -Page $script:page -Selector "#form" -First
        # "Item 1" is outside the form, should not be found
        $el = Find-PupElements -Element $form -Text "Item 1" -First
        $el | Should -BeNullOrEmpty
    }
}
