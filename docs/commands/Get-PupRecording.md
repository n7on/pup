---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupRecording

## SYNOPSIS
Gets the recorded events from a browser page.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
