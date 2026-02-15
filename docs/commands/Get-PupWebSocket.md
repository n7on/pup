---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupWebSocket

## SYNOPSIS
Lists WebSocket connections and their message frames.

## SYNTAX

```
Get-PupWebSocket -Page <PupPage> [-Url <String>] [-Active] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Returns all WebSocket connections made by the page, including connection state, URL, and all sent/received message frames.
Useful for testing real-time APIs.

## EXAMPLES

### Example 1: List all WebSocket connections
```
$websockets = Get-PupWebSocket -Page $page
$websockets | Format-List Url, State, @{N="FrameCount";E={$_.Frames.Count}}
```

Shows all WebSocket connections with their state and frame count.

### Example 2: View WebSocket messages
```
$ws = Get-PupWebSocket -Page $page | Select-Object -First 1
$ws.Frames | Format-Table Direction, Timestamp, PayloadData
```

Displays all messages sent and received through the WebSocket.

### Example 3: Filter active connections
```
$active = Get-PupWebSocket -Page $page -Active
$active | Where-Object { $_.Url -like "*chat*" }
```

Gets only open WebSocket connections and filters for chat endpoints.

## PARAMETERS

### -Active
Only show active (open) connections

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
The page to get WebSocket connections from

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

### -Url
Filter by URL pattern (substring match)

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
