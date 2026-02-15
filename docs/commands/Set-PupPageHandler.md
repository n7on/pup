---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupPageHandler

## SYNOPSIS
Sets an event handler for page events.

## SYNTAX

### ScriptBlock
```
Set-PupPageHandler -Page <PupPage> -Event <PupPageEvent> -ScriptBlock <ScriptBlock>
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Action
```
Set-PupPageHandler -Page <PupPage> -Event <PupPageEvent> -Action <PupHandlerAction>
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Registers an event handler for page-level events like dialogs, console messages, network requests, and more.
Use -Action for built-in behaviors (Accept/Dismiss/Ignore) or -ScriptBlock for custom handling.

## EXAMPLES

### Example 1: Auto-accept alert dialogs
```
Set-PupPageHandler -Page $page -Event Dialog -Action Accept
```

Automatically accepts all dialogs on the page.

### Example 2: Auto-dismiss dialogs
```
Set-PupPageHandler -Page $page -Event Dialog -Action Dismiss
```

Automatically dismisses (cancels) all dialogs.

### Example 3: Custom dialog handler
```
Set-PupPageHandler -Page $page -Event Dialog -ScriptBlock {
    param($e)
    if ($e.Type -eq 'prompt') {
        $e.Accept("my answer")
    } else {
        $e.Dismiss()
    }
}
```

Handles dialogs with custom logic based on dialog type.

### Example 4: Capture console messages
```
$global:logs = @()
Set-PupPageHandler -Page $page -Event Console -ScriptBlock {
    param($e)
    $global:logs += $e.Text
}
```

Captures all console.log messages from the page.

### Example 5: Monitor network requests
```
Set-PupPageHandler -Page $page -Event Request -ScriptBlock {
    param($e)
    Write-Host "Request: $($e.Method) $($e.Url)"
}
```

Logs all network requests made by the page.

## PARAMETERS

### -Action
Action to take when the event occurs (Accept/Dismiss for dialogs, Ignore to suppress)

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

### -Event
The page event to handle

```yaml
Type: PupPageEvent
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to set the handler on

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
