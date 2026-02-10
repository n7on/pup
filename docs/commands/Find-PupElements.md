---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Find-PupElements

## SYNOPSIS
Finds elements on a page using CSS selectors, XPath, or visible text.

## SYNTAX

## DESCRIPTION
Searches for elements matching the given selector or text content.
Returns element objects that can be used for interaction (click, type, etc.).
Text search finds the most specific matching elements, filtering out parent elements whose text is inherited from children.

## EXAMPLES

### Example 1: Find element by ID
```
$loginBtn = Find-PupElements -Page $page -Selector "#login-button" -First
```

Finds the first element with ID "login-button".

### Example 2: Find all form inputs
```
$inputs = Find-PupElements -Page $page -Selector "form input"
```

Returns all input elements within forms on the page.

### Example 3: Find element by XPath
```
$el = Find-PupElements -Page $page -Selector "//input[@name='csrf_token']" -XPath -First
```

Uses XPath to find CSRF token input fields.

### Example 4: Find element by exact visible text
```
$btn = Find-PupElements -Page $page -Text "Add to Cart" -First
$btn | Invoke-PupElementClick
```

Finds an element with exactly "Add to Cart" as its visible text and clicks it.

### Example 5: Find elements containing text (case-insensitive)
```
$links = Find-PupElements -Page $page -TextContains "privacy"
$links | ForEach-Object { $_.InnerText }
```

Finds all elements containing "privacy" in their text (case-insensitive partial match).

### Example 6: Combine text search with CSS selector
```
$submitBtn = Find-PupElements -Page $page -Selector "button" -TextContains "Submit" -First
```

Finds the first button element containing "Submit" text.
Combining selector and text search narrows results efficiently.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
