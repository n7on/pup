---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Move-PupPage

## SYNOPSIS
Navigates a page to a new URL.

## SYNTAX

## DESCRIPTION
Navigates the page to the specified URL.
Can optionally wait for the page to fully load before returning.

## EXAMPLES

### Example 1: Navigate to URL
```
Move-PupPage -Page $page -Url "https://target.com/login" -WaitForLoad
```

Navigates to the login page and waits for it to load.

### Example 2: Navigate without waiting
```
Move-PupPage -Page $page -Url "https://target.com/api/data"
```

Starts navigation without waiting for completion.

### Example 3: Test for open redirects
```
Move-PupPage -Page $page -Url "https://target.com/redirect?url=https://evil.com" -WaitForLoad
$page.Page.Url  # Check if redirected to evil.com
```

Tests for open redirect vulnerabilities.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
