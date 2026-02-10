---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupDialogHandler

## SYNOPSIS
Configures automatic handling of browser dialogs.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
