---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupHttpHeader

## SYNOPSIS
Sets custom HTTP headers for all requests from a page.

## SYNTAX

### SingleHeader
```
Set-PupHttpHeader -Page <PupPage> -Name <String> -Value <String> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### MultipleHeaders
```
Set-PupHttpHeader -Page <PupPage> -Headers <Hashtable> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### Clear
```
Set-PupHttpHeader -Page <PupPage> [-Clear] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Adds custom headers to every HTTP request made by the page.
Useful for injecting authorization tokens, spoofing IP addresses, or testing header-based vulnerabilities.

## EXAMPLES

### Example 1: Add authorization header
```
Set-PupHttpHeader -Page $page -Name "Authorization" -Value "Bearer eyJhbG..."
```

Adds a Bearer token to all subsequent requests.

### Example 2: Spoof client IP
```
Set-PupHttpHeader -Page $page -Headers @{
    "X-Forwarded-For" = "127.0.0.1"
    "X-Real-IP" = "127.0.0.1"
}
```

Sets headers to bypass IP-based access controls.

### Example 3: Test for host header injection
```
Set-PupHttpHeader -Page $page -Name "X-Forwarded-Host" -Value "evil.com"
```

Tests for host header injection vulnerabilities.

### Example 4: Clear all custom headers
```
Set-PupHttpHeader -Page $page -Clear
```

Removes all previously set custom headers.

## PARAMETERS

### -Clear
Clear all extra headers

```yaml
Type: SwitchParameter
Parameter Sets: Clear
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Headers
Hashtable of header name/value pairs

```yaml
Type: Hashtable
Parameter Sets: MultipleHeaders
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
HTTP header name

```yaml
Type: String
Parameter Sets: SingleHeader
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to set HTTP request headers for

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

### -Value
HTTP header value

```yaml
Type: String
Parameter Sets: SingleHeader
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
