---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupPageCookie

## SYNOPSIS
Sets a cookie on the page.

## SYNTAX

```
Set-PupPageCookie -Page <PupPage> -Name <String> -Value <String> [-Domain <String>] [-Path <String>]
 [-Expires <DateTime>] [-HttpOnly] [-Secure] [-SameSite <PupSameSite>] [-Cookies <PupCookie[]>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Creates or modifies cookies with full control over name, value, domain, path, expiration, and security flags.
Essential for session manipulation and testing cookie-based vulnerabilities.

## EXAMPLES

### Example 1: Set a session cookie
```
Set-PupPageCookie -Page $page -Name "session" -Value "admin_session_token" -Domain "target.com"
```

Sets a session cookie to impersonate an admin user.

### Example 2: Set cookie with security flags
```
Set-PupPageCookie -Page $page -Name "auth" -Value "token123" -Domain "target.com" -HttpOnly -Secure -SameSite Strict
```

Creates a secure cookie with HttpOnly and SameSite flags.

### Example 3: Test session fixation
```
# Set a known session ID before authentication
Set-PupPageCookie -Page $page -Name "PHPSESSID" -Value "fixated_session_id" -Domain "target.com"
# Navigate to login and authenticate...
```

Tests for session fixation vulnerabilities.

## PARAMETERS

### -Cookies
Cookie object(s) to set

```yaml
Type: PupCookie[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Domain
Domain for the cookie (defaults to current page domain)

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

### -Expires
Expiration date for the cookie

```yaml
Type: DateTime
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -HttpOnly
Mark cookie as HTTP only

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

### -Name
Name of the cookie

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

### -Path
Path for the cookie (defaults to '/')

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

### -SameSite
SameSite policy for the cookie

```yaml
Type: PupSameSite
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Secure
Mark cookie as secure (HTTPS only)

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

### -Value
Value of the cookie

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
