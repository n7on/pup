---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupElementValue

## SYNOPSIS
Gets the value property of a form element.

## SYNTAX

## DESCRIPTION
Returns the current value of an input, textarea, or select element.
Different from text content - this is the form field value.

## EXAMPLES

### Example 1: Get input value
```
$input = Find-PupElements -Page $page -Selector "#username" -First
Get-PupElementValue -Element $input
```

Gets the current value in the username field.

### Example 2: Extract hidden field values
```
$hiddenInputs = Find-PupElements -Page $page -Selector "input[type=hidden]"
$hiddenInputs | ForEach-Object {
    $name = Get-PupElementAttribute -Element $_ -Name "name"
    $value = Get-PupElementValue -Element $_
    "$name = $value"
}
```

Extracts all hidden form field values.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
