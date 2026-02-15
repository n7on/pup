---
external help file: Pup.dll-Help.xml
Module Name: Pup
online version:
schema: 2.0.0
---

# Get-PupCertificate

## SYNOPSIS
Retrieves SSL/TLS certificate and security details from a page.

## SYNTAX

```
Get-PupCertificate -Page <PupPage> [-Origin <String>] [-UrlFilter <String>] [-Expired]
 [-ExpiringWithin <Int32>] [-Unique] [-ProgressAction <ActionPreference>] [<CommonParameters>]
```

## DESCRIPTION
Gets SSL/TLS certificate information in two modes: passive mode returns security details captured from network requests (protocol, cipher, subject, validity), while active mode fetches the full certificate chain for a specific origin via CDP.
Useful for certificate validation, expiry monitoring, and TLS configuration analysis.

## EXAMPLES

### Example 1: Get all captured security details
```
$certs = Get-PupCertificate -Page $page
$certs | Format-Table SubjectName, Protocol, Cipher, ValidTo
```

Lists TLS security details for all HTTPS requests made by the page.

### Example 2: Filter by URL pattern
```
Get-PupCertificate -Page $page -UrlFilter "api.github.com"
```

Returns security details only for requests matching the URL pattern.

### Example 3: Find certificates expiring soon
```
Get-PupCertificate -Page $page -ExpiringWithin 30 |
    Select-Object SubjectName, ValidTo, DaysUntilExpiry
```

Finds certificates expiring within 30 days for proactive monitoring.

### Example 4: Fetch full certificate for an origin
```
$cert = Get-PupCertificate -Page $page -Origin "https://github.com"
$cert.SubjectName
$cert.Thumbprint
$cert.DaysUntilExpiry
$cert.CertificateChain.Count  # Number of certs in chain
```

Actively fetches the full certificate chain and parses X.509 details including thumbprint and serial number.

### Example 5: Get unique certificates
```
Get-PupCertificate -Page $page -Unique |
    Select-Object SubjectName, Issuer, Protocol
```

Deduplicates results by SubjectName to see unique certificates across all requests.

### Example 6: Check certificate in network entries
```
$network = Get-PupNetwork -Page $page
$network | Where-Object { $_.SecurityDetails } |
    Select-Object Url, @{N="TLS";E={$_.SecurityDetails.Protocol}}
```

Access security details directly from network entries for per-request TLS info.

## PARAMETERS

### -Expired
Only show expired certificates

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -ExpiringWithin
Show certificates expiring within N days

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

### -Origin
Fetch certificate for specific origin (active mode).
Example: https://github.com

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
The page to get certificates from

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

### -Unique
Deduplicate results by SubjectName

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -UrlFilter
Filter captured security details by URL pattern

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
