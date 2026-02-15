# Pup

[![CI](https://github.com/n7on/Pup/actions/workflows/ci.yml/badge.svg)](https://github.com/n7on/Pup/actions/workflows/ci.yml)
[![PowerShell Gallery Version](https://img.shields.io/powershellgallery/v/Pup)](https://www.powershellgallery.com/packages/Pup)
[![PowerShell Gallery Downloads](https://img.shields.io/powershellgallery/dt/Pup)](https://www.powershellgallery.com/packages/Pup)
[![License](https://img.shields.io/github/license/n7on/Pup)](https://github.com/n7on/Pup/blob/main/LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-blue)](#)

A PowerShell module for browser automation using the Chrome DevTools Protocol. Built on PuppeteerSharp, it works with PowerShell 5.1+ on Windows, Linux, and macOS. 

## Install
```powershell
Install-Module Pup
Install-PupBrowser
```

## Commands

### Browser
- [Start-PupBrowser](./docs/commands/Start-PupBrowser.md) - Launch browser instance
- [Stop-PupBrowser](./docs/commands/Stop-PupBrowser.md) - Close browser
- [Get-PupBrowser](./docs/commands/Get-PupBrowser.md) - Get running browsers
- [Install-PupBrowser](./docs/commands/Install-PupBrowser.md) - Download browser
- [Uninstall-PupBrowser](./docs/commands/Uninstall-PupBrowser.md) - Remove browser

### Page
- [New-PupPage](./docs/commands/New-PupPage.md) - Create new page/tab
- [Get-PupPage](./docs/commands/Get-PupPage.md) - Get open pages
- [Remove-PupPage](./docs/commands/Remove-PupPage.md) - Close page
- [Move-PupPage](./docs/commands/Move-PupPage.md) - Navigate to URL
- [Invoke-PupPageBack](./docs/commands/Invoke-PupPageBack.md) - Go back
- [Invoke-PupPageForward](./docs/commands/Invoke-PupPageForward.md) - Go forward
- [Invoke-PupPageReload](./docs/commands/Invoke-PupPageReload.md) - Reload page
- [Invoke-PupPageScroll](./docs/commands/Invoke-PupPageScroll.md) - Scroll page
- [Invoke-PupScript](./docs/commands/Invoke-PupScript.md) - Execute JavaScript
- [Set-PupViewport](./docs/commands/Set-PupViewport.md) - Set viewport size
- [Get-PupSource](./docs/commands/Get-PupSource.md) - Get page HTML

### Elements
- [Find-PupElements](./docs/commands/Find-PupElements.md) - Find elements by selector
- [Wait-PupElement](./docs/commands/Wait-PupElement.md) - Wait for element
- [Invoke-PupElementClick](./docs/commands/Invoke-PupElementClick.md) - Click element
- [Invoke-PupElementHover](./docs/commands/Invoke-PupElementHover.md) - Hover over element
- [Invoke-PupElementFocus](./docs/commands/Invoke-PupElementFocus.md) - Focus element
- [Invoke-PupElementScroll](./docs/commands/Invoke-PupElementScroll.md) - Scroll element
- [Set-PupElement](./docs/commands/Set-PupElement.md) - Set element text
- [Set-PupElementValue](./docs/commands/Set-PupElementValue.md) - Set element value
- [Set-PupElementAttribute](./docs/commands/Set-PupElementAttribute.md) - Set attribute
- [Get-PupElementValue](./docs/commands/Get-PupElementValue.md) - Get element value
- [Get-PupElementAttribute](./docs/commands/Get-PupElementAttribute.md) - Get attribute
- [Get-PupElementSelector](./docs/commands/Get-PupElementSelector.md) - Get CSS selector
- [Get-PupElementPattern](./docs/commands/Get-PupElementPattern.md) - Get selector patterns
- [Select-PupElementOption](./docs/commands/Select-PupElementOption.md) - Select dropdown option

### Recording
- [Start-PupRecording](./docs/commands/Start-PupRecording.md) - Start recording interactions
- [Stop-PupRecording](./docs/commands/Stop-PupRecording.md) - Stop recording
- [Get-PupRecording](./docs/commands/Get-PupRecording.md) - Get recorded events
- [Clear-PupRecording](./docs/commands/Clear-PupRecording.md) - Clear recorded events
- [ConvertTo-PupScript](./docs/commands/ConvertTo-PupScript.md) - Convert events to script

### Screenshots & Export
- [Get-PupPageScreenshot](./docs/commands/Get-PupPageScreenshot.md) - Capture page screenshot
- [Get-PupElementScreenshot](./docs/commands/Get-PupElementScreenshot.md) - Capture element screenshot
- [Export-PupPdf](./docs/commands/Export-PupPdf.md) - Export page as PDF

### Cookies & Storage
- [Get-PupCookie](./docs/commands/Get-PupCookie.md) - Get cookies
- [Set-PupCookie](./docs/commands/Set-PupCookie.md) - Set cookie
- [Remove-PupCookie](./docs/commands/Remove-PupCookie.md) - Remove cookies
- [Get-PupStorage](./docs/commands/Get-PupStorage.md) - Get local/session storage
- [Set-PupStorage](./docs/commands/Set-PupStorage.md) - Set storage item
- [Clear-PupStorage](./docs/commands/Clear-PupStorage.md) - Clear storage

### Network
- [Invoke-PupHttpFetch](./docs/commands/Invoke-PupHttpFetch.md) - Make HTTP request
- [Set-PupHttpHeader](./docs/commands/Set-PupHttpHeader.md) - Set request headers
- [Set-PupHttpAuth](./docs/commands/Set-PupHttpAuth.md) - Set HTTP authentication
- [Get-PupNetwork](./docs/commands/Get-PupNetwork.md) - Get network requests

### WebSocket
- [Get-PupWebSocket](./docs/commands/Get-PupWebSocket.md) - Get WebSocket connections
- [Send-PupWebSocketMessage](./docs/commands/Send-PupWebSocketMessage.md) - Send WebSocket message

### Session
- [Export-PupSession](./docs/commands/Export-PupSession.md) - Export session (cookies, storage)
- [Import-PupSession](./docs/commands/Import-PupSession.md) - Import session

### Input
- [Send-PupKey](./docs/commands/Send-PupKey.md) - Send keyboard input
- [Send-PupFile](./docs/commands/Send-PupFile.md) - Upload file

### Console & Debugging
- [Enter-PupConsole](./docs/commands/Enter-PupConsole.md) - Interactive console mode
- [Get-PupConsole](./docs/commands/Get-PupConsole.md) - Get console messages
- [Invoke-PupCdpMessage](./docs/commands/Invoke-PupCdpMessage.md) - Send raw CDP command
- [Get-PupCertificate](./docs/commands/Get-PupCertificate.md) - Get page certificate
- [Set-PupDialogHandler](./docs/commands/Set-PupDialogHandler.md) - Handle dialogs (alert, confirm)

## Examples

### Web Scraping
This example scrapes Ubuntu security notices from `https://ubuntu.com/security/notices` and returns the date and link to security issues.


#### Start browser 
```powershell
Import-Module Pup
$page = start-PupBrowser | New-PupPage -Url https://ubuntu.com/security/notices 
```
#### Get selectors

##### List selector
If you look at the opened page in the browser, you see that there are 10 notices per page. So we need a list that contains all those, so that we can iterate over it. Copy the first link name, which at this time is `USN-8015-3: Linux kernel (FIPS) vulnerabilities`.
```powershell

# Try with different depths, and try to go as deep as possible while at the same time catching all 10 items.
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
##### Date selector
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
##### Link selector

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
##### NextPage selector 
We need the selector to next page in order to scrape multiple pages.
```powershell
# it doesn't show on the page, but if you look in source it's actually "Next page". 
# the selector from Find-PupElements doesn't look great, but if we run the element through Get-PupElementPattern we get better ones. We can choose any with 1 match.
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
#### Create script
Now we have everything that is needed for the script. We only need the Ubuntu notices from the last month, so we'll stop when we get an older notice.

```powershell
$url = "https://ubuntu.com"
$listSelector = "#notices-list section"
$dateSelector = "div.row > div.col-6 > p.u-text--muted"
$linkSelector = "div.u-fixed-width > h3.u-no-margin > a"
$nextPageSelector = ".p-pagination__link--next i"

# check notices from 1 month back.
$fromDate = (Get-Date).AddMonths(-1)
$browser = Start-PupBrowser -Headless
$page = New-PupPage -Url "$url/security/notices"

$date = Get-Date

# while found date is not older than 1 month
while ($date -gt $fromDate) {

    $page | Find-PupElements -WaitForLoad -Selector $listSelector | ForEach-Object {
        $date = [datetime]($_ | Find-PupElements -Selector $dateSelector).InnerHTML.Trim()
        $href = $_ | Find-PupElements -Selector $linkSelector | Get-PupElementAttribute -Name href
        [PSCustomObject]@{
            Date  = $date
            Link  = "$url$href"
        }
    }
    # click next-page link
    $page | Find-PupElements -Selector $nextPageSelector | Invoke-PupElementClick
}
$browser | Stop-PupBrowser

```
See more examples in [./examples](./examples/)

## Contributing

See [CONTRIBUTING.md](./CONTRIBUTING.md) for development setup, testing, and troubleshooting.