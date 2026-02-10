---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupPageScreenshot

## SYNOPSIS
Takes a screenshot of the page.

## SYNTAX

## DESCRIPTION
Captures the current page as a PNG image.
Can capture the visible viewport or the full scrollable page.

## EXAMPLES

### Example 1: Save screenshot to file
```
Get-PupPageScreenshot -Page $page -FilePath "evidence.png"
```

Saves a screenshot of the visible page.

### Example 2: Full page screenshot
```
Get-PupPageScreenshot -Page $page -FilePath "full-page.png" -FullPage
```

Captures the entire scrollable page.

### Example 3: Get screenshot as bytes
```
$bytes = Get-PupPageScreenshot -Page $page -PassThru
[System.IO.File]::WriteAllBytes("screenshot.png", $bytes)
```

Returns screenshot data for further processing.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
