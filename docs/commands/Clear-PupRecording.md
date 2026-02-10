---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Clear-PupRecording

## SYNOPSIS
Clears all recorded events from a browser page.

## SYNTAX

## DESCRIPTION
Removes all recorded interaction events from the specified page.
This can be called whether recording is active or not.
Useful for starting a fresh recording session without creating a new page.

## EXAMPLES

### Example 1: Clear recorded events
```
Clear-PupRecording -Page $page
```

Clears all recorded events from the page.

### Example 2: Record multiple sessions
```
# First session
Start-PupRecording -Page $page
# ... interactions ...
Stop-PupRecording -Page $page
$session1 = Get-PupRecording -Page $page
Clear-PupRecording -Page $page

# Second session
Start-PupRecording -Page $page
# ... more interactions ...
Stop-PupRecording -Page $page
$session2 = Get-PupRecording -Page $page
```

Record multiple separate sessions by clearing between them.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
