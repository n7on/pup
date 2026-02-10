---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Start-PupRecording

## SYNOPSIS
Starts recording user interactions on a browser page.

## SYNTAX

## DESCRIPTION
Begins capturing user interactions (clicks, typing, navigation, etc.) on the specified page.
The recording continues until Stop-PupRecording is called.
Use Get-PupRecording to retrieve captured events and ConvertTo-PupScript to generate a PowerShell automation script.

## EXAMPLES

### Example 1: Start recording on a page
```
$page = New-PupPage -Browser $browser -Url "https://example.com" -WaitForLoad
Start-PupRecording -Page $page
# Interact with the browser manually...
Stop-PupRecording -Page $page
```

Starts recording interactions on the page.
Interact with the browser, then stop recording.

### Example 2: Start recording with scroll events
```
Start-PupRecording -Page $page -IncludeScroll
```

Includes scroll events in the recording (can be verbose).

### Example 3: Clear previous recording and start fresh
```
Start-PupRecording -Page $page -Clear
```

Clears any existing recorded events before starting the new recording.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
