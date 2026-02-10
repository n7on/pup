---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Export-PupPagePdf

## SYNOPSIS
Exports the page as a PDF document.

## SYNTAX

## DESCRIPTION
Generates a PDF of the current page.
Useful for creating documentation or evidence during security assessments.

## EXAMPLES

### Example 1: Save page as PDF
```
Export-PupPagePdf -Page $page -FilePath "evidence.pdf"
```

Exports the page to a PDF file.

### Example 2: Get PDF as bytes
```
$bytes = Export-PupPagePdf -Page $page -PassThru
[System.IO.File]::WriteAllBytes("report.pdf", $bytes)
```

Returns PDF data for further processing.

### Example 3: Custom PDF settings
```
Export-PupPagePdf -Page $page -FilePath "report.pdf" -Landscape -PrintBackground
```

Exports with landscape orientation and background colors.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
