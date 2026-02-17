---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupPagePermission

## SYNOPSIS
Sets browser permissions for a page.

## SYNTAX

```
Set-PupPagePermission [-Page] <PupPage> [-Permission] <String> [-State] <String> [-Origin <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Overrides browser permission prompts for the page.
This allows automation of features that normally require user permission like geolocation, notifications, camera, and clipboard access.

## EXAMPLES

### Example 1: Grant geolocation permission
```
PS C:\> Set-PupPagePermission -Page $page -Permission geolocation -State Granted
```

Allows the page to access geolocation without prompting.

### Example 2: Deny notification permission
```
PS C:\> Set-PupPagePermission -Page $page -Permission notifications -State Denied
```

Blocks notification requests from the page.

### Example 3: Grant clipboard access
```
PS C:\> Set-PupPagePermission -Page $page -Permission clipboard-read -State Granted
PS C:\> Set-PupPagePermission -Page $page -Permission clipboard-write -State Granted
```

Allows the page to read from and write to the clipboard.

### Example 4: Grant camera and microphone
```
PS C:\> Set-PupPagePermission -Page $page -Permission camera -State Granted
PS C:\> Set-PupPagePermission -Page $page -Permission microphone -State Granted
```

Allows the page to access camera and microphone for video calls.

## PARAMETERS

### -Origin
Origin to set permission for (defaults to current page origin)

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

### -Page
The page to set permission for

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Permission
The permission to set

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: geolocation, notifications, camera, microphone, clipboard-read, clipboard-write, midi, midi-sysex, background-sync, accelerometer, gyroscope, magnetometer, accessibility-events, payment-handler, idle-detection, screen-wake-lock, storage-access

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -State
The permission state

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: Granted, Denied, Prompt

Required: True
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
Controls how the cmdlet responds to progress updates.

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
