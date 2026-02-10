---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupElementClick

## SYNOPSIS
Clicks on an element.

## SYNTAX

```
Invoke-PupElementClick -Element <PupElement> [-ClickCount <Int32>] [-DoubleClick] [-WaitForLoad]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Simulates a mouse click on the specified element.
The element is scrolled into view if necessary.

## EXAMPLES

### Example 1: Click a button
```
$btn = Find-PupElements -Page $page -Selector "#submit" -First
$btn | Invoke-PupElementClick
```

Finds and clicks the submit button.

### Example 2: Click and wait for navigation
```
$link = Find-PupElements -Page $page -Selector "a.admin-link" -First
$link | Invoke-PupElementClick
Start-Sleep -Seconds 2  # Wait for navigation
```

Clicks a link and waits for the page to load.

## PARAMETERS

### -ClickCount
Number of clicks (2 for double-click)

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

### -DoubleClick
Perform a double-click

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

### -Element
Element to click

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

### -WaitForLoad
Wait for page to load after click (useful for links)

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
