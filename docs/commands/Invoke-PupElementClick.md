---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupElementClick

## SYNOPSIS
Clicks on an element.

## SYNTAX

## DESCRIPTION
Simulates a mouse click on the specified element.
The element is scrolled into view if necessary.

## EXAMPLES

### Example 1: Click a button
```
$btn = Find-PupElements -Page $page -Selector "#submit" -First
$btn | Invoke-PupElementClick
```

Finds and clicks the submit button.

### Example 2: Click and wait for navigation
```
$link = Find-PupElements -Page $page -Selector "a.admin-link" -First
$link | Invoke-PupElementClick
Start-Sleep -Seconds 2  # Wait for navigation
```

Clicks a link and waits for the page to load.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
