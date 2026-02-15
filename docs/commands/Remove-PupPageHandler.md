---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Remove-PupPageHandler

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

```
Remove-PupPageHandler [-Page] <PupPage> [-Event] <PupPageEvent> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
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

### -Event
The page event to remove the handler for

```yaml
Type: PupPageEvent
Parameter Sets: (All)
Aliases:
Accepted values: Dialog, Console, PageError, Load, DOMContentLoaded, Request, RequestFinished, RequestFailed, Response, FrameAttached, FrameDetached, FrameNavigated, Download, FileChooser, WorkerCreated, WorkerDestroyed, Close

Required: True
Position: 1
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
Position: 0
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

### Pup.Transport.PupPage
## OUTPUTS

### System.Void
## NOTES

## RELATED LINKS
