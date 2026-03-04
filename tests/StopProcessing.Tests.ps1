BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:testUrl = "file://" + [System.IO.Path]::Combine($PSScriptRoot, "fixtures", "test-page.html")
    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url $script:testUrl -WaitForLoad
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Pipeline stopping (Select-Object -First)" {
    It "Stops Find-PupElements pipeline early without errors" {
        $elements = Find-PupElements -Page $script:page -Selector "li"
        $elements.Count | Should -Be 3

        $first = $elements | Get-PupElementAttribute -Name "data-id" | Select-Object -First 1
        $first | Should -Be "1"
    }

    It "Stops Get-PupElementAttribute pipeline at 2 items" {
        $elements = Find-PupElements -Page $script:page -Selector "li"
        $results = $elements | Get-PupElementAttribute -Name "data-id" | Select-Object -First 2
        $results.Count | Should -Be 2
        $results[0] | Should -Be "1"
        $results[1] | Should -Be "2"
    }

    It "Browser session stays healthy after pipeline stop" {
        $elements = Find-PupElements -Page $script:page -Selector "li"
        $null = $elements | Get-PupElementAttribute -Name "data-id" | Select-Object -First 1

        # The page should still be usable after the pipeline was stopped
        $title = Find-PupElements -Page $script:page -Selector "#title" -First
        $title.InnerText | Should -Be "Test Page"
    }

    It "Stops Find-PupElements piped to Get-PupElementValue" {
        # Set a known value first
        $input = Find-PupElements -Page $script:page -Selector "#username" -First
        Set-PupElement -Element $input -Value "testuser"

        $val = Find-PupElements -Page $script:page -Selector "input[type=text]" |
            Get-PupElementValue |
            Select-Object -First 1
        $val | Should -Be "testuser"

        # Page still healthy
        $script:page.Running | Should -BeTrue
    }

    It "Stops Invoke-PupScript pipeline early" {
        $results = 1..5 | ForEach-Object {
            Invoke-PupScript -Page $script:page -Script "() => $_" -AsNumber
        } | Select-Object -First 2
        $results.Count | Should -Be 2

        # Page still healthy
        $el = Find-PupElements -Page $script:page -Selector "#title" -First
        $el | Should -Not -BeNullOrEmpty
    }
}
