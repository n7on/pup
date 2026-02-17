---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupBrowserHandler

## SYNOPSIS
Sets an event handler for browser-level events.

## SYNTAX

### ScriptBlock
```
Set-PupBrowserHandler -Event <PupBrowserEvent> -ScriptBlock <ScriptBlock> [-Browser <PupBrowser>]
 [-BrowserType <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Action
```
Set-PupBrowserHandler -Event <PupBrowserEvent> -Action <PupHandlerAction> [-Browser <PupBrowser>]
 [-BrowserType <String>] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Registers an event handler for browser-level events like popup windows, new pages, page closures, and disconnection.
Use -Action Dismiss to auto-close popups, or -ScriptBlock for custom handling.

## EXAMPLES

### Example 1: Auto-dismiss popups
```
Set-PupBrowserHandler -Browser $browser -Event PopupCreated -Action Dismiss
```

Automatically closes any popup windows opened by the page.

### Example 2: Capture popup for OAuth flow
```
$global:popup = $null
Set-PupBrowserHandler -Browser $browser -Event PopupCreated -ScriptBlock {
    param($e)
    $global:popup = $e.Page
}
# Click button that opens OAuth popup
Find-PupElements -Page $page -Selector "#login-oauth" | Invoke-PupElementClick
Start-Sleep -Milliseconds 500
# Now interact with the popup
Find-PupElements -Page $global:popup -Selector "#approve" | Invoke-PupElementClick
```

Captures popup windows for multi-window workflows like OAuth.

### Example 3: Track new pages
```
$global:pages = @()
Set-PupBrowserHandler -Browser $browser -Event PageCreated -ScriptBlock {
    param($e)
    $global:pages += $e.Page
    Write-Host "New page opened: $($e.Page.Url)"
}
```

Tracks all new pages/tabs opened in the browser.

## PARAMETERS

### -Action
Action to take when the event occurs (Dismiss for popups)

```yaml
Type: PupHandlerAction
Parameter Sets: Action
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Browser
The browser instance

```yaml
Type: PupBrowser
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
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
The browser event to handle

```yaml
Type: PupBrowserEvent
Parameter Sets: (All)
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

### -ScriptBlock
Script block to execute when the event occurs.
Receives event data as parameter.

```yaml
Type: ScriptBlock
Parameter Sets: ScriptBlock
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
