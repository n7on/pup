---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupPermission

## SYNOPSIS
Gets browser permissions for a page.

## SYNTAX

```
Get-PupPermission -Page <PupPage> [-Permission <String>] [-Origin <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Queries the current state of browser permissions for the page.
Returns granted, denied, or prompt status.
If no specific permission is specified, returns all permissions.

## EXAMPLES

### Example 1: Get all permissions
```
Get-PupPermission -Page $page
```

Returns the state of all supported permissions for the page.

### Example 2: Get specific permission
```
Get-PupPermission -Page $page -Permission geolocation
```

Returns the current state of the geolocation permission.

### Example 3: Check before setting
```
$perm = Get-PupPermission -Page $page -Permission notifications
if ($perm.State -eq 'prompt') {
    Set-PupPermission -Page $page -Permission notifications -State Granted
}
```

Only sets permission if it hasn't been set yet.

## PARAMETERS

### -Origin
Origin to query permission for (defaults to current page origin)

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
The page to get permission for

```yaml
Type: PupPage
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Permission
The permission to query (if not specified, returns all permissions)

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
