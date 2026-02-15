---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupPagePermission

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

```
Set-PupPagePermission [-Page] <PupPage> [-Permission] <String> [-State] <String> [-Origin <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

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

### Pup.Transport.PupPage
## OUTPUTS

### System.Void
## NOTES

## RELATED LINKS
