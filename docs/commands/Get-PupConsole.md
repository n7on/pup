---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupConsole

## SYNOPSIS
Gets console messages from a page.

## SYNTAX

```
Get-PupConsole -Page <PupPage> [-PassThru] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns JavaScript console output including logs, warnings, and errors.
Useful for debugging and detecting client-side errors or information disclosure.

## EXAMPLES

### Example 1: Get all console messages
```
$logs = Get-PupConsole -Page $page
$logs | Format-Table Type, Text, Url
```

Lists all console output from the page.

### Example 2: Find errors
```
Get-PupConsole -Page $page | Where-Object { $_.Type -eq "Error" }
```

Filters for JavaScript errors.

### Example 3: Look for sensitive data in logs
```
Get-PupConsole -Page $page | Where-Object {
    $_.Text -match "token|password|secret|key|api"
}
```

Searches console output for potentially sensitive information.

## PARAMETERS

### -Page
The page to get console messages from

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -PassThru
Return captured entries to the pipeline (default: true)

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
