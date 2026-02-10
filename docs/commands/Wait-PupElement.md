---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Wait-PupElement

## SYNOPSIS
Waits for an element to appear or meet a condition.

## SYNTAX

```
Wait-PupElement -Page <PupPage> -Selector <String> [-Timeout <Int32>] [-PollingInterval <Int32>] [-Visible]
 [-Hidden] [-Enabled] [-Disabled] [-TextContains <String>] [-AttributeValue <String>] [-AttributeName <String>]
 [-PassThru] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Waits until an element matching the selector appears in the DOM, becomes visible, enabled, or meets other conditions.
Essential for handling dynamic content.

## EXAMPLES

### Example 1: Wait for element to appear
```
Wait-PupElement -Page $page -Selector "#dynamic-content" -Timeout 10000
```

Waits up to 10 seconds for the element to appear.

### Example 2: Wait for element to be visible
```
Wait-PupElement -Page $page -Selector "#modal" -Visible -Timeout 5000
```

Waits for a modal dialog to become visible.

### Example 3: Wait for button to be enabled
```
Wait-PupElement -Page $page -Selector "#submit-btn" -Enabled -Timeout 5000
$btn = Find-PupElements -Page $page -Selector "#submit-btn" -First
$btn | Invoke-PupElementClick
```

Waits for a button to become enabled before clicking.

### Example 4: Wait for specific text
```
Wait-PupElement -Page $page -Selector "#status" -TextContains "Complete" -Timeout 30000
```

Waits for a status element to contain "Complete".

## PARAMETERS

### -AttributeName
Name of the attribute to check when using -AttributeValue

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

### -AttributeValue
Wait until an attribute equals the specified value

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

### -Disabled
Wait for element to be disabled

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

### -Enabled
Wait for element to be enabled

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

### -Hidden
Wait for element to be hidden/removed

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
{{ Fill Page Description }}

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -PassThru
Return the element after waiting (default: true)

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

### -PollingInterval
Polling interval in milliseconds (default: 200)

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
CSS selector to wait for

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

### -TextContains
Wait until element's text contains this value

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
Timeout in milliseconds to wait for element (default: 30000)

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

### -Visible
Wait for element to be visible (default: false - just present in DOM)

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
