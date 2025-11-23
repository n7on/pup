# PowerShell functions for the Pup module

function Invoke-PupPageSequence {
    <#
    .SYNOPSIS
    Execute a sequence of actions on a page with automatic waiting
    
    .DESCRIPTION
    Combines common page operations with built-in waits and error handling
    
    .PARAMETER Page
    The PupPage to operate on
    
    .PARAMETER Actions
    Array of action hashtables with Type, Selector, Value, etc.
    
    .EXAMPLE
    $actions = @(
        @{Type='Click'; Selector='#login-button'},
        @{Type='Type'; Selector='#username'; Value='user@example.com'},
        @{Type='Type'; Selector='#password'; Value='mypassword'},
        @{Type='Click'; Selector='#submit'}
    )
    Invoke-PupPageSequence -Page $page -Actions $actions
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [Pup.Transport.PupPage]$Page,
        
        [Parameter(Mandatory)]
        [hashtable[]]$Actions,
        
        [int]$WaitBetweenActions = 1000
    )
    
    foreach ($action in $Actions) {
        Write-Verbose "Executing action: $($action.Type) on $($action.Selector)"
        
        switch ($action.Type) {
            'Click' {
                $element = Find-PupElements -Page $Page -Selector $action.Selector -First -WaitForLoad
                if ($element) { Invoke-PupElementClick -Element $element }
            }
            'Type' {
                $element = Find-PupElements -Page $Page -Selector $action.Selector -First -WaitForLoad
                if ($element) { Set-PupElement -Element $element -Text $action.Value }
            }
            'Wait' {
                Wait-PupElement -Page $Page -Selector $action.Selector -Timeout ($action.Timeout ?? 5000)
            }
            'Screenshot' {
                Get-PupPageScreenshot -Page $Page -Path ($action.Path ?? "screenshot-$(Get-Date -Format 'yyyyMMdd-HHmmss').png")
            }
        }
        
        if ($WaitBetweenActions -gt 0) {
            Start-Sleep -Milliseconds $WaitBetweenActions
        }
    }
}

function Start-PupSession {
    <#
    .SYNOPSIS
    Start a browser session with common configuration
    
    .DESCRIPTION
    Convenience function that starts browser, creates page, and optionally navigates
    
    .PARAMETER Url
    URL to navigate to immediately
    
    .PARAMETER Visible
    Whether to show the browser window
    
    .PARAMETER Width
    Browser window width
    
    .PARAMETER Height  
    Browser window height
    
    .EXAMPLE
    $session = Start-PupSession -Url 'https://example.com' -Visible
    # Returns hashtable with Browser and Page objects
    #>
    [CmdletBinding()]
    param(
        [string]$Url = 'about:blank',
        [switch]$Visible,
        [int]$Width = 1280,
        [int]$Height = 720
    )
    
    $browser = Start-PupBrowser -Headless:(!$Visible.IsPresent)
    $page = New-PupPage -Browser $browser -Url $Url -Width $Width -Height $Height -WaitForLoad
    
    return @{
        Browser = $browser
        Page = $page
    }
}

function Stop-PupSession {
    <#
    .SYNOPSIS
    Clean up a browser session
    
    .DESCRIPTION
    Stops browser from a session object created by Start-PupSession
    
    .PARAMETER Session
    Session object returned by Start-PupSession
    
    .EXAMPLE
    $session = Start-PupSession -Url 'https://example.com'
    # ... do work ...
    Stop-PupSession -Session $session
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        [hashtable]$Session
    )
    
    if ($Session.Browser) {
        Stop-PupBrowser -Browser $Session.Browser
    }
}

function Get-PupElementText {
    <#
    .SYNOPSIS
    Extract text content from multiple elements
    
    .DESCRIPTION
    Convenience function to get text from elements matching a selector
    
    .PARAMETER Page
    The page to search
    
    .PARAMETER Element
    Parent element to search within
    
    .PARAMETER Selector
    CSS selector to find elements
    
    .EXAMPLE
    Get-PupElementText -Page $page -Selector 'h2' | ForEach-Object { "Found heading: $_" }
    #>
    [CmdletBinding()]
    param(
        [Parameter(ParameterSetName='FromPage')]
        [Pup.Transport.PupPage]$Page,
        
        [Parameter(ParameterSetName='FromElement')]
        [Pup.Transport.PupElement]$Element,
        
        [Parameter(Mandatory)]
        [string]$Selector
    )
    
    if ($Page) {
        $elements = Find-PupElements -Page $Page -Selector $Selector
    } else {
        $elements = Find-PupElements -Element $Element -Selector $Selector
    }
    
    return $elements | ForEach-Object { $_.InnerText }
}

function Get-PupBestSelector {
    <#
    .SYNOPSIS
    Get the best CSS selector for an element with intelligent recommendations
    
    .DESCRIPTION
    Analyzes an element and provides multiple selector options with recommendations
    
    .PARAMETER Element
    The element to analyze
    
    .PARAMETER ShowAll
    Show all selector variations
    
    .EXAMPLE
    $element = Find-PupElements -Page $page -Selector 'button' -First
    Get-PupBestSelector -Element $element -ShowAll
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [Pup.Transport.PupElement]$Element,
        
        [switch]$ShowAll
    )
    
    $selectors = @{}
    
    # Get different selector types
    $selectors.Default = Get-PupElementSelector -Element $Element
    $selectors.Unique = Get-PupElementSelector -Element $Element -Unique
    $selectors.Short = Get-PupElementSelector -Element $Element -Short
    $selectors.FullPath = Get-PupElementSelector -Element $Element -FullPath
    
    if ($ShowAll) {
        Write-Host "Selector Options for element:" -ForegroundColor Green
        Write-Host "  Tag: $($Element.TagName)" -ForegroundColor Gray
        Write-Host "  Text: $($Element.InnerText.Substring(0, [Math]::Min(50, $Element.InnerText.Length)))" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Available selectors:" -ForegroundColor Yellow
        Write-Host "  Default:   $($selectors.Default)" -ForegroundColor White
        Write-Host "  Unique:    $($selectors.Unique)" -ForegroundColor Cyan
        Write-Host "  Short:     $($selectors.Short)" -ForegroundColor Green
        Write-Host "  FullPath:  $($selectors.FullPath)" -ForegroundColor Magenta
        
        # Recommend best selector
        if ($selectors.Unique -match '^#') {
            Write-Host "  → Recommended: Unique (has ID)" -ForegroundColor Green
            return $selectors.Unique
        } elseif ($selectors.Short.Length -lt $selectors.Default.Length) {
            Write-Host "  → Recommended: Short (more concise)" -ForegroundColor Green  
            return $selectors.Short
        } else {
            Write-Host "  → Recommended: Default (balanced)" -ForegroundColor Green
            return $selectors.Default
        }
    } else {
        # Return best selector automatically
        if ($selectors.Unique -match '^#') {
            return $selectors.Unique  # ID selectors are most reliable
        } elseif ($selectors.Short.Length -lt $selectors.Default.Length) {
            return $selectors.Short   # Prefer shorter if significantly smaller
        } else {
            return $selectors.Default # Default balance
        }
    }
}

function Find-PupSimilarElements {
    <#
    .SYNOPSIS
    Find all elements at the same hierarchical level as a reference element
    
    .DESCRIPTION
    Takes a reference element and finds all elements that exist at the same structural level in the DOM.
    Perfect for web scraping when you find one item and need to get all similar items that share the same hierarchy.
    
    .PARAMETER ReferenceElement
    The element to use as reference for finding structurally similar elements
    
    .PARAMETER ShowSelector
    Display the generated selector used
    
    .EXAMPLE
    # Find one product in a list, then get all products at same hierarchical level
    $firstProduct = Find-PupElements -Page $page -Selector '.product-card' -First
    $allProducts = Find-PupSimilarElements -ReferenceElement $firstProduct
    # This finds all .product-card elements in the same container
    
    .EXAMPLE  
    # Find all navigation links at same hierarchical level as a reference link
    $navLink = Find-PupElements -Page $page -Selector 'nav a' -First
    $allNavLinks = Find-PupSimilarElements -ReferenceElement $navLink -ShowSelector
    #>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [Pup.Transport.PupElement]$ReferenceElement,
        
        [switch]$ShowSelector
    )
    
    # Generate selector for elements at same hierarchical level
    $selector = Get-PupElementSelector -Element $ReferenceElement -Similar
    
    if ($ShowSelector) {
        Write-Host "Using selector: $selector" -ForegroundColor Green
    }
    
    # Find all matching elements on the same page
    return Find-PupElements -Page $ReferenceElement.Page -Selector $selector
}

# Export functions
Export-ModuleMember -Function 'Invoke-PupPageSequence', 'Start-PupSession', 'Stop-PupSession', 'Get-PupElementText', 'Get-PupBestSelector', 'Find-PupSimilarElements'