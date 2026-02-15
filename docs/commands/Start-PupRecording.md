---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Start-PupRecording

## SYNOPSIS
Starts recording user interactions on a browser page.

## SYNTAX

```
Start-PupRecording -Page <PupPage> [-Clear] [-IncludeScroll] [-IncludeHover]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Clear
Clear any existing recorded events before starting

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IncludeHover
Include hover events

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IncludeScroll
Include scroll events (can be verbose)

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
Page to record interactions on

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
