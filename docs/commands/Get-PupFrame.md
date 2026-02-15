---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupFrame

## SYNOPSIS
Gets frames (iframes) from a page.

## SYNTAX

```
Get-PupFrame -Page <PupPage> [-Name <String>] [-Url <String>] [-IncludeMain] [-First]
 [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Returns child frames (iframes) from a page.
By default excludes the main frame.
Use -IncludeMain to include it.
Frames can be filtered by name or URL pattern.
The returned frame objects can be used with Find-PupElements, Wait-PupElement, Invoke-PupScript, and Get-PupSource.

## EXAMPLES

### Example 1: Get all frames from a page
```
$frames = Get-PupFrame -Page $page
$frames.Count
```

Returns all child frames (iframes) from the page.

### Example 2: Get frame by name
```
$frame = Get-PupFrame -Page $page -Name "contentFrame" -First
```

Gets a specific frame by its name attribute.

### Example 3: Find elements within a frame
```
$frame = $page | Get-PupFrame -Name "loginFrame" -First
$input = Find-PupElements -Frame $frame -Selector "#username" -First
$input | Set-PupElement -Text "user@example.com"
```

Gets a frame and interacts with elements inside it.

### Example 4: Filter frames by URL
```
$frames = Get-PupFrame -Page $page -Url "*ads*"
```

Gets all frames whose URL contains "ads".

## PARAMETERS

### -First
Return only the first matching frame

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

### -IncludeMain
Include the main frame in results

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

### -Name
Filter frames by name

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

### -Page
Page to get frames from

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

### -Url
Filter frames by URL (supports wildcards)

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
