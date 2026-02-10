---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Install-PupBrowser

## SYNOPSIS
Downloads and installs a browser for automation.

## SYNTAX

```
Install-PupBrowser [-BrowserType <PupSupportedBrowser>] [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Downloads a Chromium-based browser to use with Pup.
Must be run before starting a browser for the first time.

## EXAMPLES

### Example 1: Install Chrome
```
Install-PupBrowser -BrowserType Chrome
```

Downloads and installs Chrome for automation.

### Example 2: Install specific version
```
Install-PupBrowser -BrowserType Chrome -Revision 1234567
```

Installs a specific browser revision.

## PARAMETERS

### -BrowserType
Browser type to install (Chrome, Firefox, or Chromium - use -Headless flag with Start-Browser instead of ChromeHeadlessShell)

```yaml
Type: PupSupportedBrowser
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
