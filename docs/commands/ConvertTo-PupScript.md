---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# ConvertTo-PupScript

## SYNOPSIS
Converts recorded events to a PowerShell automation script.

## SYNTAX

## DESCRIPTION
Converts an array of recorded events into a PowerShell script that can replay the interactions.
Supports adding setup/teardown code, custom variable names, and delays between actions for human-like behavior.

## EXAMPLES

### Example 1: Convert events to script
```
$events = Get-PupRecording -Page $page
$script = $events | ConvertTo-PupScript
$script
```

Converts recorded events to a PowerShell script and displays it.

### Example 2: Generate complete script with setup and teardown
```
$events = Get-PupRecording -Page $page
$script = $events | ConvertTo-PupScript -IncludeSetup -IncludeTeardown
```

Generates a complete script including browser startup and cleanup code.
The URL is automatically captured from when recording started.

### Example 3: Save script to file with custom variable names
```
$events | ConvertTo-PupScript -IncludeSetup -PageVariable '$myPage' -BrowserVariable '$myBrowser' -OutputFile "automation.ps1"
```

Saves the script to a file using custom variable names.

### Example 4: Add human-like delays between actions
```
$events | ConvertTo-PupScript -DelayMin 100 -DelayMax 500
```

Adds random delays between 100-500ms between each action for more human-like behavior.

### Example 5: Preserve actual timing from recording
```
$events = Get-PupRecording -Page $page -IncludeWaits
$script = $events | ConvertTo-PupScript
```

Using -IncludeWaits with Get-PupRecording adds wait events that preserve the actual timing from the recording session.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
