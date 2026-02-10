---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Export-PupPagePdf

## SYNOPSIS
Exports the page as a PDF document.

## SYNTAX

```
Export-PupPagePdf -Page <PupPage> [-FilePath <String>] [-Landscape] [-NoPrintBackground] [-Format <String>]
 [-Scale <Decimal>] [-PassThru] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -FilePath
Path to save the PDF file

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

### -Format
Paper format: A0-A6, Letter, Legal, Tabloid, Ledger (default: A4)

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

### -Landscape
Use landscape orientation

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

### -NoPrintBackground
Print background graphics (default: true)

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
Page to export as PDF

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

### -PassThru
Return PDF data as byte array

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

### -Scale
Scale of the page rendering (default: 1)

```yaml
Type: Decimal
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
