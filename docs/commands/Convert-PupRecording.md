---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Convert-PupRecording

## SYNOPSIS
Converts Chrome DevTools Recorder JSON to a Pup PowerShell script.

## SYNTAX

## DESCRIPTION
Takes a recording exported from Chrome DevTools Recorder (JSON format) and generates equivalent Pup PowerShell commands.
Supports navigation, clicks, typing, keyboard input, scrolling, hovering, and wait conditions.

## EXAMPLES

### Example 1: Convert recording to script
```
Convert-PupRecording -InputFile "recording.json" -OutputFile "script.ps1"
```

Converts a Chrome DevTools recording to a Pup script file.

### Example 2: Include setup and teardown
```
Convert-PupRecording -InputFile "recording.json" -IncludeSetup -IncludeTeardown -OutputFile "complete-script.ps1"
```

Generates a complete script with browser setup and cleanup code.

### Example 3: Preview conversion output
```
Convert-PupRecording -InputFile "recording.json" | Out-Host
```

Outputs the converted script to the console for review.

### Example 4: Custom variable names
```
Convert-PupRecording -InputFile "recording.json" -PageVariable '$p' -BrowserVariable '$b' -IncludeSetup
```

Uses custom variable names for page and browser objects.

### Example 5: Record in Chrome, convert, run
```
# 1. In Chrome: DevTools > Recorder > Start recording
# 2. Perform actions in the browser
# 3. Export as JSON

# 4. Convert to Pup script
Convert-PupRecording -InputFile "my-recording.json" -OutputFile "test.ps1" -IncludeSetup -IncludeTeardown

# 5. Run the script
./test.ps1
```

Complete workflow from recording to automated script.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
