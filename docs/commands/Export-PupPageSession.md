---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Export-PupPageSession

## SYNOPSIS
Exports browser session data (cookies, localStorage, sessionStorage) to a file or object.

## SYNTAX

```
Export-PupPageSession -Page <PupPage> [-FilePath <String>] [-PassThru] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Captures the current session state including cookies, localStorage, and sessionStorage.
Can be saved to a JSON file for later import or sharing with team members.

## EXAMPLES

### Example 1: Export session to file
```
Export-PupPageSession -Page $page -FilePath "authenticated-session.json"
```

Saves the current session to a JSON file.

### Example 2: Export session to object
```
$session = Export-PupPageSession -Page $page
$session.Cookies | Format-Table Name, Value, Domain
```

Returns the session as an object for inspection.

### Example 3: Save after authentication
```
# Log in to the application
$username = Find-PupElements -Page $page -Selector "#username" -First
Set-PupElement -Element $username -Text "admin"
$password = Find-PupElements -Page $page -Selector "#password" -First
Set-PupElement -Element $password -Text "secret"
$login = Find-PupElements -Page $page -Selector "#login-btn" -First
$login | Invoke-PupElementClick
Start-Sleep -Seconds 2

# Save the authenticated session for reuse
Export-PupPageSession -Page $page -FilePath "admin-session.json"
```

Saves an authenticated session for reuse without logging in again.

## PARAMETERS

### -FilePath
{{ Fill FilePath Description }}

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
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -PassThru
{{ Fill PassThru Description }}

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
