---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupBrowserHandler

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### ScriptBlock
```
Set-PupBrowserHandler [-Event] <PupBrowserEvent> [-ScriptBlock] <ScriptBlock> [[-Browser] <PupBrowser>]
 [-BrowserType <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Action
```
Set-PupBrowserHandler [-Event] <PupBrowserEvent> [-Action] <PupHandlerAction> [[-Browser] <PupBrowser>]
 [-BrowserType <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Action
Action to take when the event occurs (Dismiss for popups)

```yaml
Type: PupHandlerAction
Parameter Sets: Action
Aliases:
Accepted values: Dismiss, Accept, Ignore

Required: True
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Browser
The browser instance

```yaml
Type: PupBrowser
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
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

### -Event
The browser event to handle

```yaml
Type: PupBrowserEvent
Parameter Sets: (All)
Aliases:
Accepted values: PopupCreated, PageCreated, PageClosed, Disconnected

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ScriptBlock
Script block to execute when the event occurs.
Receives event data as parameter.

```yaml
Type: ScriptBlock
Parameter Sets: ScriptBlock
Aliases:

Required: True
Position: 2
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

### Pup.Transport.PupBrowser
## OUTPUTS

### System.Void
## NOTES

## RELATED LINKS
