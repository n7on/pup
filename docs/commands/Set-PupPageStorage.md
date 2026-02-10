---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupPageStorage

## SYNOPSIS
Sets localStorage or sessionStorage data on a page.

## SYNTAX

```
Set-PupPageStorage -Page <PupPage> [-Type <String>] [-Key <String>] [-Value <String>] [-Items <Hashtable>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Stores data in the browser's localStorage or sessionStorage.
Can set individual keys or multiple key-value pairs at once.

## EXAMPLES

### Example 1: Set a storage value
```
Set-PupPageStorage -Page $page -Type Local -Key "theme" -Value "dark"
```

Sets a single localStorage value.

### Example 2: Inject auth token
```
Set-PupPageStorage -Page $page -Type Local -Key "authToken" -Value "eyJhbGc..."
Invoke-PupPageReload -Page $page -WaitForLoad
```

Injects an authentication token and reloads to test access.

### Example 3: Set multiple values
```
Set-PupPageStorage -Page $page -Type Session -Items @{
    "userId" = "admin"
    "role" = "administrator"
}
```

Sets multiple sessionStorage values at once.

## PARAMETERS

### -Items
Hashtable of key/value pairs to set

```yaml
Type: Hashtable
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Key
Key to set

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
The page to set storage on

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

### -Value
Value to set

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
