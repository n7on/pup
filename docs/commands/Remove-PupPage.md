---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Remove-PupPage

## SYNOPSIS
Closes a browser page.

## SYNTAX

```
Remove-PupPage -Page <PupPage> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Closes the specified page (tab) in the browser.
The page object should not be used after removal.

## EXAMPLES

### Example 1: Close a page
```
Remove-PupPage -Page $page
```

Closes the browser tab.

## PARAMETERS

### -Page
The page to close

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
