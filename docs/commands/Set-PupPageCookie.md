---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupPageCookie

## SYNOPSIS
Sets a cookie on the page.

## SYNTAX

## DESCRIPTION
Creates or modifies cookies with full control over name, value, domain, path, expiration, and security flags.
Essential for session manipulation and testing cookie-based vulnerabilities.

## EXAMPLES

### Example 1: Set a session cookie
```
Set-PupPageCookie -Page $page -Name "session" -Value "admin_session_token" -Domain "target.com"
```

Sets a session cookie to impersonate an admin user.

### Example 2: Set cookie with security flags
```
Set-PupPageCookie -Page $page -Name "auth" -Value "token123" -Domain "target.com" -HttpOnly -Secure -SameSite Strict
```

Creates a secure cookie with HttpOnly and SameSite flags.

### Example 3: Test session fixation
```
# Set a known session ID before authentication
Set-PupPageCookie -Page $page -Name "PHPSESSID" -Value "fixated_session_id" -Domain "target.com"
# Navigate to login and authenticate...
```

Tests for session fixation vulnerabilities.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
