---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Remove-PupPageCookie

## SYNOPSIS
Removes cookies from a page.

## SYNTAX

### ByFilter
```
Remove-PupPageCookie -Page <PupPage> [-Name <String>] [-Domain <String>] [-Path <String>] [-Url <String>]
 [-All] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### ByObject
```
Remove-PupPageCookie -Page <PupPage> -Cookies <PupCookie[]> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Deletes cookies matching the specified criteria.
Useful for testing session handling and logout functionality.

## EXAMPLES

### Example 1: Remove specific cookie
```
Remove-PupPageCookie -Page $page -Name "session" -Domain "target.com"
```

Removes the session cookie.

### Example 2: Test session invalidation
```
# Remove session and verify access is denied
Remove-PupPageCookie -Page $page -Name "auth_token" -Domain "target.com"
Invoke-PupPageReload -Page $page -WaitForLoad
# Check if redirected to login
```

Tests that removing auth cookie invalidates the session.

## PARAMETERS

### -All
Remove all cookies

```yaml
Type: SwitchParameter
Parameter Sets: ByFilter
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Cookies
Cookie object(s) to remove

```yaml
Type: PupCookie[]
Parameter Sets: ByObject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Domain
Domain filter (supports wildcards; defaults to current page host when deleting by name)

```yaml
Type: String
Parameter Sets: ByFilter
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name
Cookie name to remove (supports wildcards)

```yaml
Type: String
Parameter Sets: ByFilter
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
Parameter Sets: ByFilter
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

```yaml
Type: PupPage
Parameter Sets: ByObject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -Path
Path filter

```yaml
Type: String
Parameter Sets: ByFilter
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

### -Url
Url for the cookie (used when domain is not provided)

```yaml
Type: String
Parameter Sets: ByFilter
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
