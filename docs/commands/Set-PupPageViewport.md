---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupPageViewport

## SYNOPSIS
Sets the browser viewport size and device emulation settings.

## SYNTAX

```
Set-PupPageViewport -Page <PupPage> -Width <Int32> -Height <Int32> [-DeviceScaleFactor <Double>] [-IsMobile]
 [-HasTouch] [-IsLandscape] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Configures the viewport dimensions and device characteristics.
Useful for testing responsive designs or emulating mobile devices.

## EXAMPLES

### Example 1: Set desktop viewport
```
Set-PupPageViewport -Page $page -Width 1920 -Height 1080
```

Sets a standard desktop viewport size.

### Example 2: Emulate mobile device
```
Set-PupPageViewport -Page $page -Width 375 -Height 667 -IsMobile -HasTouch -DeviceScaleFactor 2
```

Emulates an iPhone-like mobile device.

### Example 3: Test responsive breakpoints
```
# Test mobile
Set-PupPageViewport -Page $page -Width 375 -Height 667
Get-PupPageScreenshot -Page $page -FilePath "mobile.png"

# Test tablet
Set-PupPageViewport -Page $page -Width 768 -Height 1024
Get-PupPageScreenshot -Page $page -FilePath "tablet.png"

# Test desktop
Set-PupPageViewport -Page $page -Width 1920 -Height 1080
Get-PupPageScreenshot -Page $page -FilePath "desktop.png"
```

Captures screenshots at different responsive breakpoints.

## PARAMETERS

### -DeviceScaleFactor
{{ Fill DeviceScaleFactor Description }}

```yaml
Type: Double
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HasTouch
{{ Fill HasTouch Description }}

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

### -Height
{{ Fill Height Description }}

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -IsLandscape
{{ Fill IsLandscape Description }}

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

### -IsMobile
{{ Fill IsMobile Description }}

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
{{ Fill Page Description }}

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
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

### -Width
{{ Fill Width Description }}

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: True
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
