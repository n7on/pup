---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Send-PupKey

## SYNOPSIS
Sends keyboard input to the page.

## SYNTAX

### SingleKey
```
Send-PupKey -Page <PupPage> -Key <String> [-Modifiers <String[]>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### TypeText
```
Send-PupKey -Page <PupPage> -Text <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Key
The key to press (e.g., Enter, Tab, Escape, ArrowUp, Backspace)

```yaml
Type: String
Parameter Sets: SingleKey
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Modifiers
Modifier keys to hold while pressing the key (e.g., Control, Shift, Alt, Meta)

```yaml
Type: String[]
Parameter Sets: SingleKey
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to send keys to

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

### -Text
Text to type character by character

```yaml
Type: String
Parameter Sets: TypeText
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
