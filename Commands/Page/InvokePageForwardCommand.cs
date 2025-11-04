using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageForward")]
    [OutputType(typeof(void))]
    public class InvokePageForwardCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Wait for page to load after navigation")]
        public SwitchParameter WaitForLoad { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var page = ResolvePageOrThrow();
                
                if (WaitForLoad.IsPresent)
                {
                    page.Page.GoForwardAsync(new PuppeteerSharp.NavigationOptions
                    {
                        WaitUntil = new[] { PuppeteerSharp.WaitUntilNavigation.Load, PuppeteerSharp.WaitUntilNavigation.DOMContentLoaded }
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    page.Page.GoForwardAsync().GetAwaiter().GetResult();
                }

                WriteVerbose($"Navigated forward to: {page.Page.Url}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageForwardError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}