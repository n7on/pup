---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupPageBack

## SYNOPSIS
Navigates the page back in history.

## SYNTAX

```
Invoke-PupPageBack -Page <PupPage> [-WaitForLoad] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Page
The page to navigate back

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
Wait for page to load after navigation

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
