---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Invoke-PupPageReload

## SYNOPSIS
Reloads the current page.

## SYNTAX

```
Invoke-PupPageReload -Page <PupPage> [-WaitForLoad] [-HardReload] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Refreshes the page, optionally waiting for the reload to complete.
Useful after modifying cookies or storage.

## EXAMPLES

### Example 1: Reload and wait
```
Invoke-PupPageReload -Page $page -WaitForLoad
```

Reloads the page and waits for it to fully load.

### Example 2: Test after modifying session
```
Set-PupPageCookie -Page $page -Name "role" -Value "admin" -Domain "target.com"
Invoke-PupPageReload -Page $page -WaitForLoad
# Check if admin features are now visible
```

Reloads after cookie manipulation to test privilege escalation.

## PARAMETERS

### -HardReload
Ignore cache and force reload from server

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
Wait for page to load after reload

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
