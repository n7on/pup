---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Stop-PupRecording

## SYNOPSIS
Stops recording user interactions on a browser page.

## SYNTAX

## DESCRIPTION
Stops the active recording session on the specified page.
The recorded events are preserved and can be retrieved using Get-PupRecording.
Recording can be started and stopped multiple times to capture different sessions.

## EXAMPLES

### Example 1: Stop recording
```
Stop-PupRecording -Page $page
```

Stops the recording session on the page.

### Example 2: Full recording workflow
```
Start-PupRecording -Page $page
# ... user interactions ...
Stop-PupRecording -Page $page
$events = Get-PupRecording -Page $page
$script = $events | ConvertTo-PupScript -IncludeSetup
```

Complete workflow: start recording, interact, stop, get events, and convert to script.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
