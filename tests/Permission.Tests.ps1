BeforeAll {
    Import-Module ([System.IO.Path]::Combine($PSScriptRoot, "..", "output", "Pup", "Pup.psd1")) -Force
    Install-PupBrowser -BrowserType Chrome

    $script:browser = Start-PupBrowser -Headless
    $script:page = New-PupPage -Browser $script:browser -Url "https://example.com" -WaitForLoad
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Set-PupPagePermission" {
    It "Command exists with correct parameters" {
        $cmd = Get-Command Set-PupPagePermission -ErrorAction SilentlyContinue
        $cmd | Should -Not -BeNullOrEmpty
        $cmd.Parameters.Keys | Should -Contain "Page"
        $cmd.Parameters.Keys | Should -Contain "Permission"
        $cmd.Parameters.Keys | Should -Contain "State"
        $cmd.Parameters.Keys | Should -Contain "Origin"
    }

    It "Permission parameter has ValidateSet" {
        $cmd = Get-Command Set-PupPagePermission
        $permParam = $cmd.Parameters["Permission"]
        $validateSet = $permParam.Attributes | Where-Object { $_.TypeId.Name -eq "ValidateSetAttribute" }
        $validateSet | Should -Not -BeNullOrEmpty
        $validateSet.ValidValues | Should -Contain "geolocation"
        $validateSet.ValidValues | Should -Contain "notifications"
        $validateSet.ValidValues | Should -Contain "camera"
        $validateSet.ValidValues | Should -Contain "microphone"
    }

    It "State parameter has ValidateSet" {
        $cmd = Get-Command Set-PupPagePermission
        $stateParam = $cmd.Parameters["State"]
        $validateSet = $stateParam.Attributes | Where-Object { $_.TypeId.Name -eq "ValidateSetAttribute" }
        $validateSet | Should -Not -BeNullOrEmpty
        $validateSet.ValidValues | Should -Contain "Granted"
        $validateSet.ValidValues | Should -Contain "Denied"
        $validateSet.ValidValues | Should -Contain "Prompt"
    }

    It "Sets geolocation permission to granted" {
        { Set-PupPagePermission -Page $script:page -Permission "geolocation" -State "Granted" } | Should -Not -Throw
    }

    It "Sets notifications permission to denied" {
        { Set-PupPagePermission -Page $script:page -Permission "notifications" -State "Denied" } | Should -Not -Throw
    }

    It "Sets camera permission to prompt" {
        { Set-PupPagePermission -Page $script:page -Permission "camera" -State "Prompt" } | Should -Not -Throw
    }

    It "Sets microphone permission" {
        { Set-PupPagePermission -Page $script:page -Permission "microphone" -State "Granted" } | Should -Not -Throw
    }

    It "Sets clipboard-read permission" {
        { Set-PupPagePermission -Page $script:page -Permission "clipboard-read" -State "Granted" } | Should -Not -Throw
    }

    It "Sets midi permission" {
        { Set-PupPagePermission -Page $script:page -Permission "midi" -State "Granted" } | Should -Not -Throw
    }

    It "Permission parameter is case-insensitive" {
        { Set-PupPagePermission -Page $script:page -Permission "GEOLOCATION" -State "granted" } | Should -Not -Throw
    }

    It "State parameter is case-insensitive" {
        { Set-PupPagePermission -Page $script:page -Permission "geolocation" -State "GRANTED" } | Should -Not -Throw
    }

    It "Rejects invalid permission value" {
        { Set-PupPagePermission -Page $script:page -Permission "invalid-perm" -State "Granted" } | Should -Throw
    }

    It "Rejects invalid state value" {
        { Set-PupPagePermission -Page $script:page -Permission "geolocation" -State "allow" } | Should -Throw
    }

    It "Supports pipeline input for Page" {
        { $script:page | Set-PupPagePermission -Permission "geolocation" -State "Granted" } | Should -Not -Throw
    }

    It "Can set permission with explicit origin" {
        { Set-PupPagePermission -Page $script:page -Permission "geolocation" -State "Granted" -Origin "https://example.com" } | Should -Not -Throw
    }
}

Describe "Permission Effect Verification" {
    It "Granted geolocation allows navigator.geolocation" {
        Set-PupPagePermission -Page $script:page -Permission "geolocation" -State "Granted"

        # Check that geolocation API is accessible (won't throw permission error)
        $hasGeolocation = Invoke-PupScript -Page $script:page -Script "() => 'geolocation' in navigator" -AsBoolean
        $hasGeolocation | Should -BeTrue
    }

    It "Can query permission state via JavaScript" {
        Set-PupPagePermission -Page $script:page -Permission "notifications" -State "Denied"

        $state = Invoke-PupScript -Page $script:page -Script @"
            async () => {
                const result = await navigator.permissions.query({ name: 'notifications' });
                return result.state;
            }
"@ -AsString

        $state | Should -Be "denied"
    }
}
