using System;
using System.Management.Automation;
using PowerBrowser.Transport;
using PowerBrowser.Services;
public abstract class PageBaseCommand : PSCmdlet
{
    [Parameter(
        Position = 0,
        Mandatory = false,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public PBPage Page { get; set; }

    [Parameter(
        Position = 1,
        Mandatory = false)]
    public string PageId { get; set; }
    protected IPageService PageService => ServiceFactory.CreatePageService(SessionState);
    protected PBPage ResolvePageOrThrow()
    {
        if(Page == null && string.IsNullOrEmpty(PageId))
        {
            ThrowTerminatingError(new ErrorRecord(
                new ArgumentException(
                    "Either BrowserPage or PageId parameter must be provided."),
                    "InvalidParameters",
                    ErrorCategory.InvalidArgument,
                    null
                )
            );
        }
        else if(Page == null)
        {
            Page = PageService.GetPages()
                .Find(page => page.PageId.Equals(PageId, StringComparison.OrdinalIgnoreCase));

            if(Page == null)
            {
                ThrowTerminatingError(new ErrorRecord(
                    new ArgumentException(
                        $"No browser page found with PageId: {PageId}"),
                        "BrowserPageNotFound",
                        ErrorCategory.ObjectNotFound,
                        null
                    )
                );
            } 
        }
        return Page;
    }
}