---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Select-PupElementOption

## SYNOPSIS
Selects options in a dropdown/select element.

## SYNTAX

### ByValue
```
Select-PupElementOption -Element <PupElement> -Value <String[]> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### ByText
```
Select-PupElementOption -Element <PupElement> -Text <String[]> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### ByIndex
```
Select-PupElementOption -Element <PupElement> -Index <Int32[]> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### ListOptions
```
Select-PupElementOption -Element <PupElement> [-List] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Element
The select element to interact with

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

### -Index
Select option(s) by zero-based index

```yaml
Type: Int32[]
Parameter Sets: ByIndex
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -List
List all available options in the select element

```yaml
Type: SwitchParameter
Parameter Sets: ListOptions
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

### -Text
Select option(s) by visible text

```yaml
Type: String[]
Parameter Sets: ByText
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value
Select option(s) by value attribute

```yaml
Type: String[]
Parameter Sets: ByValue
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
