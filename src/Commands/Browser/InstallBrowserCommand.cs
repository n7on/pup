using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsLifecycle.Install, "PupBrowser")]
    [OutputType(typeof(PupBrowser))]
    public class InstallBrowserCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            HelpMessage = "Browser type to install (Chrome, Firefox, or Chromium - use -Headless flag with Start-Browser instead of ChromeHeadlessShell)")]
        public PupSupportedBrowser BrowserType { get; set; } = PupSupportedBrowser.Chrome;

        protected override void ProcessRecord()
        {
            try
            {
                var browserService = ServiceFactory.CreateSupportedBrowserService();

                
                // Check if browser is already installed
                if (!browserService.IsBrowserTypeInstalled(BrowserType))
                {
                    WriteVerbose($"Downloading {BrowserType}... This may take a few minutes depending on your connection.");
                    browserService.DownloadBrowser(BrowserType);
                }
                else
                {
                    WriteVerbose($"{BrowserType} is already installed.");
                }
                
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InstallBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
