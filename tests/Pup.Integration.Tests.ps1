#Requires -Modules Pester

Describe "Pup Integration Tests" -Tags @("Integration", "Core") {
    BeforeAll {
        # Import the module
        $ModulePath = Join-Path $PSScriptRoot '..' 'src' 'bin' 'Debug' 'netstandard2.0' 'Pup.dll'
        Import-Module $ModulePath -Force
        
        # Test configuration
        $TestUrl = "data:text/html,<html><head><title>Test Page</title></head><body><h1 id='header'>Test Header</h1><p class='content'>First paragraph</p><p class='content'>Second paragraph</p><input id='textInput' type='text' value='initial'><button id='testButton'>Click Me</button><div id='hidden' style='display:none'>Hidden content</div></body></html>"
        $TestUrl2 = "data:text/html,<html><head><title>Second Page</title></head><body><h2>Second Page Content</h2><a href='#' id='link'>Test Link</a></body></html>"
        
        # Global variables for test session
        $script:TestBrowser = $null
        $script:TestPage = $null
    }
    
    AfterAll {
        # Global cleanup
        if ($script:TestBrowser) {
            Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
        }
    }
    
    Context "Browser Management Commands" {
        It "Install-PupBrowser should install browser successfully" {
            { Install-PupBrowser } | Should -Not -Throw
        }
        
        It "Get-PupBrowser should list available browsers" {
            $browsers = Get-PupBrowser
            $browsers | Should -Not -BeNullOrEmpty
            $browsers | Should -BeOfType [Object]
        }
        
        It "Start-PupBrowser should start browser instance" {
            $script:TestBrowser = Start-PupBrowser -Headless
            $script:TestBrowser | Should -Not -BeNullOrEmpty
            $script:TestBrowser.GetType().Name | Should -Be "PupBrowser"
        }
        
        It "Stop-PupBrowser should stop browser instance" {
            $tempBrowser = Start-PupBrowser -Headless
            { Stop-PupBrowser -Browser $tempBrowser } | Should -Not -Throw
        }
        
        It "Uninstall-PupBrowser should support WhatIf and confirmation" {
            # Now supports proper ShouldProcess with -WhatIf and -Confirm
            { Uninstall-PupBrowser -WhatIf } | Should -Not -Throw
            # Test that WhatIf parameter is available
            $cmd = Get-Command Uninstall-PupBrowser
            $cmd.Parameters.ContainsKey('WhatIf') | Should -Be $true
            $cmd.Parameters.ContainsKey('Confirm') | Should -Be $true
        }
    }
    
    Context "Page Management Commands" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "New-PupPage should create new page" {
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url $TestUrl
            $script:TestPage | Should -Not -BeNullOrEmpty
            $script:TestPage.GetType().Name | Should -Be "PupPage"
        }
        
        It "Get-PupPage should list browser pages" {
            $page2 = New-PupPage -Browser $script:TestBrowser -Url $TestUrl2
            $pages = Get-PupPage -Browser $script:TestBrowser
            $pages | Should -Not -BeNullOrEmpty
            $pages.Count | Should -BeGreaterOrEqual 2
        }
        
        It "Move-PupPage should navigate to URL" {
            { Move-PupPage -Page $script:TestPage -Url $TestUrl2 } | Should -Not -Throw
            Start-Sleep -Milliseconds 500 # Allow navigation to complete
        }
        
        It "Remove-PupPage should close page" {
            $tempPage = New-PupPage -Browser $script:TestBrowser -Url "about:blank"
            { Remove-PupPage -Page $tempPage } | Should -Not -Throw
        }
    }
    
    Context "Page Navigation Commands" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url $TestUrl
            Start-Sleep -Milliseconds 500
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "Invoke-PupPageScript should execute JavaScript with auto-wrapping" {
            # Now supports simple expressions without manual arrow function wrapping
            $result = Invoke-PupPageScript -Page $script:TestPage -Script "document.title" -AsString
            $result | Should -Be "Test Page"
        }
        
        It "Invoke-PupPageReload should refresh page" {
            { Invoke-PupPageReload -Page $script:TestPage } | Should -Not -Throw
            Start-Sleep -Milliseconds 500
        }
        
        It "Invoke-PupPageBack and Invoke-PupPageForward should navigate history" {
            # Navigate to create history
            Move-PupPage -Page $script:TestPage -Url $TestUrl2
            Start-Sleep -Milliseconds 500
            
            # Go back
            { Invoke-PupPageBack -Page $script:TestPage } | Should -Not -Throw
            Start-Sleep -Milliseconds 500
            
            # Go forward
            { Invoke-PupPageForward -Page $script:TestPage } | Should -Not -Throw
            Start-Sleep -Milliseconds 500
        }
    }
    
    Context "Element Finding Commands" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url $TestUrl
            Start-Sleep -Milliseconds 500
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "Find-PupElement should find single element by selector" {
            $element = Find-PupElement -Page $script:TestPage -Selector "#header"
            $element | Should -Not -BeNullOrEmpty
            $element.GetType().Name | Should -Be "PupElement"
            $element.TagName.ToLower() | Should -Be "h1"
        }
        
        It "Find-PupElements should find multiple elements by selector" {
            $elements = Find-PupElements -Page $script:TestPage -Selector ".content"
            $elements | Should -Not -BeNullOrEmpty
            $elements.Count | Should -Be 2
            $elements[0].GetType().Name | Should -Be "PupElement"
        }
        
        It "Wait-PupElement should wait for element to appear" {
            # Element already exists, should return immediately
            $element = Wait-PupElement -Page $script:TestPage -Selector "#header" -Timeout 1000
            $element | Should -Not -BeNullOrEmpty
        }
    }
    
    Context "Element Interaction Commands" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url $TestUrl
            Start-Sleep -Milliseconds 500
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "Invoke-PupElementClick should click element" {
            $button = Find-PupElement -Page $script:TestPage -Selector "#testButton"
            { Invoke-PupElementClick -Element $button } | Should -Not -Throw
        }
        
        It "Invoke-PupElementHover should hover over element" {
            $button = Find-PupElement -Page $script:TestPage -Selector "#testButton"
            { Invoke-PupElementHover -Element $button } | Should -Not -Throw
        }
        
        It "Invoke-PupElementFocus should focus element" {
            $inputElement = Find-PupElement -Page $script:TestPage -Selector "#textInput"
            { Invoke-PupElementFocus -Element $inputElement } | Should -Not -Throw
        }
        
        It "Set-PupElement should set element text value" {
            $inputElement = Find-PupElement -Page $script:TestPage -Selector "#textInput"
            { Set-PupElement -Element $inputElement -Text "new value" } | Should -Not -Throw
        }
    }
    
    Context "Element Attribute Commands" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url $TestUrl
            Start-Sleep -Milliseconds 500
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "Get-PupElementAttribute should retrieve element attribute" {
            $inputElement = Find-PupElement -Page $script:TestPage -Selector "#textInput"
            $type = Get-PupElementAttribute -Element $inputElement -Name "type"
            $type | Should -Be "text"
        }
        
        It "Set-PupElementAttribute should set element attribute" {
            $inputElement = Find-PupElement -Page $script:TestPage -Selector "#textInput"
            { Set-PupElementAttribute -Element $inputElement -Name "placeholder" -Value "Enter text here" } | Should -Not -Throw
            
            # Verify the attribute was set
            $placeholder = Get-PupElementAttribute -Element $inputElement -Name "placeholder"
            $placeholder | Should -Be "Enter text here"
        }
    }
    
    Context "Page Cookie Commands" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
            # Navigate to about:blank which has better cookie support than data: URLs
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url "about:blank"
            Start-Sleep -Milliseconds 500
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "Set-PupPageCookie command should have correct parameters" {
            # Test command syntax rather than actual cookie setting since about:blank has limited cookie support
            $cmd = Get-Command Set-PupPageCookie
            $cmd.Parameters.ContainsKey('Name') | Should -Be $true
            $cmd.Parameters.ContainsKey('Value') | Should -Be $true
            $cmd.Parameters.ContainsKey('Domain') | Should -Be $true
            $cmd.Parameters.ContainsKey('Path') | Should -Be $true
        }
        
        It "Get-PupPageCookie should retrieve cookies without error" {
            # Just verify the command works and returns something (even if empty)
            { $cookies = Get-PupPageCookie -Page $script:TestPage } | Should -Not -Throw
        }
    }
    
    Context "Page Screenshot Command" {
        BeforeAll {
            $script:TestBrowser = Start-PupBrowser -Headless
            $script:TestPage = New-PupPage -Browser $script:TestBrowser -Url $TestUrl
            Start-Sleep -Milliseconds 500
        }
        
        AfterAll {
            if ($script:TestBrowser) {
                Stop-PupBrowser -Browser $script:TestBrowser -ErrorAction SilentlyContinue
            }
        }
        
        It "Get-PupPageScreenshot should capture page screenshot" {
            $tempFile = [System.IO.Path]::GetTempFileName() + ".png"
            try {
                { Get-PupPageScreenshot -Page $script:TestPage -FilePath $tempFile } | Should -Not -Throw
                Test-Path $tempFile | Should -Be $true
                (Get-Item $tempFile).Length | Should -BeGreaterThan 0
            }
            finally {
                if (Test-Path $tempFile) {
                    Remove-Item $tempFile -ErrorAction SilentlyContinue
                }
            }
        }
    }
    
    Context "Command Integration Workflow" {
        It "Should support complete browser automation workflow" {
            try {
                # 1. Start browser
                $browser = Start-PupBrowser -Headless
                
                # 2. Create page and navigate
                $page = New-PupPage -Browser $browser -Url $TestUrl
                Start-Sleep -Milliseconds 500
                
                # 3. Find and interact with elements
                $elements = Find-PupElements -Page $page -Selector "p"
                $elements.Count | Should -Be 2
                
                $inputElement = Find-PupElement -Page $page -Selector "#textInput"
                $inputElement | Should -Not -BeNullOrEmpty
                
                # 4. Modify element (now clears by default)
                Set-PupElement -Element $inputElement -Text "automated test"
                
                # 5. Execute JavaScript to verify (now supports simple expressions)
                $value = Invoke-PupPageScript -Page $page -Script "document.getElementById('textInput').value" -AsString
                $value | Should -Be "automated test"
                
                # 6. Take screenshot
                $tempFile = [System.IO.Path]::GetTempFileName() + ".png"
                Get-PupPageScreenshot -Page $page -FilePath $tempFile
                Test-Path $tempFile | Should -Be $true
                
                # 7. Navigate to different page
                Move-PupPage -Page $page -Url $TestUrl2
                Start-Sleep -Milliseconds 500
                
                # 8. Verify navigation worked
                $title = Invoke-PupPageScript -Page $page -Script "document.title" -AsString
                $title | Should -Be "Second Page"
                
                # Cleanup
                Remove-Item $tempFile -ErrorAction SilentlyContinue
                Stop-PupBrowser -Browser $browser
            }
            catch {
                throw $_
            }
        }
    }
}