---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Set-PupPageStorage

## SYNOPSIS
Sets localStorage or sessionStorage data on a page.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
