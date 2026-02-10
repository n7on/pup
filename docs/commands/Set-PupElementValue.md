---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupElementValue

## SYNOPSIS
Sets the value property of a form element.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
