---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupPageConsole

## SYNOPSIS
Gets console messages from a page.

## SYNTAX

## DESCRIPTION
Returns JavaScript console output including logs, warnings, and errors.
Useful for debugging and detecting client-side errors or information disclosure.

## EXAMPLES

### Example 1: Get all console messages
```
$logs = Get-PupPageConsole -Page $page
$logs | Format-Table Type, Text, Url
```

Lists all console output from the page.

### Example 2: Find errors
```
Get-PupPageConsole -Page $page | Where-Object { $_.Type -eq "Error" }
```

Filters for JavaScript errors.

### Example 3: Look for sensitive data in logs
```
Get-PupPageConsole -Page $page | Where-Object {
    $_.Text -match "token|password|secret|key|api"
}
```

Searches console output for potentially sensitive information.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
