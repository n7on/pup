---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupElementHover

## SYNOPSIS
Hovers the mouse over an element.

## SYNTAX

## DESCRIPTION
Simulates moving the mouse over an element.
Triggers hover states, tooltips, and dropdown menus.

## EXAMPLES

### Example 1: Trigger dropdown menu
```
$menu = Find-PupElements -Page $page -Selector ".dropdown-trigger" -First
$menu | Invoke-PupElementHover
Start-Sleep -Milliseconds 500
$item = Find-PupElements -Page $page -Selector ".dropdown-item.admin" -First
$item | Invoke-PupElementClick
```

Hovers to open a dropdown menu, then clicks an item.

### Example 2: Capture tooltip content
```
$el = Find-PupElements -Page $page -Selector "[data-tooltip]" -First
$el | Invoke-PupElementHover
Start-Sleep -Milliseconds 300
$tooltip = Find-PupElements -Page $page -Selector ".tooltip" -First
Get-PupElementText -Element $tooltip
```

Triggers and captures tooltip text.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
