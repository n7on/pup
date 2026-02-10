---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupBrowser

## SYNOPSIS
Gets browser instances from the current session.

## SYNTAX

```
Get-PupBrowser [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns browser objects that were started in the current PowerShell session.
Useful for managing multiple browser instances.

## EXAMPLES

### Example 1: Get all browsers
```
Get-PupBrowser
```

Lists all browser instances in the session.

### Example 2: Check browser status
```
Get-PupBrowser | Select-Object BrowserType, Running, Headless, ProcessId
```

Shows status information for all browsers.

## PARAMETERS

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
