---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupElementScroll

## SYNOPSIS
Scrolls an element into view.

## SYNTAX

## DESCRIPTION
Scrolls the page so that the specified element is visible in the viewport.
Useful before interacting with elements that are off-screen.

## EXAMPLES

### Example 1: Scroll element into view
```
$el = Find-PupElements -Page $page -Selector "#footer-link" -First
$el | Invoke-PupElementScroll
$el | Invoke-PupElementClick
```

Scrolls to an element before clicking it.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
