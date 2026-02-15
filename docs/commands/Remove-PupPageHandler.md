---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Remove-PupPageHandler

## SYNOPSIS
Removes a page event handler.

## SYNTAX

```
Remove-PupPageHandler -Page <PupPage> -Event <PupPageEvent> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Removes a previously registered event handler from the page.
After removal, the event will no longer trigger any custom behavior.
For Dialog events, this means dialogs will block until manually handled.

## EXAMPLES

### Example 1: Remove dialog handler
```
Remove-PupPageHandler -Page $page -Event Dialog
```

Removes the dialog handler.
Dialogs will now require manual handling.

## PARAMETERS

### -Event
The page event to remove the handler for

```yaml
Type: PupPageEvent
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to remove the handler from

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
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
