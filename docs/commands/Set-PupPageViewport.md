---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupPageViewport

## SYNOPSIS
Sets the browser viewport size and device emulation settings.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
