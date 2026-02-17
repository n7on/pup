# Exercise 5: GraphQL Background Requests
# https://web-scraping.dev/reviews
# Fetch reviews directly via the GraphQL API using cursor-based pagination

Import-Module Pup

$Browser = Start-PupBrowser -Headless
$Page = New-PupPage -Url "https://web-scraping.dev/reviews" -WaitForLoad

# Read the GraphQL query template from the page
$GqlQuery = $Page | Invoke-PupScript -Script "document.querySelector('#reviewsGQL')?.textContent" -AsString

if (-not $GqlQuery) {
    $GqlQuery = @"
query GetReviews(`$first: Int, `$after: String) {
  reviews(first: `$first, after: `$after) {
    edges {
      node {
        id
        date
        rating
        text
      }
      cursor
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}
"@
}

Write-Host "Fetching reviews via GraphQL API..."

$AllReviews = @()
$Cursor = $null
$PageNum = 0

do {
    $PageNum++
    $Variables = @{ first = 10 }
    if ($Cursor) { $Variables.after = $Cursor }

    $Body = @{
        query     = $GqlQuery
        variables = $Variables
    }

    $Response = $Page | Invoke-PupHttpFetch `
        -Url "https://web-scraping.dev/api/graphql" `
        -Method POST `
        -Body $Body `
        -AsJson

    if (-not $Response.Ok) {
        Write-Host "GraphQL request failed: $($Response.Status)"
        break
    }

    $Data = $Response.Body.data.reviews
    $Edges = $Data.edges

    foreach ($Edge in $Edges) {
        $AllReviews += [PSCustomObject]@{
            Id     = $Edge.node.id
            Date   = $Edge.node.date
            Rating = $Edge.node.rating
            Text   = $Edge.node.text
        }
    }

    Write-Host "Page $PageNum`: fetched $($Edges.Count) reviews (total: $($AllReviews.Count))"

    $HasNextPage = $Data.pageInfo.hasNextPage
    $Cursor = $Data.pageInfo.endCursor
} while ($HasNextPage)

Write-Host "`nTotal reviews: $($AllReviews.Count)"
$AllReviews | Format-Table -AutoSize

$Browser | Stop-PupBrowser
