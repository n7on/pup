BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = "file://" + [System.IO.Path]::Combine($PSScriptRoot, "fixtures", "frame-test.html")
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    Start-Sleep -Milliseconds 500  # Give iframes time to load
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Get-PupFrame" {
    It "Gets all child frames" {
        $frames = Get-PupFrame -Page $script:page
        $frames.Count | Should -Be 2
    }

    It "Excludes main frame by default" {
        $frames = Get-PupFrame -Page $script:page
        $frames | Where-Object { $_.IsMainFrame } | Should -BeNullOrEmpty
    }

    It "Includes main frame with -IncludeMain" {
        $frames = Get-PupFrame -Page $script:page -IncludeMain
        $frames.Count | Should -Be 3
        ($frames | Where-Object { $_.IsMainFrame }).Count | Should -Be 1
    }

    It "Filters frames by name" {
        $frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
        $frame | Should -Not -BeNullOrEmpty
        $frame.Name | Should -Be "contentFrame"
    }

    It "Filters frames by URL wildcard" {
        $frames = Get-PupFrame -Page $script:page -Url "*frame-content*"
        $frames.Count | Should -Be 2
    }

    It "Returns first matching frame with -First" {
        $frame = Get-PupFrame -Page $script:page -First
        $frame | Should -Not -BeNullOrEmpty
        $frame.GetType().Name | Should -Be "PupFrame"
    }

    It "Frame has expected properties" {
        $frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
        $frame.Id | Should -Not -BeNullOrEmpty
        $frame.Url | Should -BeLike "*frame-content*"
        $frame.IsMainFrame | Should -BeFalse
        $frame.IsDetached | Should -BeFalse
    }
}

Describe "Find-PupElements with Frame" {
    BeforeAll {
        $script:frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
    }

    It "Finds element by selector in frame" {
        $el = Find-PupElements -Frame $script:frame -Selector "#frame-title" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "H1"
        $el.InnerText | Should -Be "Inside Frame"
    }

    It "Finds multiple elements in frame" {
        $els = Find-PupElements -Frame $script:frame -Selector ".frame-item"
        $els.Count | Should -Be 2
    }

    It "Finds element by XPath in frame" {
        $el = Find-PupElements -Frame $script:frame -Selector "//h1" -XPath -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "H1"
    }

    It "Finds element by text in frame" {
        $el = Find-PupElements -Frame $script:frame -Text "Inside Frame" -First
        $el | Should -Not -BeNullOrEmpty
        $el.TagName | Should -Be "H1"
    }

    It "Finds element by text contains in frame" {
        $el = Find-PupElements -Frame $script:frame -TextContains "inside" -First
        $el | Should -Not -BeNullOrEmpty
    }

    It "Returns null for element not in frame" {
        $el = Find-PupElements -Frame $script:frame -Selector "#main-title" -First
        $el | Should -BeNullOrEmpty
    }

    It "Waits for element in frame with -WaitForLoad" {
        $el = Find-PupElements -Frame $script:frame -Selector "#frame-button" -WaitForLoad -First
        $el | Should -Not -BeNullOrEmpty
    }
}

Describe "Wait-PupElement with Frame" {
    BeforeAll {
        $script:frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
    }

    It "Waits for existing element in frame" {
        { Wait-PupElement -Frame $script:frame -Selector "#frame-title" -Timeout 5000 } | Should -Not -Throw
    }

    It "Returns element with -PassThru" {
        $el = Wait-PupElement -Frame $script:frame -Selector "#frame-title" -PassThru
        $el | Should -Not -BeNullOrEmpty
        $el.InnerText | Should -Be "Inside Frame"
    }

    It "Waits for element to be visible in frame" {
        { Wait-PupElement -Frame $script:frame -Selector "#frame-button" -Visible -Timeout 5000 } | Should -Not -Throw
    }

    It "Errors for unsupported frame wait conditions" {
        Wait-PupElement -Frame $script:frame -Selector "#frame-button" -Enabled -Timeout 1000 -ErrorVariable err -ErrorAction SilentlyContinue
        $err.Count | Should -BeGreaterThan 0
        $err[0].Exception.Message | Should -BeLike "*does not support*"
    }
}

Describe "Invoke-PupScript with Frame" {
    BeforeAll {
        $script:frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
    }

    It "Executes script in frame context returning string" {
        $title = Invoke-PupScript -Frame $script:frame -Script "() => document.title" -AsString
        $title | Should -Be "Frame Content"
    }

    It "Executes script in frame context returning number" {
        $count = Invoke-PupScript -Frame $script:frame -Script "() => document.querySelectorAll('.frame-item').length" -AsNumber
        $count | Should -Be 2
    }

    It "Executes script in frame context returning boolean" {
        $exists = Invoke-PupScript -Frame $script:frame -Script "() => !!document.getElementById('frame-title')" -AsBoolean
        $exists | Should -BeTrue
    }

    It "Auto-wraps simple expressions" {
        $title = Invoke-PupScript -Frame $script:frame -Script "document.title" -AsString
        $title | Should -Be "Frame Content"
    }

    It "Script sees frame DOM, not main page DOM" {
        $mainTitle = Invoke-PupScript -Frame $script:frame -Script "() => document.getElementById('main-title')"
        $mainTitle | Should -BeNullOrEmpty
    }
}

Describe "Get-PupSource with Frame" {
    BeforeAll {
        $script:frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
    }

    It "Gets HTML source from frame" {
        $html = Get-PupSource -Frame $script:frame
        $html | Should -BeLike "*Inside Frame*"
        $html | Should -BeLike "*frame-title*"
    }

    It "Frame source differs from main page source" {
        $frameHtml = Get-PupSource -Frame $script:frame
        $pageHtml = Get-PupSource -Page $script:page

        $frameHtml | Should -BeLike "*Frame Content*"
        $pageHtml | Should -BeLike "*Frame Test Page*"
        $frameHtml | Should -Not -BeLike "*Main Page*"
    }

    It "Saves frame source to file" {
        $path = Join-Path ([IO.Path]::GetTempPath()) "frame-source.html"
        Get-PupSource -Frame $script:frame -FilePath $path
        Test-Path $path | Should -BeTrue
        $content = Get-Content $path -Raw
        $content | Should -BeLike "*Inside Frame*"
        Remove-Item $path
    }
}

Describe "Pipeline Support" {
    It "Supports pipeline input for Get-PupFrame" {
        $frame = $script:page | Get-PupFrame -First
        $frame | Should -Not -BeNullOrEmpty
    }

    It "Supports pipeline for Find-PupElements with Frame" {
        $frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
        $el = $frame | Find-PupElements -Selector "#frame-title" -First
        $el | Should -Not -BeNullOrEmpty
    }

    It "Supports pipeline for Wait-PupElement with Frame" {
        $frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
        $el = $frame | Wait-PupElement -Selector "#frame-title" -PassThru
        $el | Should -Not -BeNullOrEmpty
    }

    It "Supports pipeline for Invoke-PupScript with Frame" {
        $frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
        $title = $frame | Invoke-PupScript -Script "document.title" -AsString
        $title | Should -Be "Frame Content"
    }

    It "Supports pipeline for Get-PupSource with Frame" {
        $frame = Get-PupFrame -Page $script:page -Name "contentFrame" -First
        $html = $frame | Get-PupSource
        $html | Should -BeLike "*Inside Frame*"
    }
}
