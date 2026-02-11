using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Http
{
    [Cmdlet(VerbsCommon.Set, "PupHttpAuth")]
    [OutputType(typeof(void))]
    public class SetHttpAuthCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to set authentication for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "Credentials",
            HelpMessage = "Username for HTTP Basic Authentication")]
        public string Username { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            ParameterSetName = "Credentials",
            HelpMessage = "Password for HTTP Basic Authentication")]
        public string Password { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "PSCredential",
            HelpMessage = "PSCredential object for HTTP Basic Authentication")]
        public PSCredential Credential { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Clear",
            HelpMessage = "Clear authentication credentials")]
        public SwitchParameter Clear { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);

                if (Clear.IsPresent)
                {
                    pageService.ClearAuthenticationAsync().GetAwaiter().GetResult();
                    WriteVerbose("Cleared HTTP authentication credentials");
                    return;
                }

                string user, pass;

                if (Credential != null)
                {
                    user = Credential.UserName;
                    pass = Credential.GetNetworkCredential().Password;
                }
                else
                {
                    user = Username;
                    pass = Password;
                }

                pageService.SetAuthenticationAsync(user, pass).GetAwaiter().GetResult();
                WriteVerbose($"Set HTTP authentication for user: {user}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetHttpAuthError", ErrorCategory.InvalidOperation, Page));
            }
        }
    }
}
