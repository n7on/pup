---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Stop-PupBrowser

## SYNOPSIS
Stops a running browser instance.

## SYNTAX

## DESCRIPTION
Closes the browser and releases all resources.
All pages associated with the browser will be closed.

## EXAMPLES

### Example 1: Stop a browser
```
Stop-PupBrowser -Browser $browser
```

Closes the browser instance.

### Example 2: Stop via pipeline
```
$browser | Stop-PupBrowser
```

Stops the browser using pipeline input.

### Example 3: Stop all running browsers
```
Get-PupBrowser | Where-Object Running | Stop-PupBrowser
```

Finds and stops all running browser instances.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
