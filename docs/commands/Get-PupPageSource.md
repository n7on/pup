---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupPageSource

## SYNOPSIS
Gets the HTML source of a page.

## SYNTAX

## DESCRIPTION
Returns the current HTML content of the page, including any dynamic changes made by JavaScript.

## EXAMPLES

### Example 1: Get page source
```
$html = Get-PupPageSource -Page $page
$html | Out-File "page.html"
```

Saves the page HTML to a file.

### Example 2: Search for comments
```
$html = Get-PupPageSource -Page $page
[regex]::Matches($html, "<!--.*?-->", "Singleline") | ForEach-Object { $_.Value }
```

Extracts HTML comments that might contain sensitive info.

### Example 3: Find hidden inputs
```
$html = Get-PupPageSource -Page $page
[regex]::Matches($html, '<input[^>]*type=["\']hidden["\'][^>]*>') | ForEach-Object { $_.Value }
```

Finds hidden form fields that might contain tokens or IDs.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
