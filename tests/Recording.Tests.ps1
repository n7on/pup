BeforeAll {
    Import-Module (Join-Path $PSScriptRoot "../" "output" "Pup" "Pup.psd1") -Force
}

Describe "Live Recording" {
    BeforeAll {
        Install-PupBrowser -BrowserType Chrome
        $script:browser = Start-PupBrowser -Headless
        $script:testUrl = "file://" + (Join-Path $PSScriptRoot "fixtures" "test-page.html")
        $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
    }

    AfterAll {
        if ($script:page.Running) { Remove-PupPage -Page $script:page }
        if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
    }

    BeforeEach {
        # Clear any existing recordings and stop if active
        if ($script:page.RecordingActive) {
            Stop-PupRecording -Page $script:page | Out-Null
        }
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Starts and stops recording" {
        { Start-PupRecording -Page $script:page } | Should -Not -Throw
        { Stop-PupRecording -Page $script:page } | Should -Not -Throw
    }

    It "Captures click events" {
        Start-PupRecording -Page $script:page

        # Perform a click
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 100

        Stop-PupRecording -Page $script:page
        $events = Get-PupRecording -Page $script:page
        $clickEvents = $events | Where-Object { $_.Type -eq 'click' }
        $clickEvents.Count | Should -BeGreaterThan 0
        Clear-PupRecording -Page $script:page
    }

    It "Captures input events" {
        Start-PupRecording -Page $script:page

        # Type in an input
        $inputEl = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElement -Element $inputEl -Text "testuser" -Clear

        Start-Sleep -Milliseconds 600  # Wait for debounce

        Stop-PupRecording -Page $script:page
        $events = Get-PupRecording -Page $script:page
        $inputEvents = $events | Where-Object { $_.Type -eq 'input' -or $_.Type -eq 'change' }
        $inputEvents.Count | Should -BeGreaterThan 0
        Clear-PupRecording -Page $script:page
    }

    It "Get-PupRecording returns events without stopping" {
        Start-PupRecording -Page $script:page

        # Perform a click
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 100

        # Get events without stopping
        $events = Get-PupRecording -Page $script:page
        $events.Count | Should -BeGreaterThan 0

        # Perform another click to verify recording is still active
        $btn | Invoke-PupElementClick
        Start-Sleep -Milliseconds 100

        # Should have more events now
        $events2 = Get-PupRecording -Page $script:page
        $events2.Count | Should -BeGreaterThan $events.Count

        Stop-PupRecording -Page $script:page
        Clear-PupRecording -Page $script:page
    }

    It "Converts events to PowerShell script" {
        Start-PupRecording -Page $script:page

        # Perform a click
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        $btn | Invoke-PupElementClick

        Start-Sleep -Milliseconds 100

        Stop-PupRecording -Page $script:page
        $events = Get-PupRecording -Page $script:page
        $output = $events | ConvertTo-PupScript
        $output | Should -BeLike "*Find-PupElements*"
        $output | Should -BeLike "*Invoke-PupElementClick*"
        Clear-PupRecording -Page $script:page
    }

    It "Includes setup code when requested" {
        Start-PupRecording -Page $script:page
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        $btn | Invoke-PupElementClick
        Start-Sleep -Milliseconds 100

        Stop-PupRecording -Page $script:page
        $events = Get-PupRecording -Page $script:page
        # URL should be captured automatically from recording start
        $output = $events | ConvertTo-PupScript -IncludeSetup
        $output | Should -BeLike "*Start-PupBrowser*"
        $output | Should -BeLike "*New-PupPage*"
        $output | Should -BeLike "*-Url*"
        $output | Should -BeLike "*-WaitForLoad*"
        Clear-PupRecording -Page $script:page
    }

    It "Uses custom variable names" {
        Start-PupRecording -Page $script:page
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        $btn | Invoke-PupElementClick
        Start-Sleep -Milliseconds 100

        Stop-PupRecording -Page $script:page
        $events = Get-PupRecording -Page $script:page
        $output = $events | ConvertTo-PupScript -PageVariable '$myPage' -IncludeSetup
        $output | Should -BeLike '*$myPage*'
        Clear-PupRecording -Page $script:page
    }

    It "Saves script to output file" {
        Start-PupRecording -Page $script:page
        $btn = Find-PupElements -Page $script:page -Selector "#btn-submit" -First
        $btn | Invoke-PupElementClick
        Start-Sleep -Milliseconds 100

        Stop-PupRecording -Page $script:page
        $outPath = Join-Path ([IO.Path]::GetTempPath()) "live-recording.ps1"
        $events = Get-PupRecording -Page $script:page
        $events | ConvertTo-PupScript -OutputFile $outPath | Out-Null

        Test-Path $outPath | Should -BeTrue
        $content = Get-Content $outPath -Raw
        $content | Should -BeLike "*Invoke-PupElementClick*"

        Remove-Item $outPath -ErrorAction SilentlyContinue
        Clear-PupRecording -Page $script:page
    }
}
