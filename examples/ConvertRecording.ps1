# Convert a Chrome DevTools recording to a PowerShell script
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$RecordingPath,
    [string]$OutputPath
)

Import-Module Pup
# The recording should be in the format exported by Chrome DevTools Performance panel (JSON with "traceEvents" array)
# Convert the JSON recording to PowerShell
$Script = Convert-PupRecording -InputFile $RecordingPath

if ($OutputPath) {
    $Script | Set-Content -Path $OutputPath
    Write-Host "Script saved to $OutputPath"
} else {
    $Script
}
