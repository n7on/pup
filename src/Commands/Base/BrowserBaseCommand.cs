using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Completers;
using Pup.Services;

namespace Pup.Commands.Base
{

    public abstract class BrowserBaseCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = false,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The browser instance")]
        public PupBrowser Browser { get; set; }

        [Parameter(
            HelpMessage = "Name of the browser to stop (used when Browser parameter is not provided)",
            Mandatory = false)]
        [ArgumentCompleter(typeof(InstalledBrowserCompleter))]
        public string BrowserType { get; set; } = "Chrome";
        protected IBrowserService BrowserService => ServiceFactory.CreateBrowserService(Browser);
        protected ISupportedBrowserService SupportedBrowserService => ServiceFactory.CreateSupportedBrowserService();
        protected PupBrowser ResolveBrowserOrThrow()
        {

            if (Browser == null)
            {
                BrowserTypeValidator.Validate(BrowserType);
                Browser = SupportedBrowserService.GetBrowser(BrowserType.ToPBSupportedBrowser());
            }

            return Browser;
        }
    }
}
