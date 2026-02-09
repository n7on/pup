# Pup
Pup is a native PowerShell module made for browser automation. It's build upon PuppeteerSharp, which is a Dotnet library using DevTools API in order to automate the browser. It targets the netstandard 2.0, so it's fully supported on all powershell versions. 

## Install
```powershell
Install-Module Pup
Install-PupBrowser
```

# Use-Cases

## Web Scraping
This example scrape Ubuntu security notices from `https://ubuntu.com/security/notices `. And return the date and link to security issues.


### Start browser 
```powershell
Import-Module Pup
$page = start-PupBrowser | New-PupPage -Url https://ubuntu.com/security/notices 
```
### Get selectors

#### List selector
If you look at the opened page in the browser, you see that there are 10 notices per page. So we need a list that contains all those, so that we can iterate over it. Copy the first link name, which at this time is `USN-8015-3: Linux kernel (FIPS) vulnerabilities`.
```powershell

# Try with different depts, and try to go as deep as possible while at same time catch all 10 items.
$page | find-pupelements -Text "USN-8015-3: Linux kernel (FIPS) vulnerabilities" | get-PupElementPattern -Depth 3

# Type          Selector                   MatchCount Description
# ----          --------                   ---------- -----------
# ByClass       section.p-section--shallow         10 Elements with same tag and…
# ByParentClass .col-9 section                     10 All section inside .col-9
# ByAncestorId  #notices-list section              10 All section under #notices…
# ByStructure   div.col-9 section                  10 Elements in repeating div.…
# ByTag         section                            16 All section elements

$listSelector = "#notices-list section"
```
#### Date selector
If we first get the first element by using the `$listSelector`, we can get the element that holds the date-text, and see its selector.
```powershell
$page | find-pupelements -Selector $listSelector -First | Find-PupElements -TextContains "6 february"

# Selector  : div.row > div.col-6 > p.u-text--muted
# Index     : 0
# FoundTime : 2026-02-09 20:21:09
# TagName   : P
# InnerText : 6 February 2026
# InnerHTML : 6 February 2026
# Id        : 
# IsVisible : False

$dateSelector = "div.row > div.col-6 > p.u-text--muted"
```
#### Link selector

We use the `$listSelector` again, and get the element that holds the link-text, and see its selector.
```powershell
$page | find-pupelements -Selector $listSelector -First | Find-PupElements -TextContains "USN-8015-3: Linux kernel (FIPS) vulnerabilities"

# ElementId : 17fa5859-2293-4913-87db-4fc0049e3f89
# Selector  : div.u-fixed-width > h3.u-no-margin > a
# Index     : 0
# FoundTime : 2026-02-09 20:34:23
# TagName   : A
# InnerText : USN-8015-3: Linux kernel (FIPS) vulnerabilities
# InnerHTML : USN-8015-3: Linux kernel (FIPS) vulnerabilities
# Id        : 
# IsVisible : False

$linkSelector = "div.u-fixed-width > h3.u-no-margin > a"
```
#### NextPage selector 
We need the selector to next page in order to scrape multiple pages.
```powershell
# it doesn't show on the page, but if you look in source it's actually "Next page". 
# the selector from find-pupElements doesn't look great, but if we run the element through Get-PupElementPattern we get better onces. And we can choose anyone which have 1 matches.
$page | find-pupelements -Text "Next page" | Get-PupElementPattern

# Type          Selector                            MatchCount Description
# ----          --------                            ---------- -----------
# ByClass       i.p-icon--chevron-down                       3 Elements with same tag and classes
# ByTag         i                                           13 All i elements
# ByParentClass .p-pagination__link--next i                  1 All i inside .p-pagination__link--next
# ByAncestorId  #notices-list div > ol > li > a > i          1 All i under #notices-list
# ByStructure   li.p-pagination__item a > i                  1 Elements in repeating li.p-pagination__item containers

$nextPageSelector = ".p-pagination__link--next i"
```
### Create script
Now we have everything that is needed for the script. We only need the Ubunty Notices from 1 month back. So we'll stop when we get an older notice.

```powershell
$url = "https://ubuntu.com/security/notices"
$listSelector = "#notices-list section"
$dateSelector = "div.row > div.col-6 > p.u-text--muted"
$linkSelector = "div.u-fixed-width > h3.u-no-margin > a"
$nextPageSelector = ".p-pagination__link--next i"

# check notices from 1 month back.
$fromDate = (Get-Date).AddMonths(-1)
$browser = Start-PupBrowser -Headless
$page = New-PupPage -Url $url

$date = Get-Date

# while found date is not older than 1 month
while ($date -gt $fromDate) {

    $page | Find-PupElements -WaitForLoad -Selector $listSelector | ForEach-Object {
        $date = [datetime]($_ | Find-PupElements -Selector $dateSelector).InnerHTML.Trim()
        $link = ($_ | Find-PupElements -Selector $linkSelector).InnerHTML.Trim()
        [PSCustomObject]@{
            Date  = $date
            Link  = $link
        }
    }
    # click next-page link
    $page | Find-PupElements -Selector $nextPageSelector | Invoke-PupElementClick
}
$browser | Stop-PupBrowser

```
See more examples in [./examples](./examples/)

# Development
If you want to contribute to the source you're highly welcome! 
## Prerequisites

* Dotnet 8
* Pester

## Test
DLLs can't be unloaded from PowerShell, so you need to run tests in a different process, as below.
``` powershell
pwsh -Command "Invoke-Pester ./tests/Browser.Tests.ps1 -Output Detailed"
pwsh -Command "Invoke-Pester ./tests/Page.Tests.ps1 -Output Detailed"
pwsh -Command "Invoke-Pester ./tests/Element.Tests.ps1 -Output Detailed"

# or all tests
pwsh -Command "Invoke-Pester ./tests/ -Output Detailed"
``` 

# Troubleshooting

## Windows
Uninstall the old `Pester`that is shipped with Windows, and never updated.

``` PowerShell
$module = "C:\Program Files\WindowsPowerShell\Modules\Pester"
takeown /F $module /A /R
icacls $module /reset
icacls $module /grant "*S-1-5-32-544:F" /inheritance:d /T
Remove-Item -Path $module -Recurse -Force -Confirm:$false

Install-Module -Name Pester  -MaximumVersion 4.99 -Force

```