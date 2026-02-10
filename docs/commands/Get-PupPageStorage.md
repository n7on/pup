---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupPageStorage

## SYNOPSIS
Gets localStorage or sessionStorage data from a page.

## SYNTAX

```
Get-PupPageStorage -Page <PupPage> [-Type <String>] [-Key <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Retrieves data stored in the browser's localStorage or sessionStorage.
Useful for inspecting client-side data storage for sensitive information.

## EXAMPLES

### Example 1: Get all localStorage
```
Get-PupPageStorage -Page $page -Type Local
```

Returns all localStorage key-value pairs.

### Example 2: Get specific key
```
Get-PupPageStorage -Page $page -Type Local -Key "authToken"
```

Gets a specific storage value.

### Example 3: Check for sensitive data
```
$storage = Get-PupPageStorage -Page $page -Type Local
$storage | Where-Object { $_.Key -match "token|auth|session|password" }
```

Searches storage for potentially sensitive keys.

## PARAMETERS

### -Key
Specific key to retrieve

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
