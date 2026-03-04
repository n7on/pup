using System;
using System.Collections;
using System.Management.Automation;
using Pup.Common;
using Pup.Completers;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Cdp
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupCdpMessage", DefaultParameterSetName = "FromPage")]
    [OutputType(typeof(PSObject))]
    public class InvokeCdpMessageCommand : PupBaseCommand
    {
        [Parameter(
            ParameterSetName = "FromPage",
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to send the CDP message through")]
        public PupPage Page { get; set; }

        [Parameter(
            ParameterSetName = "FromBrowser",
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The browser to send the CDP message through (for browser-level domains like SystemInfo)")]
        public PupBrowser Browser { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The CDP method to invoke (e.g., 'DOM.getDocument', 'Network.enable')")]
        [ArgumentCompleter(typeof(CdpMethodCompleter))]
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
                var cdpService = ParameterSetName == "FromBrowser"
                    ? ServiceFactory.CreateCdpService(Browser)
                    : ServiceFactory.CreateCdpService(Page);

                if (AsJson.IsPresent)
                {
                    var result = Await(cdpService.SendRawAsync(Method, Parameters));
                    WriteObject(result);
                }
                else
                {
                    var result = Await(cdpService.SendAsync(Method, Parameters));
                    WriteObject(result);
                }
            }
            catch (PipelineStoppedException) { throw; }
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
