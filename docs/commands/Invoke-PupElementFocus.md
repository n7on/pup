---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupElementFocus

## SYNOPSIS
Sets focus on an element.

## SYNTAX

```
Invoke-PupElementFocus -Element <PupElement> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Gives keyboard focus to an element.
Required before sending keyboard input to a specific element.

## EXAMPLES

### Example 1: Focus input before typing
```
$input = Find-PupElements -Page $page -Selector "#search" -First
$input | Invoke-PupElementFocus
Send-PupKey -Page $page -Text "search query"
```

Focuses an input field before typing into it.

### Example 2: Test autofocus behavior
```
$input = Find-PupElements -Page $page -Selector "#password" -First
$input | Invoke-PupElementFocus
# Check if any autocomplete values appear
```

Focuses password field to test for autocomplete behavior.

## PARAMETERS

### -Element
Element to focus

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
