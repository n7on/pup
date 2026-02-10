---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Clear-PupPageStorage

## SYNOPSIS
Clears localStorage or sessionStorage data.

## SYNTAX

```
Clear-PupPageStorage -Page <PupPage> [-Type <String>] [-Key <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Removes data from the browser's localStorage or sessionStorage.
Can clear all data or specific keys.

## EXAMPLES

### Example 1: Clear all localStorage
```
Clear-PupPageStorage -Page $page -Type Local
```

Removes all localStorage data.

### Example 2: Clear specific key
```
Clear-PupPageStorage -Page $page -Type Local -Key "authToken"
```

Removes only the specified key.

### Example 3: Test logout functionality
```
# Clear all client-side storage and verify logout
Clear-PupPageStorage -Page $page -Type Local
Clear-PupPageStorage -Page $page -Type Session
Invoke-PupPageReload -Page $page -WaitForLoad
# Check if user is logged out
```

Tests that clearing storage properly logs out the user.

## PARAMETERS

### -Key
Key to remove (omit to clear all)

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

### -Page
The page to clear storage from

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

### -Type
Storage type: Local or Session (default: Local)

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
