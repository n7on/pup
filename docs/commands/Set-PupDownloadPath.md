---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Set-PupDownloadPath

## SYNOPSIS
Configures the download directory for a page, or disables downloads.

## SYNTAX

### Allow
```
Set-PupDownloadPath -Page <PupPage> -Path <String> [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

### Deny
```
Set-PupDownloadPath -Page <PupPage> [-Deny] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Sets the download behavior for a browser page via the Chrome DevTools Protocol.
When a path is specified, files downloaded by the page are saved to that directory.
Use -Deny to disable downloads.
Pair with Set-PupPageHandler -Event Download to receive notifications when downloads start or complete.

## EXAMPLES

### Example 1: Enable downloads to a directory
```
$page | Set-PupDownloadPath -Path "C:\downloads\"
```

Configures the page to save downloaded files to C:\downloads\.

### Example 2: Handle download completion
```
$page | Set-PupDownloadPath -Path "C:\downloads\"
$page | Set-PupPageHandler -Event Download -ScriptBlock {
    if ($_.State -eq "completed") {
        Write-Host "Downloaded: $($_.SuggestedFilename) ($($_.TotalBytes) bytes)"
    }
}
```

Sets up a download directory and a handler that reports when downloads finish.

### Example 3: Disable downloads
```
$page | Set-PupDownloadPath -Deny
```

Disables file downloads for the page.

## PARAMETERS

### -Deny
Disable downloads

```yaml
Type: SwitchParameter
Parameter Sets: Deny
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Page
The page to configure downloads for

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

### -Path
Directory path to save downloads to

```yaml
Type: String
Parameter Sets: Allow
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

## OUTPUTS

## NOTES

## RELATED LINKS
