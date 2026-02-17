---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupSource

## SYNOPSIS
Gets the HTML source of a page.

## SYNTAX

### FromPage
```
Get-PupSource -Page <PupPage> [-FilePath <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### FromFrame
```
Get-PupSource -Frame <PupFrame> [-FilePath <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns the current HTML content of the page, including any dynamic changes made by JavaScript.

## EXAMPLES

### Example 1: Get page source
```
$html = Get-PupSource -Page $page
$html | Out-File "page.html"
```

Saves the page HTML to a file.

### Example 2: Search for comments
```
$html = Get-PupSource -Page $page
[regex]::Matches($html, "<!--.*?-->", "Singleline") | ForEach-Object { $_.Value }
```

Extracts HTML comments that might contain sensitive info.

### Example 3: Find hidden inputs
```
$html = Get-PupSource -Page $page
[regex]::Matches($html, '<input[^>]*type=["\']hidden["\'][^>]*>') | ForEach-Object { $_.Value }
```

Finds hidden form fields that might contain tokens or IDs.

## PARAMETERS

### -FilePath
Save the HTML to a file path

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

### -Frame
The frame to get HTML source from

```yaml
Type: PupFrame
Parameter Sets: FromFrame
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Page
The page to get HTML source from

```yaml
Type: PupPage
Parameter Sets: FromPage
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
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
