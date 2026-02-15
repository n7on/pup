---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupCookie

## SYNOPSIS
Gets cookies from a page.

## SYNTAX

```
Get-PupCookie -Page <PupPage> [-Name <String>] [-Domain <String>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Returns cookies associated with the page.
Can filter by name or domain.
Useful for inspecting session tokens and authentication cookies.

## EXAMPLES

### Example 1: Get all cookies
```
$cookies = Get-PupCookie -Page $page
$cookies | Format-Table Name, Value, Domain, HttpOnly, Secure
```

Lists all cookies with their security flags.

### Example 2: Get session cookie
```
$session = Get-PupCookie -Page $page -Name "session"
$session.Value
```

Retrieves the session cookie value.

### Example 3: Check for insecure cookies
```
Get-PupCookie -Page $page | Where-Object { -not $_.HttpOnly -or -not $_.Secure } |
    Select-Object Name, HttpOnly, Secure
```

Finds cookies missing HttpOnly or Secure flags.

## PARAMETERS

### -Domain
Filter cookies by domain (supports wildcards)

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

### -Name
Filter cookies by name (supports wildcards)

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
The page to get cookies from

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
