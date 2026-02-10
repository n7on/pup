---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupPageNetwork

## SYNOPSIS
Retrieves captured network requests and responses from a page.

## SYNTAX

## DESCRIPTION
Returns all HTTP requests made by the page, including URLs, methods, status codes, headers, and optionally response bodies.
Essential for analyzing API calls and finding sensitive data.

## EXAMPLES

### Example 1: Get all network requests
```
$requests = Get-PupPageNetwork -Page $page
$requests | Format-Table Method, Status, Url
```

Lists all captured requests with their method, status, and URL.

### Example 2: Get requests with response bodies
```
$requests = Get-PupPageNetwork -Page $page -IncludeContent
$apiCalls = $requests | Where-Object { $_.Url -like "*api*" }
$apiCalls | ForEach-Object { $_.Body }
```

Captures response bodies and filters for API calls to analyze responses.

### Example 3: Find requests with sensitive headers
```
$requests = Get-PupPageNetwork -Page $page
$requests | Where-Object { $_.RequestHeaders["Authorization"] } | Select-Object Url, @{N="Auth";E={$_.RequestHeaders["Authorization"]}}
```

Finds requests containing Authorization headers for token analysis.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
