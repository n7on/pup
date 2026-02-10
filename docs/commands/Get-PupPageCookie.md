---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupPageCookie

## SYNOPSIS
Gets cookies from a page.

## SYNTAX

## DESCRIPTION
Returns cookies associated with the page.
Can filter by name or domain.
Useful for inspecting session tokens and authentication cookies.

## EXAMPLES

### Example 1: Get all cookies
```
$cookies = Get-PupPageCookie -Page $page
$cookies | Format-Table Name, Value, Domain, HttpOnly, Secure
```

Lists all cookies with their security flags.

### Example 2: Get session cookie
```
$session = Get-PupPageCookie -Page $page -Name "session"
$session.Value
```

Retrieves the session cookie value.

### Example 3: Check for insecure cookies
```
Get-PupPageCookie -Page $page | Where-Object { -not $_.HttpOnly -or -not $_.Secure } |
    Select-Object Name, HttpOnly, Secure
```

Finds cookies missing HttpOnly or Secure flags.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
