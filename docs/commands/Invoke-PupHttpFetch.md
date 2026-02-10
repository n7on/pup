---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupHttpFetch

## SYNOPSIS
Makes an HTTP request from within the browser context.

## SYNTAX

## DESCRIPTION
Executes a fetch() request in the browser page context.
Automatically uses the page's cookies and session.
Subject to CORS and CSP restrictions like a real browser request.

## EXAMPLES

### Example 1: Simple GET request
```
$response = Invoke-PupFetch -Page $page -Url "/api/user/profile"
$response.Body | ConvertFrom-Json
```

Makes a GET request to a relative URL using the page's session.

### Example 2: POST with JSON body
```
$response = Invoke-PupFetch -Page $page -Url "/api/users" -Method POST -Body @{
    username = "newuser"
    role = "admin"
} -AsJson
if ($response.Ok) { "User created: $($response.Body.id)" }
```

Sends a POST request with a JSON body.
The -AsJson switch parses the response.

### Example 3: Test API endpoint with custom headers
```
$response = Invoke-PupFetch -Page $page -Url "/api/admin/users" -Method DELETE -Headers @{
    "X-Custom-Header" = "test"
}
"Status: $($response.Status) - $($response.StatusText)"
```

Makes a DELETE request with custom headers.

### Example 4: Test IDOR vulnerability
```
# Try accessing another user's data
$response = Invoke-PupFetch -Page $page -Url "/api/users/999/documents" -AsJson
if ($response.Ok) {
    "IDOR vulnerability! Got $($response.Body.Count) documents"
}
```

Tests for insecure direct object reference by accessing another user's resources.

### Example 5: Test CSRF with state-changing request
```
$response = Invoke-PupFetch -Page $page -Url "/api/account/email" -Method PUT -Body @{
    email = "attacker@evil.com"
}
"Response: $($response.Status) - Check if CSRF token was required"
```

Tests if state-changing requests require CSRF protection.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
