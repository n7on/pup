using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageReload")]
    [OutputType(typeof(void))]
    public class InvokePageReloadCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Wait for page to load after reload")]
        public SwitchParameter WaitForLoad { get; set; }

        [Parameter(HelpMessage = "Ignore cache and force reload from server")]
        public SwitchParameter HardReload { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var page = ResolvePageOrThrow();
                
                var reloadOptions = new PuppeteerSharp.NavigationOptions();
                
                if (WaitForLoad.IsPresent)
                {
                    reloadOptions.WaitUntil = new[] { PuppeteerSharp.WaitUntilNavigation.Load, PuppeteerSharp.WaitUntilNavigation.DOMContentLoaded };
                }

                if (HardReload.IsPresent)
                {
                    // Hard reload: clear cache and reload
                    page.Page.ReloadAsync(reloadOptions).GetAwaiter().GetResult();
                }
                else
                {
                    // Normal reload
                    page.Page.ReloadAsync(reloadOptions).GetAwaiter().GetResult();
                }

                WriteVerbose($"Page reloaded: {page.Page.Url}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageReloadError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}