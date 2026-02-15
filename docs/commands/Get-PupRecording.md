---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupRecording

## SYNOPSIS
Gets the recorded events from a browser page.

## SYNTAX

```
Get-PupRecording -Page <PupPage> [-IncludeWaits] [-WaitThreshold <Int32>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieves the recorded interaction events from the specified page.
Events include clicks, typing, navigation, and other user interactions.
Can be called while recording is active or after it has been stopped.
Use -IncludeWaits to insert wait events based on actual timing between actions.

## EXAMPLES

### Example 1: Get recorded events
```
$events = Get-PupRecording -Page $page
$events | Format-Table Type, Selector, Value
```

Retrieves all recorded events and displays them in a table.

### Example 2: Get events with wait times
```
$events = Get-PupRecording -Page $page -IncludeWaits
```

Includes wait events based on actual timing between actions (useful for realistic playback).

### Example 3: Get events with custom wait threshold
```
$events = Get-PupRecording -Page $page -IncludeWaits -WaitThreshold 500
```

Only includes wait events longer than 500ms (default is 100ms).

## PARAMETERS

### -IncludeWaits
Include wait events based on actual timing between actions

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
Page to get recorded events from

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

### -WaitThreshold
Minimum wait to include (waits shorter than this are skipped, default 100ms)

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

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
