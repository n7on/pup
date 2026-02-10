---
external help file: Pup.dll-Help.xml
Module Name:
online version:
schema: 2.0.0
---

# Start-PupBrowser

## SYNOPSIS
Starts a new browser instance for web automation and pentesting.

## SYNTAX

## DESCRIPTION
Launches a Chromium-based browser with optional proxy, user agent, and stealth mode settings.
The browser can run in headless mode for automated testing or with a visible window for debugging.

## EXAMPLES

### Example 1: Start a headless browser
```
$browser = Start-PupBrowser -Headless
```

Starts a headless browser instance for automated testing.

### Example 2: Start browser with Burp proxy
```
$browser = Start-PupBrowser -Headless -Proxy "127.0.0.1:8080"
```

Routes all traffic through Burp Suite for interception and analysis.

### Example 3: Start browser with custom user agent
```
$browser = Start-PupBrowser -Headless -UserAgent "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X)"
```

Spoofs the user agent to emulate a mobile device.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
