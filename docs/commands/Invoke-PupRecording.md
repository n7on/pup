---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupRecording

## SYNOPSIS
Replays recorded browser interactions on a page.

## SYNTAX

```
Invoke-PupRecording [-Page] <PupPage> [-Recording] <PupRecordingEvent[]> [-Delay <Int32>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Replays an array of recorded events on the specified page. Events are obtained from Get-PupRecording and can include clicks, typing, navigation, scrolling, and key presses. Duplicate and redundant events are automatically filtered out during replay. Use -Delay to add a fixed pause between each action.

## EXAMPLES

### Example 1: Record and replay interactions
```
Start-PupRecording -Page $page
# ... interact with the browser ...
Stop-PupRecording -Page $page
$recording = $page | Get-PupRecording
$page | Invoke-PupRecording -Recording $recording
```

Records browser interactions and replays them on the same page.

### Example 2: Replay with timing from original session
```
$recording = $page | Get-PupRecording -IncludeWaits
$page | Invoke-PupRecording -Recording $recording
```

Includes wait events based on the actual timing between actions during recording, preserving the original pace.

### Example 3: Replay with a fixed delay between actions
```
$recording = $page | Get-PupRecording
$page | Invoke-PupRecording -Recording $recording -Delay 200
```

Adds a 200ms pause between each replayed action.

### Example 4: Replay a recording on a different page
```
$recording = $page1 | Get-PupRecording
$page2 = New-PupPage -Browser $browser -Url "https://example.com" -WaitForLoad
$page2 | Invoke-PupRecording -Recording $recording
```

Replays interactions recorded on one page onto a different page.

## PARAMETERS

### -Page
Page to replay recording on

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Recording
Recording events to replay (from Get-PupRecording)

```yaml
Type: PupRecordingEvent[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Delay
Fixed delay between actions in milliseconds (default 0)

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 0
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

### Pup.Transport.PupPage

## OUTPUTS

### System.Void

## NOTES

## RELATED LINKS
