---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Get-PupWebSocket

## SYNOPSIS
Lists WebSocket connections and their message frames.

## SYNTAX

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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
