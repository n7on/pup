---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# ConvertTo-PupScript

## SYNOPSIS
Converts recorded events to a PowerShell automation script.

## SYNTAX

```
ConvertTo-PupScript -RecordingEvents <PupRecordingEvent[]> [-PageVariable <String>] [-BrowserVariable <String>]
 [-IncludeSetup] [-IncludeTeardown] [-Url <String>] [-OutputFile <String>] [-DelayMin <Int32>]
 [-DelayMax <Int32>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -BrowserVariable
Variable name for browser in generated script

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DelayMax
Maximum delay between actions in milliseconds (randomizes between min and max)

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

### -DelayMin
Minimum delay between actions in milliseconds

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

### -IncludeSetup
Include setup code in generated script

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

### -IncludeTeardown
Include teardown code in generated script

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

### -OutputFile
Save output to file

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PageVariable
Variable name for page in generated script

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
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

### -RecordingEvents
Recording events to convert

```yaml
Type: PupRecordingEvent[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Url
Override URL for setup code (by default uses URL from recording start)

```yaml
Type: String
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
