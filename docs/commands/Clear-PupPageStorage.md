---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Clear-PupPageStorage

## SYNOPSIS
Clears localStorage or sessionStorage data.

## SYNTAX

## DESCRIPTION
Removes data from the browser's localStorage or sessionStorage.
Can clear all data or specific keys.

## EXAMPLES

### Example 1: Clear all localStorage
```
Clear-PupPageStorage -Page $page -Type Local
```

Removes all localStorage data.

### Example 2: Clear specific key
```
Clear-PupPageStorage -Page $page -Type Local -Key "authToken"
```

Removes only the specified key.

### Example 3: Test logout functionality
```
# Clear all client-side storage and verify logout
Clear-PupPageStorage -Page $page -Type Local
Clear-PupPageStorage -Page $page -Type Session
Invoke-PupPageReload -Page $page -WaitForLoad
# Check if user is logged out
```

Tests that clearing storage properly logs out the user.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
