---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Find-PupElements

## SYNOPSIS
Finds elements on a page using CSS selectors, XPath, or visible text.

## SYNTAX

### FromPage
```
Find-PupElements -Page <PupPage> [-Selector <String>] [-XPath] [-Text <String>] [-TextContains <String>]
 [-WaitForLoad] [-Timeout <Int32>] [-First] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### FromElement
```
Find-PupElements -Element <PupElement> [-Selector <String>] [-XPath] [-Text <String>] [-TextContains <String>]
 [-WaitForLoad] [-Timeout <Int32>] [-First] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Element
Parent element to search within

```yaml
Type: PupElement
Parameter Sets: FromElement
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -First
Return only the first element found

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
Page to search within

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

### -Selector
CSS selector or XPath expression to find elements

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

### -Text
Find elements by exact visible text match

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

### -TextContains
Find elements containing this text (case-insensitive)

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

### -Timeout
Timeout in milliseconds to wait for elements to appear (default: 5000)

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WaitForLoad
Wait for elements to load before returning

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

### -XPath
Use XPath expression instead of CSS selector

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
