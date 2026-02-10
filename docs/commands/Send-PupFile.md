---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Send-PupFile

## SYNOPSIS
Uploads files through a file input element.

## SYNTAX

```
Send-PupFile -Element <PupElement> -FilePath <String[]> [-ProgressAction <ActionPreference>]
 [<CommonParameters>]
```

## DESCRIPTION
Sets the files for a file input element, simulating a user selecting files for upload.
Supports single and multiple file uploads.

## EXAMPLES

### Example 1: Upload a single file
```
$fileInput = Find-PupElements -Page $page -Selector "input[type=file]" -First
Send-PupFile -Element $fileInput -FilePath "C:\payloads\test.pdf"
```

Uploads a PDF file through the file input.

### Example 2: Upload multiple files
```
$fileInput = Find-PupElements -Page $page -Selector "#attachments" -First
Send-PupFile -Element $fileInput -FilePath "doc1.pdf", "doc2.pdf", "image.png"
```

Uploads multiple files at once (for inputs with 'multiple' attribute).

### Example 3: Test file upload vulnerabilities
```
$fileInput = Find-PupElements -Page $page -Selector "input[type=file]" -First
Send-PupFile -Element $fileInput -FilePath "shell.php.jpg"
$submitBtn = Find-PupElements -Page $page -Selector "#upload-btn" -First
$submitBtn | Invoke-PupElementClick
```

Tests file upload bypass with double extension.

## PARAMETERS

### -Element
The file input element to upload files to

```yaml
Type: PupElement
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -FilePath
Path(s) to the file(s) to upload

```yaml
Type: String[]
Parameter Sets: (All)
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
