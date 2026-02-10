---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Send-PupWebSocketMessage

## SYNOPSIS
Sends a message through an active WebSocket connection.

## SYNTAX

## DESCRIPTION
Injects a message into an open WebSocket connection on the page.
Useful for testing WebSocket-based APIs and real-time features.

## EXAMPLES

### Example 1: Send a simple message
```
Send-PupWebSocketMessage -Page $page -Message '{"action":"ping"}'
```

Sends a JSON message through the first available WebSocket.

### Example 2: Target specific WebSocket by URL
```
Send-PupWebSocketMessage -Page $page -Message '{"cmd":"admin"}' -Url "api.target.com"
```

Sends a message to a specific WebSocket matching the URL pattern.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
