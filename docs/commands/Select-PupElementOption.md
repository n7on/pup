---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Select-PupElementOption

## SYNOPSIS
Selects options in a dropdown/select element.

## SYNTAX

## DESCRIPTION
Selects one or more options in a select element by value, visible text, or index.
Can also list all available options.

## EXAMPLES

### Example 1: List available options
```
$select = Find-PupElements -Page $page -Selector "#country" -First
Select-PupElementOption -Element $select -List
```

Shows all options in the dropdown.

### Example 2: Select by value
```
$select = Find-PupElements -Page $page -Selector "#country" -First
Select-PupElementOption -Element $select -Value "US"
```

Selects the option with value "US".

### Example 3: Select by visible text
```
$select = Find-PupElements -Page $page -Selector "#role" -First
Select-PupElementOption -Element $select -Text "Administrator"
```

Selects the option showing "Administrator".

### Example 4: Multi-select
```
$select = Find-PupElements -Page $page -Selector "#permissions" -First
Select-PupElementOption -Element $select -Value "read", "write", "delete"
```

Selects multiple options in a multi-select element.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
