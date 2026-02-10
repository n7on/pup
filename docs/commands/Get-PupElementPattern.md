---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupElementPattern

## SYNOPSIS
Gets selector patterns that match similar elements on the page.

## SYNTAX

## DESCRIPTION
Given an element, generates multiple CSS selector patterns that can match structurally similar elements.
Useful for web scraping when you find one item and want to extract all similar items.
Returns patterns of different types: ByTag, ByClass, ByParentClass, ByStructure, ByAncestorId, ByRole, and ByDataAttribute.

## EXAMPLES

### Example 1: Get patterns for an element
```
# Find one article title by text
$el = Find-PupElements -Page $page -Text "Breaking News" -First

# Get selector patterns that match similar elements
Get-PupElementPattern -Element $el | Format-Table Type, MatchCount, Selector
```

Shows all pattern types with their match counts and selectors.

### Example 2: Use pattern to find all similar elements
```
# Find one product on the page
$product = Find-PupElements -Page $page -TextContains "Price:" -First

# Get the ByStructure pattern (good for repeating items)
$pattern = Get-PupElementPattern -Element $product -Type ByStructure

# Use it to find all products
$allProducts = Find-PupElements -Page $page -Selector $pattern.Selector
$allProducts.Count  # Number of similar items found
```

Uses ByStructure pattern to find elements in repeating containers.

### Example 3: Web scraping workflow
```
# 1. Find one article by visible text
$article = Find-PupElements -Page $page -Text "Thoughts on AI" -First

# 2. Find a pattern that gives reasonable count
$pattern = Get-PupElementPattern -Element $article -MinMatches 5 -MaxMatches 100 |
    Select-Object -First 1

# 3. Use pattern to get all articles
$allArticles = Find-PupElements -Page $page -Selector $pattern.Selector

# 4. Extract data from each
$allArticles | ForEach-Object {
    [PSCustomObject]@{
        Title = $_.InnerText
        Url = Get-PupElementAttribute -Element $_ -Name "href"
    }
}
```

Complete workflow: find one element, get pattern, extract data from all matches.

### Example 4: Filter patterns by type
```
$el = Find-PupElements -Page $page -Selector "button" -First

# Get only class-based patterns
Get-PupElementPattern -Element $el -Type ByClass

# Get only structure-based patterns (for lists/tables)
Get-PupElementPattern -Element $el -Type ByStructure
```

Filter to specific pattern types: ByTag, ByClass, ByParentClass, ByStructure, ByAncestorId, ByRole, ByDataAttribute.

### Example 5: Get container patterns with -Depth
```
# Find article title link
$title = Find-PupElements -Page $page -Text "Breaking News" -First

# Depth 0: patterns for the <a> element itself
Get-PupElementPattern -Element $title -Depth 0

# Depth 1: patterns for the parent (e.g., <span class="titleline">)
Get-PupElementPattern -Element $title -Depth 1

# Depth 2: patterns for the row container (e.g., <tr class="athing">)
$rowPattern = Get-PupElementPattern -Element $title -Depth 2 -Type ByClass

# Now get all article rows, each containing title, points, author, etc.
$allRows = Find-PupElements -Page $page -Selector $rowPattern.Selector
```

Use -Depth to get patterns for ancestor containers.
This is useful when you want to capture related data (title, URL, author, score) that share a common parent.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
