BeforeAll {
    Import-Module (Join-Path $PSScriptRoot "../" "output" "Pup" "Pup.psd1") -Force
    Install-PupBrowser -BrowserType Chrome

    $script:browser = Start-PupBrowser -Headless
    # Use jsonplaceholder as it's reliable and allows CORS
    $script:page = New-PupPage -Browser $script:browser -Url "https://jsonplaceholder.typicode.com" -WaitForLoad
}

AfterAll {
    if ($script:page.Running) { Remove-PupPage -Page $script:page }
    if ($script:browser.Running) { Stop-PupBrowser -Browser $script:browser }
}

Describe "Invoke-PupHttpFetch" {
    It "Makes a GET request and returns response" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1"

        $response | Should -Not -BeNullOrEmpty
        $response.Status | Should -Be 200
        $response.Ok | Should -BeTrue
        $response.Body | Should -Not -BeNullOrEmpty
    }

    It "Returns response headers" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1"

        $response.Headers | Should -Not -BeNullOrEmpty
        $response.Headers["content-type"] | Should -BeLike "*application/json*"
    }

    It "Parses JSON response with -AsJson" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1" -AsJson

        $response.JsonBody | Should -Not -BeNullOrEmpty
        $response.JsonBody["id"] | Should -Be 1
        $response.JsonBody["userId"] | Should -Not -BeNullOrEmpty
    }

    It "Makes POST request with hashtable body" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts" -Method POST -Body @{
            title = "Test Post"
            body = "Test content"
            userId = 1
        } -AsJson

        $response.Status | Should -Be 201
        $response.Ok | Should -BeTrue
        $response.JsonBody["title"] | Should -Be "Test Post"
    }

    It "Makes POST request with string body" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts" -Method POST `
            -Body '{"title":"String Body","body":"content","userId":1}' `
            -ContentType "application/json" -AsJson

        $response.Status | Should -Be 201
        $response.JsonBody["title"] | Should -Be "String Body"
    }

    It "Makes PUT request" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1" -Method PUT -Body @{
            id = 1
            title = "Updated Title"
            body = "Updated body"
            userId = 1
        } -AsJson

        $response.Status | Should -Be 200
        $response.JsonBody["title"] | Should -Be "Updated Title"
    }

    It "Makes DELETE request" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1" -Method DELETE

        $response.Status | Should -Be 200
        $response.Ok | Should -BeTrue
    }

    It "Handles 404 response" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/99999"

        $response.Status | Should -Be 404
        $response.Ok | Should -BeFalse
    }

    It "Sends custom headers" {
        # httpbin would be better for this, but jsonplaceholder still accepts the request
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1" -Headers @{
            "X-Custom-Header" = "test-value"
        }

        $response.Status | Should -Be 200
    }

    It "Returns final URL after redirect" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts/1"

        $response.Url | Should -Not -BeNullOrEmpty
        $response.Url | Should -BeLike "*jsonplaceholder*"
    }
}

Describe "Invoke-PupHttpFetch with different content types" {
    It "Sends form data" {
        $response = Invoke-PupHttpFetch -Page $script:page -Url "/posts" -Method POST `
            -Body "title=Form+Post&body=content&userId=1" `
            -ContentType "application/x-www-form-urlencoded"

        $response.Status | Should -Be 201
    }
}
