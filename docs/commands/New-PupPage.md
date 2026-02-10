---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# New-PupPage

## SYNOPSIS
Creates a new browser page (tab) and optionally navigates to a URL.

## SYNTAX

## DESCRIPTION
Creates a new page in the browser with stealth mode automatically enabled.
Network and console capture are initialized for pentesting purposes.

## EXAMPLES

### Example 1: Create a blank page
```
$page = New-PupPage -Browser $browser
```

Creates a new blank page ready for navigation.

### Example 2: Navigate to a URL and wait for load
```
$page = New-PupPage -Browser $browser -Url "https://target.com" -WaitForLoad
```

Creates a page and navigates to the target, waiting for the page to fully load.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
