---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupHttpHeader

## SYNOPSIS
Sets custom HTTP headers for all requests from a page.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
