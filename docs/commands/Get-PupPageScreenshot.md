---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupPageScreenshot

## SYNOPSIS
Takes a screenshot of the page.

## SYNTAX

```
Get-PupPageScreenshot -Page <PupPage> [-FilePath <String>] [-FullPage] [-PassThru]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -FilePath
Path to save the screenshot file (optional)

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

### -FullPage
Take screenshot of full page (default: false - visible area only)

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

### -Page
The page to capture

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

### -PassThru
Return screenshot data as byte array

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
