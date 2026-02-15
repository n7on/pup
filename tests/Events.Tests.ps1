BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = "file://" + [System.IO.Path]::Combine($PSScriptRoot, "fixtures", "popup-test.html")
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
}

AfterAll {
    Remove-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ErrorAction SilentlyContinue
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Set-PupBrowserHandler" {
    BeforeEach {
        # Remove handler before each test
        Remove-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ErrorAction SilentlyContinue

        # Close any extra pages
        $pages = Get-PupPage -Browser $script:browser
        foreach ($p in $pages) {
            if ($p.Url -notlike "*popup-test.html*") {
                Remove-PupPage -Page $p -ErrorAction SilentlyContinue
            }
        }
        Move-PupPage -Page $script:page -Url $script:testUrl -WaitForLoad
    }

    It "Command exists with correct parameters" {
        $cmd = Get-Command Set-PupBrowserHandler -ErrorAction SilentlyContinue
        $cmd | Should -Not -BeNullOrEmpty
        $cmd.Parameters.Keys | Should -Contain "Browser"
        $cmd.Parameters.Keys | Should -Contain "Event"
        $cmd.Parameters.Keys | Should -Contain "Action"
        $cmd.Parameters.Keys | Should -Contain "ScriptBlock"
    }

    It "Captures popup with ScriptBlock handler" {
        # Use $global: for cross-thread access from event handler
        $global:capturedPages = [System.Collections.ArrayList]@()
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ScriptBlock {
            param($e)
            [void]$global:capturedPages.Add($e.Page)
        }

        $btn = Find-PupElements -Page $script:page -Selector "#btn-popup" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 500

        $global:capturedPages.Count | Should -Be 1
        $global:capturedPages[0].Url | Should -BeLike "*popup-content*"
    }

    It "Captures multiple popups with ScriptBlock" {
        $global:capturedPages = [System.Collections.ArrayList]@()
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ScriptBlock {
            param($e)
            [void]$global:capturedPages.Add($e.Page)
        }

        $btn1 = Find-PupElements -Page $script:page -Selector "#btn-popup" -First
        $btn1 | Invoke-PupElementClick
        Start-Sleep -Milliseconds 300

        Move-PupPage -Page $script:page -Url $script:testUrl -WaitForLoad
        $btn2 = Find-PupElements -Page $script:page -Selector "#btn-new-tab" -First
        $btn2 | Invoke-PupElementClick
        Start-Sleep -Milliseconds 300

        $global:capturedPages.Count | Should -Be 2
    }

    It "Dismisses popup with Action Dismiss" {
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -Action Dismiss

        $btn = Find-PupElements -Page $script:page -Selector "#btn-popup" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 500

        # Popup should be closed
        $pages = Get-PupPage -Browser $script:browser
        $popupPages = $pages | Where-Object { $_.Url -like "*popup-content*" }
        $popupPages.Count | Should -Be 0
    }

    It "Executes ScriptBlock with event data" {
        $global:capturedUrls = [System.Collections.ArrayList]@()
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ScriptBlock {
            param($e)
            [void]$global:capturedUrls.Add($e.Url)
        }

        $btn = Find-PupElements -Page $script:page -Selector "#btn-popup" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 500

        $global:capturedUrls.Count | Should -Be 1
        $global:capturedUrls[0] | Should -BeLike "*popup-content*"
    }

    It "Only triggers for true popups (with opener)" {
        $global:capturedPages = [System.Collections.ArrayList]@()
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ScriptBlock {
            param($e)
            [void]$global:capturedPages.Add($e.Page)
        }

        # Create a page programmatically (no opener)
        $newPage = New-PupPage -Browser $script:browser
        Start-Sleep -Milliseconds 200

        # Should not be captured (no opener)
        $global:capturedPages.Count | Should -Be 0

        Remove-PupPage -Page $newPage
    }
}

Describe "Get-PupBrowserHandler" {
    BeforeEach {
        Remove-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ErrorAction SilentlyContinue
    }

    It "Returns empty when no handlers set" {
        $handlers = Get-PupBrowserHandler -Browser $script:browser
        $handlers | Should -BeNullOrEmpty
    }

    It "Returns handler info after setting" {
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -Action Dismiss

        $handlers = Get-PupBrowserHandler -Browser $script:browser
        $handlers | Should -Not -BeNullOrEmpty
        $handlers.Event | Should -Be "PopupCreated"
        $handlers.Action | Should -Be "Dismiss"
    }

    It "Filters by event type" {
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -Action Dismiss

        $handlers = Get-PupBrowserHandler -Browser $script:browser -Event PageCreated
        $handlers | Should -BeNullOrEmpty

        $handlers = Get-PupBrowserHandler -Browser $script:browser -Event PopupCreated
        $handlers | Should -Not -BeNullOrEmpty
    }
}

Describe "Remove-PupBrowserHandler" {
    It "Removes handler" {
        $global:capturedPages = [System.Collections.ArrayList]@()
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ScriptBlock {
            param($e)
            [void]$global:capturedPages.Add($e.Page)
        }
        Remove-PupBrowserHandler -Browser $script:browser -Event PopupCreated

        Move-PupPage -Page $script:page -Url $script:testUrl -WaitForLoad
        $btn = Find-PupElements -Page $script:page -Selector "#btn-popup" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 500

        # Should not be captured (handler removed)
        $global:capturedPages.Count | Should -Be 0

        # Cleanup popup
        $pages = Get-PupPage -Browser $script:browser
        $popupPages = $pages | Where-Object { $_.Url -like "*popup-content*" }
        $popupPages | ForEach-Object { Remove-PupPage -Page $_ -ErrorAction SilentlyContinue }
    }

    It "Does not throw when removing non-existent handler" {
        { Remove-PupBrowserHandler -Browser $script:browser -Event Disconnected } | Should -Not -Throw
    }
}

Describe "Browser Event Handlers" {
    It "Supports PageCreated and Disconnected events" {
        # Verify these events are valid enum values
        $cmd = Get-Command Set-PupBrowserHandler -ErrorAction SilentlyContinue
        $cmd | Should -Not -BeNullOrEmpty

        # Set a PageCreated handler (should not throw)
        { Set-PupBrowserHandler -Browser $script:browser -Event PageCreated -Action Ignore } | Should -Not -Throw
        Remove-PupBrowserHandler -Browser $script:browser -Event PageCreated

        # PageCreated and PopupCreated both use TargetCreated - the distinction is
        # that PopupCreated only fires for pages with an opener (true popups)
        # while PageCreated fires for all new page targets
    }
}

Describe "Multi-Page Workflow with Handler" {
    BeforeEach {
        # Clean up any leftover handlers
        Remove-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ErrorAction SilentlyContinue
        # Close extra pages
        $pages = Get-PupPage -Browser $script:browser
        foreach ($p in $pages) {
            if ($p.Url -notlike "*popup-test.html*") {
                Remove-PupPage -Page $p -ErrorAction SilentlyContinue
            }
        }
    }

    It "Simulates OAuth-like flow with handler" {
        # Setup: create fresh page and handler
        $page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
        $global:capturedPopup = $null
        Set-PupBrowserHandler -Browser $script:browser -Event PopupCreated -ScriptBlock {
            param($e)
            $global:capturedPopup = $e.Page
        }

        # 1. Click button that opens popup
        $btn = Find-PupElements -Page $page -Selector "#btn-popup" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 500

        # 2. Get captured popup
        $global:capturedPopup | Should -Not -BeNullOrEmpty
        $popup = $global:capturedPopup

        # 3. Interact with popup
        $input = Find-PupElements -Page $popup -Selector "#popup-input" -First
        $input | Should -Not -BeNullOrEmpty
        Set-PupElement -Element $input -Text "oauth-token-123"

        $value = Get-PupElementValue -Element $input
        $value | Should -Be "oauth-token-123"

        # 4. Close popup
        Remove-PupPage -Page $popup

        # 5. Main page still works
        $title = Find-PupElements -Page $page -Selector "#main-title" -First
        $title.InnerText | Should -Be "Popup Test"

        # Cleanup
        Remove-PupBrowserHandler -Browser $script:browser -Event PopupCreated
        Remove-PupPage -Page $page
    }
}
