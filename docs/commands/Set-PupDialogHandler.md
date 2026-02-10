---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupDialogHandler

## SYNOPSIS
Configures automatic handling of browser dialogs.

## SYNTAX

### SetHandler
```
Set-PupDialogHandler -Page <PupPage> -Action <PupDialogAction> [-PromptText <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### RemoveHandler
```
Set-PupDialogHandler -Page <PupPage> [-Remove] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Sets up automatic responses to JavaScript alert(), confirm(), and prompt() dialogs.
Without a handler, dialogs will block automation.

## EXAMPLES

### Example 1: Accept all dialogs
```
Set-PupDialogHandler -Page $page -Action Accept
```

Automatically clicks OK/Accept on all dialogs.

### Example 2: Dismiss dialogs
```
Set-PupDialogHandler -Page $page -Action Dismiss
```

Automatically clicks Cancel/Dismiss on all dialogs.

### Example 3: Handle prompt with text
```
Set-PupDialogHandler -Page $page -Action Accept -PromptText "user input"
```

Accepts prompts and enters the specified text.

### Example 4: Remove handler
```
Set-PupDialogHandler -Page $page -Remove
```

Removes the dialog handler.

## PARAMETERS

### -Action
Action to take when a dialog appears (Accept or Dismiss)

```yaml
Type: PupDialogAction
Parameter Sets: SetHandler
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to configure dialog handling for

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

### -PromptText
Text to enter when a prompt dialog appears (only used with Accept action)

```yaml
Type: String
Parameter Sets: SetHandler
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Remove
Remove the dialog handler

```yaml
Type: SwitchParameter
Parameter Sets: RemoveHandler
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
