---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupElement

## SYNOPSIS
Types text into an element or sets its value.

## SYNTAX

## DESCRIPTION
Types text character by character into an input element, or directly sets its value.
Can optionally clear existing content first.

## EXAMPLES

### Example 1: Type into input field
```
$username = Find-PupElements -Page $page -Selector "#username" -First
Set-PupElement -Element $username -Text "admin"
```

Types "admin" into the username field.

### Example 2: Clear and type new value
```
$input = Find-PupElements -Page $page -Selector "#search" -First
Set-PupElement -Element $input -Text "new search" -Clear
```

Clears existing text and types new content.

### Example 3: Set value directly (faster)
```
$input = Find-PupElements -Page $page -Selector "#token" -First
Set-PupElement -Element $input -Value "injected_value"
```

Sets the value property directly without typing.

### Example 4: Test SQL injection
```
$search = Find-PupElements -Page $page -Selector "#search" -First
Set-PupElement -Element $search -Text "' OR '1'='1" -Clear
$submit = Find-PupElements -Page $page -Selector "#submit" -First
$submit | Invoke-PupElementClick
```

Enters SQL injection payload into a search field.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
