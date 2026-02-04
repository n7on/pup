[CmdletBinding()]
param(
    [datetime]$FromDate = (Get-Date).AddMonths(-1)
)

Import-Module Pup
Install-PupBrowser

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://ubuntu.com/security/notices" 

$Date = Get-Date
while ($Date -gt $FromDate) {

    $Page | Find-PupElements -WaitForLoad -Selector "#notices-list > section" | ForEach-Object {
        $Date = [datetime]($_ | Find-PupElements -Selector ".row > div.col-6 > p:first-child").InnerHTML.Trim()
        $Link = ($_ | Find-PupElements -Selector ".u-fixed-width > h3").InnerHTML.Trim()
        [PSCustomObject]@{
            Date  = $Date
            Link  = $Link
        }
    }
    $Page | Find-PupElements -Selector "a.p-pagination__link--next" | Invoke-PupElementClick
}
$Browser | Stop-PupBrowser