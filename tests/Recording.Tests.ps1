BeforeAll {
    Import-Module (Join-Path $PSScriptRoot "../" "output" "Pup" "Pup.psd1") -Force
    $script:testRecording = Join-Path $PSScriptRoot "fixtures" "test-recording.json"
}

Describe "Convert-PupRecording" {
    It "Converts recording from file" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -Not -BeNullOrEmpty
        $script | Should -BeLike "*Login Flow Test*"
    }

    It "Includes navigate command" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Move-PupPage*"
        $script | Should -BeLike "*example.com/login*"
    }

    It "Includes click commands" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Invoke-PupElementClick*"
    }

    It "Includes change/input commands" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Set-PupElement*"
        $script | Should -BeLike "*admin*"
    }

    It "Includes viewport command" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Set-PupPageViewport*"
        $script | Should -BeLike "*1920*"
    }

    It "Includes wait command" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Wait-PupElement*"
        $script | Should -BeLike "*-Visible*"
    }

    It "Includes double-click command" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*-DoubleClick*"
    }

    It "Includes hover command" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Invoke-PupElementHover*"
    }

    It "Includes key command" {
        $script = Convert-PupRecording -InputFile $script:testRecording

        $script | Should -BeLike "*Send-PupKey*"
        $script | Should -BeLike "*Escape*"
    }

    It "Includes setup when requested" {
        $script = Convert-PupRecording -InputFile $script:testRecording -IncludeSetup

        $script | Should -BeLike "*Start-PupBrowser*"
        $script | Should -BeLike "*New-PupPage*"
    }

    It "Includes teardown when requested" {
        $script = Convert-PupRecording -InputFile $script:testRecording -IncludeTeardown

        $script | Should -BeLike "*Remove-PupPage*"
        $script | Should -BeLike "*Stop-PupBrowser*"
    }

    It "Uses custom variable names" {
        $script = Convert-PupRecording -InputFile $script:testRecording -PageVariable '$p' -BrowserVariable '$b' -IncludeSetup

        $script | Should -BeLike '*$b = Start-PupBrowser*'
        $script | Should -BeLike '*$p = New-PupPage*'
    }

    It "Saves to output file" {
        $outPath = Join-Path ([IO.Path]::GetTempPath()) "converted-recording.ps1"

        $result = Convert-PupRecording -InputFile $script:testRecording -OutputFile $outPath

        Test-Path $outPath | Should -BeTrue
        $content = Get-Content $outPath -Raw
        $content | Should -BeLike "*Login Flow Test*"

        Remove-Item $outPath -ErrorAction SilentlyContinue
    }

    It "Converts from JSON string" {
        $json = Get-Content $script:testRecording -Raw
        $script = Convert-PupRecording -Json $json

        $script | Should -BeLike "*Login Flow Test*"
    }
}
