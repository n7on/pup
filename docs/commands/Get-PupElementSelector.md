---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupElementSelector

## SYNOPSIS
Gets a unique CSS selector for an element.

## SYNTAX

```
Get-PupElementSelector -Element <PupElement> [-Similar] [-ShowCount] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

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

### -Element
The element to get selector for

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

### -ShowCount
Show count of elements this selector would match

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Similar
Generate selector for all similar elements at same hierarchical level

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
