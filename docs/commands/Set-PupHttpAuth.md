---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupHttpAuth

## SYNOPSIS
Sets HTTP Basic Authentication credentials for a page.

## SYNTAX

### Credentials
```
Set-PupHttpAuth -Page <PupPage> -Username <String> -Password <String> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### PSCredential
```
Set-PupHttpAuth -Page <PupPage> -Credential <PSCredential> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

### Clear
```
Set-PupHttpAuth -Page <PupPage> [-Clear] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Configures credentials to automatically respond to HTTP Basic Authentication challenges.
The credentials are used for all requests from the page.

## EXAMPLES

### Example 1: Set basic auth credentials
```
Set-PupHttpAuth -Page $page -Username "admin" -Password "admin"
Move-PupPage -Page $page -Url "https://target.com/admin" -WaitForLoad
```

Sets credentials before navigating to a protected endpoint.

### Example 2: Use PSCredential for secure password handling
```
$cred = Get-Credential
Set-PupHttpAuth -Page $page -Credential $cred
```

Uses PowerShell's secure credential prompt.

### Example 3: Clear authentication
```
Set-PupHttpAuth -Page $page -Clear
```

Removes authentication credentials from the page.

## PARAMETERS

### -Clear
Clear authentication credentials

```yaml
Type: SwitchParameter
Parameter Sets: Clear
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
PSCredential object for HTTP Basic Authentication

```yaml
Type: PSCredential
Parameter Sets: PSCredential
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to set authentication for

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

### -Password
Password for HTTP Basic Authentication

```yaml
Type: String
Parameter Sets: Credentials
Aliases:

Required: True
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

### -Username
Username for HTTP Basic Authentication

```yaml
Type: String
Parameter Sets: Credentials
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
