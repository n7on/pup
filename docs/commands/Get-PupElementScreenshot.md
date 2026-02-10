---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupElementScreenshot

## SYNOPSIS
Takes a screenshot of a specific element.

## SYNTAX

## DESCRIPTION
Captures just the specified element as an image, excluding the rest of the page.
Useful for documenting specific UI components.

## EXAMPLES

### Example 1: Screenshot a chart
```
$chart = Find-PupElements -Page $page -Selector "#revenue-chart" -First
Get-PupElementScreenshot -Element $chart -FilePath "chart.png"
```

Captures just the chart element.

### Example 2: Document error state
```
$error = Find-PupElements -Page $page -Selector ".error-panel" -First
$bytes = Get-PupElementScreenshot -Element $error -PassThru
[System.IO.File]::WriteAllBytes("error-evidence.png", $bytes)
```

Screenshots an error message for evidence.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
