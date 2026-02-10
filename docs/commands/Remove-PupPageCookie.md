---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Remove-PupPageCookie

## SYNOPSIS
Removes cookies from a page.

## SYNTAX

## DESCRIPTION
Deletes cookies matching the specified criteria.
Useful for testing session handling and logout functionality.

## EXAMPLES

### Example 1: Remove specific cookie
```
Remove-PupPageCookie -Page $page -Name "session" -Domain "target.com"
```

Removes the session cookie.

### Example 2: Test session invalidation
```
# Remove session and verify access is denied
Remove-PupPageCookie -Page $page -Name "auth_token" -Domain "target.com"
Invoke-PupPageReload -Page $page -WaitForLoad
# Check if redirected to login
```

Tests that removing auth cookie invalidates the session.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
