---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupPageHandler

## SYNOPSIS
Gets active page event handlers.

## SYNTAX

```
Get-PupPageHandler [-Page] <PupPage> [[-Event] <PupPageEvent>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Returns information about event handlers currently registered on the page.
Shows the event type, action (if using built-in action), and whether a ScriptBlock is attached.

## EXAMPLES

### Example 1: Get all page handlers
```
PS C:\> Get-PupPageHandler -Page $page

Event   Action HasScriptBlock
-----   ------ --------------
Dialog  Accept          False
Console               True
```

Lists all active page event handlers.

### Example 2: Check for dialog handler
```
PS C:\> $handler = Get-PupPageHandler -Page $page -Event Dialog
PS C:\> if (-not $handler) {
    Set-PupPageHandler -Page $page -Event Dialog -Action Accept
}
```

Sets a dialog handler only if one isn't already registered.

## PARAMETERS

### -Event
Filter by specific event type

```yaml
Type: PupPageEvent
Parameter Sets: (All)
Aliases:
Accepted values: Dialog, Console, PageError, Load, DOMContentLoaded, Request, RequestFinished, RequestFailed, Response, FrameAttached, FrameDetached, FrameNavigated, Download, FileChooser, WorkerCreated, WorkerDestroyed, Close

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to get handlers for

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
Controls how the cmdlet responds to progress updates.

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

### System.Management.Automation.PSObject
## NOTES

## RELATED LINKS
