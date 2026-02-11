BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:browser = Start-PupBrowser -Headless
    $script:testUrl = "https://example.com"
    $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Export-PupPageSession" {
    BeforeEach {
        # Set up some session data
        Set-PupPageCookie -Page $script:page -Name "testcookie" -Value "cookievalue" -Domain "example.com"
        Set-PupPageStorage -Page $script:page -Type Local -Key "localkey" -Value "localvalue"
        Set-PupPageStorage -Page $script:page -Type Session -Key "sessionkey" -Value "sessionvalue"
    }

    It "Exports session to object" {
        $session = Export-PupPageSession -Page $script:page

        $session | Should -Not -BeNullOrEmpty
        $session.Url | Should -BeLike "*example.com*"
        $session.ExportedAt | Should -Not -BeNullOrEmpty
        $session.Cookies | Should -Not -BeNullOrEmpty
        $session.LocalStorage | Should -Not -BeNullOrEmpty
        $session.SessionStorage | Should -Not -BeNullOrEmpty
    }

    It "Exports session to file" {
        $path = Join-Path ([IO.Path]::GetTempPath()) "test-session.json"

        Export-PupPageSession -Page $script:page -FilePath $path

        Test-Path $path | Should -BeTrue
        $content = Get-Content $path -Raw | ConvertFrom-Json
        $content.url | Should -BeLike "*example.com*"
        $content.cookies | Should -Not -BeNullOrEmpty

        Remove-Item $path -ErrorAction SilentlyContinue
    }

    It "Exports session to file with PassThru" {
        $path = Join-Path ([IO.Path]::GetTempPath()) "test-session-passthru.json"

        $session = Export-PupPageSession -Page $script:page -FilePath $path -PassThru

        $session | Should -Not -BeNullOrEmpty
        Test-Path $path | Should -BeTrue

        Remove-Item $path -ErrorAction SilentlyContinue
    }
}

Describe "Import-PupPageSession" {
    BeforeAll {
        # Export a session first
        Set-PupPageCookie -Page $script:page -Name "importtest" -Value "importvalue" -Domain "example.com"
        Set-PupPageStorage -Page $script:page -Type Local -Key "importlocal" -Value "localdata"
        Set-PupPageStorage -Page $script:page -Type Session -Key "importsession" -Value "sessiondata"

        $script:exportedSession = Export-PupPageSession -Page $script:page
        $script:sessionFile = Join-Path ([IO.Path]::GetTempPath()) "import-test-session.json"
        Export-PupPageSession -Page $script:page -FilePath $script:sessionFile
    }

    AfterAll {
        Remove-Item $script:sessionFile -ErrorAction SilentlyContinue
    }

    BeforeEach {
        # Clear existing data
        Remove-PupPageCookie -Page $script:page -Name "*" -Domain "example.com"
        Clear-PupPageStorage -Page $script:page -Type Local
        Clear-PupPageStorage -Page $script:page -Type Session
    }

    It "Imports session from object" {
        Import-PupPageSession -Page $script:page -Session $script:exportedSession

        $cookies = Get-PupPageCookie -Page $script:page -Name "importtest"
        $cookies | Should -Not -BeNullOrEmpty

        $local = Get-PupPageStorage -Page $script:page -Type Local -Key "importlocal"
        $local.Value | Should -Be "localdata"
    }

    It "Imports session from file" {
        Import-PupPageSession -Page $script:page -FilePath $script:sessionFile

        $cookies = Get-PupPageCookie -Page $script:page -Name "importtest"
        $cookies | Should -Not -BeNullOrEmpty
    }

    It "Imports session without cookies using NoCookies" {
        Import-PupPageSession -Page $script:page -Session $script:exportedSession -NoCookies

        $cookies = Get-PupPageCookie -Page $script:page -Name "importtest"
        $cookies | Should -BeNullOrEmpty

        $local = Get-PupPageStorage -Page $script:page -Type Local -Key "importlocal"
        $local.Value | Should -Be "localdata"
    }

    It "Imports session without localStorage using NoLocalStorage" {
        Import-PupPageSession -Page $script:page -Session $script:exportedSession -NoLocalStorage

        $local = Get-PupPageStorage -Page $script:page -Type Local -Key "importlocal"
        $local | Should -BeNullOrEmpty

        $session = Get-PupPageStorage -Page $script:page -Type Session -Key "importsession"
        $session.Value | Should -Be "sessiondata"
    }

    It "Imports session without sessionStorage using NoSessionStorage" {
        Import-PupPageSession -Page $script:page -Session $script:exportedSession -NoSessionStorage

        $session = Get-PupPageStorage -Page $script:page -Type Session -Key "importsession"
        $session | Should -BeNullOrEmpty

        $local = Get-PupPageStorage -Page $script:page -Type Local -Key "importlocal"
        $local.Value | Should -Be "localdata"
    }
}
