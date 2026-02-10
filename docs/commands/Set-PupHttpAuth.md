---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupHttpAuth

## SYNOPSIS
Sets HTTP Basic Authentication credentials for a page.

## SYNTAX

## DESCRIPTION
Configures credentials to automatically respond to HTTP Basic Authentication challenges.
The credentials are used for all requests from the page.

## EXAMPLES

### Example 1: Set basic auth credentials
```
Set-PupHttpAuth -Page $page -Username "admin" -Password "admin"
Move-PupPage -Page $page -Url "https://target.com/admin" -WaitForLoad
```

Sets credentials before navigating to a protected endpoint.

### Example 2: Use PSCredential for secure password handling
```
$cred = Get-Credential
Set-PupHttpAuth -Page $page -Credential $cred
```

Uses PowerShell's secure credential prompt.

### Example 3: Clear authentication
```
Set-PupHttpAuth -Page $page -Clear
```

Removes authentication credentials from the page.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
