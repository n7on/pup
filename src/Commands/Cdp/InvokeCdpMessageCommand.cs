using System;
using System.Collections;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Cdp
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupCdpMessage")]
    [OutputType(typeof(PSObject))]
    public class InvokeCdpMessageCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to send the CDP message through")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The CDP method to invoke (e.g., 'DOM.getDocument', 'Network.enable')")]
        public string Method { get; set; }

        [Parameter(
            Position = 2,
            HelpMessage = "Parameters to pass to the CDP method")]
        public Hashtable Parameters { get; set; }

        [Parameter(HelpMessage = "Return raw JSON string instead of parsed object")]
        public SwitchParameter AsJson { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var cdpService = ServiceFactory.CreateCdpService(Page);

                if (AsJson.IsPresent)
                {
                    var result = cdpService.SendRawAsync(Method, Parameters).GetAwaiter().GetResult();
                    WriteObject(result);
                }
                else
                {
                    var result = cdpService.SendAsync(Method, Parameters).GetAwaiter().GetResult();
                    WriteObject(result);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                WriteError(new ErrorRecord(
                    new Exception($"CDP call failed: {errorMessage}", ex),
                    "CdpMessageFailed",
                    ErrorCategory.ProtocolError,
                    Method));
            }
        }
    }
}
