---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupBrowserHandler

## SYNOPSIS
Gets active browser event handlers.

## SYNTAX

```
Get-PupBrowserHandler [[-Event] <PupBrowserEvent>] [[-Browser] <PupBrowser>] [-BrowserType <String>]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns information about event handlers currently registered on the browser.
Shows the event type, action (if using built-in action), and whether a ScriptBlock is attached.

## EXAMPLES

### Example 1: Get all browser handlers
```
PS C:\> Get-PupBrowserHandler -Browser $browser

Event         Action HasScriptBlock
-----         ------ --------------
PopupCreated Dismiss          False
PageCreated               True
```

Lists all active browser event handlers.

### Example 2: Check if specific handler exists
```
PS C:\> $handler = Get-PupBrowserHandler -Browser $browser -Event PopupCreated
PS C:\> if ($handler) { "Popup handler is set" }
```

Checks if a handler is registered for a specific event.

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
Filter by specific event type

```yaml
Type: PupBrowserEvent
Parameter Sets: (All)
Aliases:
Accepted values: PopupCreated, PageCreated, PageClosed, Disconnected

Required: False
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

### System.Management.Automation.PSObject
## NOTES

## RELATED LINKS
