BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = "file://" + [System.IO.Path]::Combine($PSScriptRoot, "fixtures", "test-page.html")
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad

    # Create a test file for upload tests
    $script:testFilePath = Join-Path ([IO.Path]::GetTempPath()) "pup-test-upload.txt"
    "Test file content" | Set-Content $script:testFilePath

    $script:testFilePath2 = Join-Path ([IO.Path]::GetTempPath()) "pup-test-upload2.txt"
    "Second test file" | Set-Content $script:testFilePath2
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }

    # Clean up test files
    if (Test-Path $script:testFilePath) { Remove-Item $script:testFilePath }
    if (Test-Path $script:testFilePath2) { Remove-Item $script:testFilePath2 }
}

Describe "File Upload" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Uploads single file" {
        $fileInput = Find-PupElements -Page $script:page -Selector "#file-single" -First
        Send-PupFile -Element $fileInput -FilePath $script:testFilePath

        $fileName = Invoke-PupScript -Page $script:page -Script "() => document.getElementById('file-single').files[0]?.name" -AsString
        $fileName | Should -Be "pup-test-upload.txt"
    }

    It "Uploads multiple files" {
        $fileInput = Find-PupElements -Page $script:page -Selector "#file-multiple" -First
        Send-PupFile -Element $fileInput -FilePath $script:testFilePath, $script:testFilePath2

        $fileCount = Invoke-PupScript -Page $script:page -Script "() => document.getElementById('file-multiple').files.length" -AsNumber
        $fileCount | Should -Be 2
    }

    It "Throws error for non-existent file" {
        $fileInput = Find-PupElements -Page $script:page -Selector "#file-single" -First
        { Send-PupFile -Element $fileInput -FilePath "/nonexistent/file.txt" -ErrorAction Stop } | Should -Throw
    }

    It "Accepts pipeline input" {
        $fileInput = Find-PupElements -Page $script:page -Selector "#file-single" -First
        $fileInput | Send-PupFile -FilePath $script:testFilePath

        $fileName = Invoke-PupScript -Page $script:page -Script "() => document.getElementById('file-single').files[0]?.name" -AsString
        $fileName | Should -Be "pup-test-upload.txt"
    }
}

Describe "Keyboard Input" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    It "Types text" {
        $el = Find-PupElements -Page $script:page -Selector "#username" -First
        $el | Invoke-PupElementFocus
        Send-PupKey -Page $script:page -Text "hello"
        $value = Invoke-PupScript -Page $script:page -Script "() => document.getElementById('username').value" -AsString
        $value | Should -Be "hello"
    }

    It "Sends key with modifier" {
        Send-PupKey -Page $script:page -Key "a" -Modifiers "Control"
        { Send-PupKey -Page $script:page -Key "Backspace" } | Should -Not -Throw
    }
}

Describe "Dialog Handler" {
    BeforeEach {
        Invoke-PupPageReload -Page $script:page -WaitForLoad
    }

    AfterEach {
        Remove-PupPageHandler -Page $script:page -Event Dialog -ErrorAction SilentlyContinue
    }

    It "Accepts alert dialogs" {
        Set-PupPageHandler -Page $script:page -Event Dialog -Action Accept
        $btn = Find-PupElements -Page $script:page -Selector "#btn-alert" -First
        { $btn | Invoke-PupElementClick; Start-Sleep -Milliseconds 200 } | Should -Not -Throw
    }

    It "Dismisses confirm dialogs" {
        Set-PupPageHandler -Page $script:page -Event Dialog -Action Dismiss
        $btn = Find-PupElements -Page $script:page -Selector "#btn-confirm" -First
        { $btn | Invoke-PupElementClick; Start-Sleep -Milliseconds 200 } | Should -Not -Throw
    }
}
