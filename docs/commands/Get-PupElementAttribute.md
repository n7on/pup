---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupElementAttribute

## SYNOPSIS
Gets an attribute value from an element.

## SYNTAX

```
Get-PupElementAttribute -Element <PupElement> -Name <String> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieves the value of a specific HTML attribute from an element.
Useful for extracting data attributes, href values, or other element metadata.

## EXAMPLES

### Example 1: Get href attribute
```
$link = Find-PupElements -Page $page -Selector "a.download" -First
Get-PupElementAttribute -Element $link -Name "href"
```

Gets the URL from a link element.

### Example 2: Extract data attributes
```
$el = Find-PupElements -Page $page -Selector "[data-user-id]" -First
$userId = Get-PupElementAttribute -Element $el -Name "data-user-id"
```

Extracts custom data attributes.

### Example 3: Check for target="_blank"
```
$links = Find-PupElements -Page $page -Selector "a"
$links | ForEach-Object {
    $target = Get-PupElementAttribute -Element $_ -Name "target"
    $rel = Get-PupElementAttribute -Element $_ -Name "rel"
    if ($target -eq "_blank" -and $rel -notlike "*noopener*") {
        "Vulnerable link: $(Get-PupElementAttribute -Element $_ -Name 'href')"
    }
}
```

Finds links vulnerable to reverse tabnabbing.

## PARAMETERS

### -Element
Element to get attribute from

```yaml
Type: PupElement
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Name
Name of the attribute to get

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
