---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupPageReload

## SYNOPSIS
Reloads the current page.

## SYNTAX

## DESCRIPTION
Refreshes the page, optionally waiting for the reload to complete.
Useful after modifying cookies or storage.

## EXAMPLES

### Example 1: Reload and wait
```
Invoke-PupPageReload -Page $page -WaitForLoad
```

Reloads the page and waits for it to fully load.

### Example 2: Test after modifying session
```
Set-PupPageCookie -Page $page -Name "role" -Value "admin" -Domain "target.com"
Invoke-PupPageReload -Page $page -WaitForLoad
# Check if admin features are now visible
```

Reloads after cookie manipulation to test privilege escalation.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
