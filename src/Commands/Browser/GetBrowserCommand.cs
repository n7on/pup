using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Browser
{
    [Cmdlet(VerbsCommon.Get, "PupBrowser")]
    [OutputType(typeof(PupBrowser))]
    public class GetBrowserCommand : PupBaseCommand
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
