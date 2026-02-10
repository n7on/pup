---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Import-PupPageSession

## SYNOPSIS
Imports browser session data from a file or object.

## SYNTAX

### FilePath
```
Import-PupPageSession -Page <PupPage> -FilePath <String> [-NoCookies] [-NoLocalStorage] [-NoSessionStorage]
 [-Reload] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Session
```
Import-PupPageSession -Page <PupPage> -Session <PupSession> [-NoCookies] [-NoLocalStorage] [-NoSessionStorage]
 [-Reload] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Restores a previously exported session including cookies, localStorage, and sessionStorage.
Useful for resuming authenticated sessions or sharing session data between team members.

## EXAMPLES

### Example 1: Import session from file
```
Import-PupPageSession -Page $page -FilePath "authenticated-session.json" -Reload
```

Restores the session and reloads the page to apply it.

### Example 2: Import only cookies
```
Import-PupPageSession -Page $page -FilePath "session.json" -NoLocalStorage -NoSessionStorage
```

Imports only cookies, skipping storage data.

### Example 3: Import from session object
```
$session = Export-PupPageSession -Page $page
# ... do some testing ...
# Restore the original session
Import-PupPageSession -Page $page -Session $session -Reload
```

Restores a session from an in-memory object.

### Example 4: Test with different user sessions
```
# Test as admin
Import-PupPageSession -Page $page -FilePath "admin-session.json" -Reload
# ... run admin tests ...

# Test as regular user
Import-PupPageSession -Page $page -FilePath "user-session.json" -Reload
# ... run user tests ...
```

Switches between different user sessions for testing access controls.

## PARAMETERS

### -FilePath
{{ Fill FilePath Description }}

```yaml
Type: String
Parameter Sets: FilePath
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoCookies
{{ Fill NoCookies Description }}

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

### -NoLocalStorage
{{ Fill NoLocalStorage Description }}

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

### -NoSessionStorage
{{ Fill NoSessionStorage Description }}

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
Accept pipeline input: True (ByValue)
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

### -Reload
{{ Fill Reload Description }}

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

### -Session
{{ Fill Session Description }}

```yaml
Type: PupSession
Parameter Sets: Session
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
