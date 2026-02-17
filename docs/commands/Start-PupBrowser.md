---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Start-PupBrowser

## SYNOPSIS
Starts a new browser instance for web automation and pentesting.

## SYNTAX

```
Start-PupBrowser [-BrowserType <String>] [-Headless] [-Fullscreen] [-Maximized] [-Arguments <String[]>]
 [-Proxy <String>] [-UserAgent <String>] [-Width <Int32>] [-Height <Int32>] [-Force]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

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

### -Arguments
Additional arguments to pass to the browser

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
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

### -Force
Force start a new browser even if one is already running

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

### -Fullscreen
Start browser in fullscreen mode (hides browser UI)

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

### -Headless
Run browser in headless mode (no GUI)

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

### -Height
Viewport height (if not specified, viewport auto-resizes with the browser window)

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Maximized
Start browser maximized

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

### -Proxy
Proxy server URL (e.g., 127.0.0.1:8080 for Burp Suite)

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

### -UserAgent
Custom User-Agent string.
Defaults to a realistic Chrome UA.
Use 'none' for the browser's native UA.

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

### -Width
Viewport width (if not specified, viewport auto-resizes with the browser window)

```yaml
Type: Int32
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
