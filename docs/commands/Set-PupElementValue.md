---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupElementValue

## SYNOPSIS
Sets the value property of a form element.

## SYNTAX

```
Set-PupElementValue -Element <PupElement> [-Value <Object>] [-Values <Object[]>] [-Check] [-Uncheck]
 [-NoEvents] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Directly sets the value property of an input element.
Faster than typing but doesn't trigger keyboard events.

## EXAMPLES

### Example 1: Set input value
```
$input = Find-PupElements -Page $page -Selector "#csrf-token" -First
Set-PupElementValue -Element $input -Value "manipulated_token"
```

Directly sets the value of a hidden input.

### Example 2: Modify hidden fields before submit
```
$priceField = Find-PupElements -Page $page -Selector "input[name=price]" -First
Set-PupElementValue -Element $priceField -Value "0.01"
$form = Find-PupElements -Page $page -Selector "form" -First
# Submit to test price manipulation
```

Tests for price manipulation vulnerabilities.

## PARAMETERS

### -Check
Mark checkbox/radio as checked

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Element
Element to set

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

### -NoEvents
Skip firing input/change events

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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

### -Uncheck
Mark checkbox/radio as unchecked

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value
Value to set (string/number/bool or array for multi-select)

```yaml
Type: Object
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Values
Values for multi-select elements

```yaml
Type: Object[]
Parameter Sets: (All)
Aliases:

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
