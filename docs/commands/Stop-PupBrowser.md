---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Stop-PupBrowser

## SYNOPSIS
Stops a running browser instance.

## SYNTAX

```
Stop-PupBrowser [-Force] [-Browser <PupBrowser>] [-BrowserType <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Closes the browser and releases all resources.
All pages associated with the browser will be closed.

## EXAMPLES

### Example 1: Stop a browser
```
Stop-PupBrowser -Browser $browser
```

Closes the browser instance.

### Example 2: Stop via pipeline
```
$browser | Stop-PupBrowser
```

Stops the browser using pipeline input.

### Example 3: Stop all running browsers
```
Get-PupBrowser | Where-Object Running | Stop-PupBrowser
```

Finds and stops all running browser instances.

## PARAMETERS

### -Browser
The browser instance

```yaml
Type: PupBrowser
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -BrowserType
Name of the browser to stop (used when Browser parameter is not provided)

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
Force stop without confirmation

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
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
