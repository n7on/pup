---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupElementSelector

## SYNOPSIS
Gets a unique CSS selector for an element.

## SYNTAX

## DESCRIPTION
Generates a CSS selector that uniquely identifies the element.
Useful for debugging and creating reproducible test scripts.

## EXAMPLES

### Example 1: Get selector for clicked element
```
$el = Find-PupElements -Page $page -Selector "button" | Select-Object -First 1
Get-PupElementSelector -Element $el
```

Returns a unique selector for the element.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
