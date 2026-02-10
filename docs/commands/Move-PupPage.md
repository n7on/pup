---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Move-PupPage

## SYNOPSIS
Navigates a page to a new URL.

## SYNTAX

```
Move-PupPage -Page <PupPage> -Url <String> [-WaitForLoad] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

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

### -Page
The page to navigate

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

### -Url
URL to navigate

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

### -WaitForLoad
Wait for page to load completely before returning

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
