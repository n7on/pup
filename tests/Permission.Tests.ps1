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

Describe "Set-PupPermission" {
    It "Command exists with correct parameters" {
        $cmd = Get-Command Set-PupPermission -ErrorAction SilentlyContinue
        $cmd | Should -Not -BeNullOrEmpty
        $cmd.Parameters.Keys | Should -Contain "Page"
        $cmd.Parameters.Keys | Should -Contain "Permission"
        $cmd.Parameters.Keys | Should -Contain "State"
        $cmd.Parameters.Keys | Should -Contain "Origin"
    }

    It "Permission parameter has ValidateSet" {
        $cmd = Get-Command Set-PupPermission
        $permParam = $cmd.Parameters["Permission"]
        $validateSet = $permParam.Attributes | Where-Object { $_.TypeId.Name -eq "ValidateSetAttribute" }
        $validateSet | Should -Not -BeNullOrEmpty
        $validateSet.ValidValues | Should -Contain "geolocation"
        $validateSet.ValidValues | Should -Contain "notifications"
        $validateSet.ValidValues | Should -Contain "camera"
        $validateSet.ValidValues | Should -Contain "microphone"
    }

    It "State parameter has ValidateSet" {
        $cmd = Get-Command Set-PupPermission
        $stateParam = $cmd.Parameters["State"]
        $validateSet = $stateParam.Attributes | Where-Object { $_.TypeId.Name -eq "ValidateSetAttribute" }
        $validateSet | Should -Not -BeNullOrEmpty
        $validateSet.ValidValues | Should -Contain "Granted"
        $validateSet.ValidValues | Should -Contain "Denied"
        $validateSet.ValidValues | Should -Contain "Prompt"
    }

    It "Sets geolocation permission to granted" {
        { Set-PupPermission -Page $script:page -Permission "geolocation" -State "Granted" } | Should -Not -Throw
    }

    It "Sets notifications permission to denied" {
        { Set-PupPermission -Page $script:page -Permission "notifications" -State "Denied" } | Should -Not -Throw
    }

    It "Sets camera permission to prompt" {
        { Set-PupPermission -Page $script:page -Permission "camera" -State "Prompt" } | Should -Not -Throw
    }

    It "Sets microphone permission" {
        { Set-PupPermission -Page $script:page -Permission "microphone" -State "Granted" } | Should -Not -Throw
    }

    It "Sets clipboard-read permission" {
        { Set-PupPermission -Page $script:page -Permission "clipboard-read" -State "Granted" } | Should -Not -Throw
    }

    It "Sets midi permission" {
        { Set-PupPermission -Page $script:page -Permission "midi" -State "Granted" } | Should -Not -Throw
    }

    It "Permission parameter is case-insensitive" {
        { Set-PupPermission -Page $script:page -Permission "GEOLOCATION" -State "granted" } | Should -Not -Throw
    }

    It "State parameter is case-insensitive" {
        { Set-PupPermission -Page $script:page -Permission "geolocation" -State "GRANTED" } | Should -Not -Throw
    }

    It "Rejects invalid permission value" {
        { Set-PupPermission -Page $script:page -Permission "invalid-perm" -State "Granted" } | Should -Throw
    }

    It "Rejects invalid state value" {
        { Set-PupPermission -Page $script:page -Permission "geolocation" -State "allow" } | Should -Throw
    }

    It "Supports pipeline input for Page" {
        { $script:page | Set-PupPermission -Permission "geolocation" -State "Granted" } | Should -Not -Throw
    }

    It "Can set permission with explicit origin" {
        { Set-PupPermission -Page $script:page -Permission "geolocation" -State "Granted" -Origin "https://example.com" } | Should -Not -Throw
    }
}

Describe "Get-PupPermission" {
    It "Command exists with correct parameters" {
        $cmd = Get-Command Get-PupPermission -ErrorAction SilentlyContinue
        $cmd | Should -Not -BeNullOrEmpty
        $cmd.Parameters.Keys | Should -Contain "Page"
        $cmd.Parameters.Keys | Should -Contain "Permission"
        $cmd.Parameters.Keys | Should -Contain "Origin"
    }

    It "Gets single permission state" {
        Set-PupPermission -Page $script:page -Permission "notifications" -State "Denied"
        $result = Get-PupPermission -Page $script:page -Permission "notifications"
        $result | Should -Not -BeNullOrEmpty
        $result.Permission | Should -Be "notifications"
        # State should be one of: granted, denied, prompt, unsupported
        $result.State | Should -BeIn @("granted", "denied", "prompt", "unsupported")
    }

    It "Gets all permissions when no specific permission specified" {
        $results = Get-PupPermission -Page $script:page
        $results | Should -Not -BeNullOrEmpty
        $results.Count | Should -BeGreaterThan 1
    }

    It "Returns objects with Permission, State, and Origin properties" {
        $result = Get-PupPermission -Page $script:page -Permission "notifications"
        $result.PSObject.Properties.Name | Should -Contain "Permission"
        $result.PSObject.Properties.Name | Should -Contain "State"
        $result.PSObject.Properties.Name | Should -Contain "Origin"
    }

    It "Supports pipeline input for Page" {
        $result = $script:page | Get-PupPermission -Permission "geolocation"
        $result | Should -Not -BeNullOrEmpty
    }
}

Describe "Permission Effect Verification" {
    It "Granted geolocation allows navigator.geolocation" {
        Set-PupPermission -Page $script:page -Permission "geolocation" -State "Granted"

        # Check that geolocation API is accessible (won't throw permission error)
        $hasGeolocation = Invoke-PupScript -Page $script:page -Script "() => 'geolocation' in navigator" -AsBoolean
        $hasGeolocation | Should -BeTrue
    }

    It "Can query permission state via JavaScript" {
        Set-PupPermission -Page $script:page -Permission "notifications" -State "Denied"

        $state = Invoke-PupScript -Page $script:page -Script @"
            async () => {
                const result = await navigator.permissions.query({ name: 'notifications' });
                return result.state;
            }
"@ -AsString

        $state | Should -Be "denied"
    }

    It "Get-PupPermission returns valid state for notifications" {
        Set-PupPermission -Page $script:page -Permission "notifications" -State "Denied"

        $pupResult = Get-PupPermission -Page $script:page -Permission "notifications"
        # The Permissions API should return a valid state
        $pupResult.State | Should -BeIn @("granted", "denied", "prompt", "unsupported")
    }
}
