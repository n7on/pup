using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageBack")]
    [OutputType(typeof(void))]
    public class InvokePageBackCommand : PageBaseCommand
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
                    page.Page.GoBackAsync(new PuppeteerSharp.NavigationOptions
                    {
                        WaitUntil = new[] { PuppeteerSharp.WaitUntilNavigation.Load, PuppeteerSharp.WaitUntilNavigation.DOMContentLoaded }
                    }).GetAwaiter().GetResult();
                }
                else
                {
                    page.Page.GoBackAsync().GetAwaiter().GetResult();
                }

                WriteVerbose($"Navigated back to: {page.Page.Url}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageBackError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}