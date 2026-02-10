---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupPageStorage

## SYNOPSIS
Gets localStorage or sessionStorage data from a page.

## SYNTAX

## DESCRIPTION
Retrieves data stored in the browser's localStorage or sessionStorage.
Useful for inspecting client-side data storage for sensitive information.

## EXAMPLES

### Example 1: Get all localStorage
```
Get-PupPageStorage -Page $page -Type Local
```

Returns all localStorage key-value pairs.

### Example 2: Get specific key
```
Get-PupPageStorage -Page $page -Type Local -Key "authToken"
```

Gets a specific storage value.

### Example 3: Check for sensitive data
```
$storage = Get-PupPageStorage -Page $page -Type Local
$storage | Where-Object { $_.Key -match "token|auth|session|password" }
```

Searches storage for potentially sensitive keys.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
