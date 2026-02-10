---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Wait-PupElement

## SYNOPSIS
Waits for an element to appear or meet a condition.

## SYNTAX

## DESCRIPTION
Waits until an element matching the selector appears in the DOM, becomes visible, enabled, or meets other conditions.
Essential for handling dynamic content.

## EXAMPLES

### Example 1: Wait for element to appear
```
Wait-PupElement -Page $page -Selector "#dynamic-content" -Timeout 10000
```

Waits up to 10 seconds for the element to appear.

### Example 2: Wait for element to be visible
```
Wait-PupElement -Page $page -Selector "#modal" -Visible -Timeout 5000
```

Waits for a modal dialog to become visible.

### Example 3: Wait for button to be enabled
```
Wait-PupElement -Page $page -Selector "#submit-btn" -Enabled -Timeout 5000
$btn = Find-PupElements -Page $page -Selector "#submit-btn" -First
$btn | Invoke-PupElementClick
```

Waits for a button to become enabled before clicking.

### Example 4: Wait for specific text
```
Wait-PupElement -Page $page -Selector "#status" -TextContains "Complete" -Timeout 30000
```

Waits for a status element to contain "Complete".

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
