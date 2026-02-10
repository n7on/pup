---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Send-PupKey

## SYNOPSIS
Sends keyboard input to the page.

## SYNTAX

## DESCRIPTION
Simulates keyboard input by pressing keys or typing text.
Can combine with modifier keys (Ctrl, Alt, Shift).

## EXAMPLES

### Example 1: Type text
```
$input = Find-PupElements -Page $page -Selector "#search" -First
$input | Invoke-PupElementFocus
Send-PupKey -Page $page -Text "search query"
```

Types text into the focused element.

### Example 2: Press Enter to submit
```
Send-PupKey -Page $page -Key "Enter"
```

Presses the Enter key to submit a form.

### Example 3: Keyboard shortcut
```
Send-PupKey -Page $page -Key "a" -Modifiers "Control"
```

Sends Ctrl+A to select all.

### Example 4: Navigate with Tab
```
Send-PupKey -Page $page -Key "Tab"
Send-PupKey -Page $page -Key "Tab"
Send-PupKey -Page $page -Key "Enter"
```

Uses Tab to navigate form fields.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
