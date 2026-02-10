---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Invoke-PupPageBack

## SYNOPSIS
Navigates the page back in history.

## SYNTAX

## DESCRIPTION
Simulates clicking the browser back button.
Navigates to the previous page in the browsing history.

## EXAMPLES

### Example 1: Go back
```
Invoke-PupPageBack -Page $page -WaitForLoad
```

Navigates back and waits for the page to load.

### Example 2: Test back navigation after form submit
```
# Submit a form, then go back to check if data persists
Invoke-PupPageBack -Page $page -WaitForLoad
$input = Find-PupElements -Page $page -Selector "#username" -First
$input.Value  # Check if form data is still there
```

Tests form data persistence across back navigation.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
