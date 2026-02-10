---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupElementText

## SYNOPSIS
Gets the text content of an element.

## SYNTAX

## DESCRIPTION
Returns the visible text content of an element, including text from child elements.

## EXAMPLES

### Example 1: Get button text
```
$btn = Find-PupElements -Page $page -Selector ".submit-btn" -First
Get-PupElementText -Element $btn
```

Gets the text displayed on a button.

### Example 2: Extract error messages
```
$errors = Find-PupElements -Page $page -Selector ".error-message"
$errors | ForEach-Object { Get-PupElementText -Element $_ }
```

Extracts all error messages from the page.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
