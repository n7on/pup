using System;
using System.Management.Automation;
using Pup.Transport;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPageCookie")]
    [OutputType(typeof(PupCookie[]))]
    public class GetPageCookieCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get cookies from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Filter cookies by name (supports wildcards)")]
        public string Name { get; set; }

        [Parameter(HelpMessage = "Filter cookies by domain (supports wildcards)")]
        public string Domain { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var cookies = pageService.GetCookiesAsync().GetAwaiter().GetResult();

                // Apply filters if provided
                if (!string.IsNullOrEmpty(Name))
                {
                    var namePattern = new WildcardPattern(Name, WildcardOptions.IgnoreCase);
                    cookies = cookies.FindAll(c => namePattern.IsMatch(c.Name));
                }

                if (!string.IsNullOrEmpty(Domain))
                {
                    var domainPattern = new WildcardPattern(Domain, WildcardOptions.IgnoreCase);
                    cookies = cookies.FindAll(c => domainPattern.IsMatch(c.Domain ?? ""));
                }

                WriteObject(cookies.ToArray(), true);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageCookieError", ErrorCategory.ReadError, null));
            }
        }
    }
}