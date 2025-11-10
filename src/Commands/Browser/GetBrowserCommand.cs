using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsCommon.Get, "PupBrowser")]
    [OutputType(typeof(PupBrowser))]
    public class GetBrowserCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            try
            {
                var browsers = ServiceFactory.CreateSupportedBrowserService(SessionState).GetBrowsers();

                if (browsers.Count == 0)
                {
                    WriteWarning("No browsers found.");
                    return;
                }

                foreach (var browser in browsers)
                {
                    WriteObject(browser);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetBrowserFailed", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
