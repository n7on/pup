---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Remove-PupBrowserHandler

## SYNOPSIS
Removes a browser event handler.

## SYNTAX

```
Remove-PupBrowserHandler [-Event] <PupBrowserEvent> [[-Browser] <PupBrowser>] [-BrowserType <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Removes a previously registered event handler from the browser.
After removal, the event will no longer trigger any custom behavior.

## EXAMPLES

### Example 1: Remove popup handler
```
PS C:\> Remove-PupBrowserHandler -Browser $browser -Event PopupCreated
```

Removes the popup handler, allowing popups to open normally again.

### Example 2: Remove handler after task completion
```
PS C:\> # Set up handler for OAuth flow
PS C:\> Set-PupBrowserHandler -Browser $browser -Event PopupCreated -ScriptBlock { ... }
PS C:\> # ... perform OAuth ...
PS C:\> # Clean up when done
PS C:\> Remove-PupBrowserHandler -Browser $browser -Event PopupCreated
```

Removes the handler after it's no longer needed.

## PARAMETERS

### -Browser
The browser instance

```yaml
Type: PupBrowser
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -BrowserType
Name of the browser to stop (used when Browser parameter is not provided)

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

### -Event
The browser event to remove the handler for

```yaml
Type: PupBrowserEvent
Parameter Sets: (All)
Aliases:
Accepted values: PopupCreated, PageCreated, PageClosed, Disconnected

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ProgressAction
Controls how the cmdlet responds to progress updates.

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

### Pup.Transport.PupBrowser
## OUTPUTS

### System.Void
## NOTES

## RELATED LINKS
