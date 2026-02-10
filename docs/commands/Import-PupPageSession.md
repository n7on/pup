---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Import-PupPageSession

## SYNOPSIS
Imports browser session data from a file or object.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
