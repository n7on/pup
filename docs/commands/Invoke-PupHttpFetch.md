---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupHttpFetch

## SYNOPSIS
Makes an HTTP request from within the browser context.

## SYNTAX

### Default (Default)
```
Invoke-PupHttpFetch -Page <PupPage> -Url <String> [-Method <String>] [-Body <Object>] [-Headers <Hashtable>]
 [-ContentType <String>] [-Timeout <Int32>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### AsJson
```
Invoke-PupHttpFetch -Page <PupPage> -Url <String> [-Method <String>] [-Body <Object>] [-Headers <Hashtable>]
 [-ContentType <String>] [-AsJson] [-Timeout <Int32>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### OutFile
```
Invoke-PupHttpFetch -Page <PupPage> -Url <String> [-Method <String>] [-Body <Object>] [-Headers <Hashtable>]
 [-ContentType <String>] [-OutFile <String>] [-Timeout <Int32>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

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

### Example 6: Download a binary file
```
$src = Get-PupElementAttribute -Page $page -Selector 'img.hero' -Attribute src
Invoke-PupHttpFetch -Page $page -Url $src -OutFile "C:\images\hero.png"
```

Downloads a binary file (image, PDF, ZIP, etc.) and saves it to disk.
The -OutFile parameter uses base64 encoding internally to safely transfer binary data.

## PARAMETERS

### -AsJson
Parse response body as JSON

```yaml
Type: SwitchParameter
Parameter Sets: AsJson
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Body
Request body (string or object to be serialized as JSON)

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ContentType
Content-Type header value

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Headers
Additional HTTP headers as a hashtable

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Method
HTTP method (GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS)

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutFile
Save response body to a file (binary-safe)

```yaml
Type: String
Parameter Sets: OutFile
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page context to make the request from

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Timeout
Request timeout in milliseconds (default: 30000)

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Url
The URL to fetch

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
