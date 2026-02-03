BeforeAll {
    Import-Module "$PSScriptRoot\..\output\Pup\Pup.psd1" -Force
}

Describe "Browser" {
    BeforeAll {
        Install-PupBrowser -BrowserType Chrome
    }

    AfterAll {
        Get-PupBrowser | Where-Object Running | Stop-PupBrowser
    }

    It "Starts in headless mode" {
        $browser = Start-PupBrowser -Headless
        $browser | Should -Not -BeNullOrEmpty
        $browser.Running | Should -BeTrue
        $browser.Headless | Should -BeTrue
        $browser.ProcessId | Should -BeGreaterThan 0
        Stop-PupBrowser -Browser $browser
    }

    It "Starts with custom window size" {
        $browser = Start-PupBrowser -Headless -Width 1920 -Height 1080
        $browser.WindowSize | Should -Be "1920x1080"
        Stop-PupBrowser -Browser $browser
    }

    It "Stops cleanly" {
        $browser = Start-PupBrowser -Headless
        Stop-PupBrowser -Browser $browser
        Start-Sleep -Milliseconds 300
        $browser.Running | Should -BeFalse
    }
}
