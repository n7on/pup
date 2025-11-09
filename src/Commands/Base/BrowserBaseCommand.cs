using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Completers;
public abstract class BrowserBaseCommand : PSCmdlet
{
    [Parameter(
        Position = 0,
        Mandatory = false,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public PupBrowser Browser { get; set; }

    [Parameter(
        HelpMessage = "Name of the browser to stop (used when Browser parameter is not provided)",
        Mandatory = false)]
    [ArgumentCompleter(typeof(InstalledBrowserCompleter))]
    public string BrowserType { get; set; }
    protected IBrowserService BrowserService => ServiceFactory.CreateBrowserService(Browser, SessionState);
    protected ISupportedBrowserService SupportedBrowserService => ServiceFactory.CreateSupportedBrowserService(SessionState);
    protected PupBrowser ResolveBrowserOrThrow()
    {
        if (Browser == null && string.IsNullOrEmpty(BrowserType))
        {
            ThrowTerminatingError(new ErrorRecord(
                new ArgumentException("Either Browser or BrowserType parameter must be provided."),
                "InvalidParameters",
                ErrorCategory.InvalidArgument,
                null));
        }

        if (Browser == null)
        {
            BrowserTypeValidator.Validate(BrowserType);
            Browser = SupportedBrowserService.GetBrowser(BrowserType.ToPBSupportedBrowser());
        }

        return Browser;
    }
}
