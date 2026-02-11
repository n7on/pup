BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:wsTestUrl = "file://" + [System.IO.Path]::Combine($PSScriptRoot, "fixtures", "websocket-test.html")
    $script:browser = Start-PupBrowser -Headless
}

AfterAll {
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "WebSocket Capture" -Tag "Network" {
    BeforeAll {
        $script:page = New-PupPage -Browser $script:browser -Url $script:wsTestUrl -WaitForLoad
        # Wait for WebSocket connection and initial message exchange
        Start-Sleep -Seconds 3
    }

    AfterAll {
        if ($script:page.Running) { Remove-PupPage -Page $script:page }
    }

    It "Captures WebSocket connection" {
        $websockets = Get-PupWebSocket -Page $script:page
        $websockets | Should -Not -BeNullOrEmpty
        $websockets.Count | Should -BeGreaterOrEqual 1
    }

    It "Captures WebSocket URL" {
        $websockets = Get-PupWebSocket -Page $script:page
        $ws = $websockets | Where-Object { $_.Url -like "*postman-echo*" }
        $ws | Should -Not -BeNullOrEmpty
        $ws.Url | Should -BeLike "*wss://ws.postman-echo.com*"
    }

    It "Captures WebSocket state and properties" {
        $websockets = Get-PupWebSocket -Page $script:page
        $ws = $websockets | Select-Object -First 1
        $ws.State | Should -BeIn @("connecting", "open", "closed")
        $ws.RequestId | Should -Not -BeNullOrEmpty
        $ws.CreatedTime | Should -Not -BeNullOrEmpty
        $ws.CreatedTime | Should -BeOfType [DateTime]
    }

    It "Captures sent frames" {
        $websockets = Get-PupWebSocket -Page $script:page
        $ws = $websockets | Where-Object { $_.Url -like "*postman-echo*" }
        $sentFrames = $ws.Frames | Where-Object { $_.Direction -eq "sent" }
        $sentFrames | Should -Not -BeNullOrEmpty
        ($sentFrames | Where-Object { $_.PayloadData -like "*hello from pup*" }) | Should -Not -BeNullOrEmpty
    }

    It "Captures received frames" {
        $websockets = Get-PupWebSocket -Page $script:page
        $ws = $websockets | Where-Object { $_.Url -like "*postman-echo*" }
        $receivedFrames = $ws.Frames | Where-Object { $_.Direction -eq "received" }
        # Echo server should echo back the message
        $receivedFrames | Should -Not -BeNullOrEmpty
    }

    It "Frame has timestamp" {
        $websockets = Get-PupWebSocket -Page $script:page
        $ws = $websockets | Select-Object -First 1
        $frame = $ws.Frames | Select-Object -First 1
        $frame.Timestamp | Should -Not -BeNullOrEmpty
        $frame.Timestamp | Should -BeOfType [DateTime]
    }

    It "Frame has opcode" {
        $websockets = Get-PupWebSocket -Page $script:page
        $ws = $websockets | Select-Object -First 1
        $frame = $ws.Frames | Select-Object -First 1
        $frame.Opcode | Should -BeIn @(1, 2)  # 1=text, 2=binary
    }

    It "Filters by URL pattern" {
        $websockets = Get-PupWebSocket -Page $script:page -Url "postman"
        $websockets | Should -Not -BeNullOrEmpty
        $websockets | ForEach-Object { $_.Url | Should -BeLike "*postman*" }
    }

    It "Returns empty for non-matching URL" {
        $websockets = Get-PupWebSocket -Page $script:page -Url "nonexistent-url-pattern"
        $websockets | Should -BeNullOrEmpty
    }

    It "Filters active connections" {
        $activeWs = Get-PupWebSocket -Page $script:page -Active
        $activeWs | ForEach-Object { $_.State | Should -BeIn @("connecting", "open") }
    }
}

Describe "WebSocket Send Message" -Tag "Network" {
    BeforeAll {
        $script:sendPage = New-PupPage -Browser $script:browser -Url $script:wsTestUrl -WaitForLoad
        Start-Sleep -Seconds 3
    }

    AfterAll {
        if ($script:sendPage.Running) { Remove-PupPage -Page $script:sendPage }
    }

    It "Sends message through WebSocket" {
        $initialWs = Get-PupWebSocket -Page $script:sendPage | Where-Object { $_.Url -like "*postman-echo*" }
        $initialFrameCount = $initialWs.Frames.Count

        $result = Send-PupWebSocketMessage -Page $script:sendPage -Message "test from pester"
        $result | Should -BeTrue

        Start-Sleep -Seconds 1

        $updatedWs = Get-PupWebSocket -Page $script:sendPage | Where-Object { $_.Url -like "*postman-echo*" }
        $updatedWs.Frames.Count | Should -BeGreaterThan $initialFrameCount

        $sentFrame = $updatedWs.Frames | Where-Object { $_.Direction -eq "sent" -and $_.PayloadData -like "*test from pester*" }
        $sentFrame | Should -Not -BeNullOrEmpty
    }

    It "Sends message to specific WebSocket by URL" {
        $result = Send-PupWebSocketMessage -Page $script:sendPage -Message "targeted message" -Url "postman"
        $result | Should -BeTrue
    }

    It "Returns false when no matching WebSocket" {
        $result = Send-PupWebSocketMessage -Page $script:sendPage -Message "test" -Url "nonexistent-ws" -WarningAction SilentlyContinue
        $result | Should -BeFalse
    }
}
