---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupElementScroll

## SYNOPSIS
Scrolls an element into view.

## SYNTAX

```
Invoke-PupElementScroll -Element <PupElement> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Element
Element to scroll into view

```yaml
Type: PupElement
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -ProgressAction
{{ Fill ProgressAction Description }}

```yaml
Type: ActionPreference
Parameter Sets: (All)
Aliases: proga

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
