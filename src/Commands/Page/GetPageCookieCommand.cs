using System;
using System.Management.Automation;
using PowerBrowser.Transport;
using System.Collections.Generic;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PageCookie")]
    [OutputType(typeof(PBCookie[]))]
    public class GetPageCookieCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Filter cookies by name (supports wildcards)")]
        public string Name { get; set; }

        [Parameter(HelpMessage = "Filter cookies by domain (supports wildcards)")]
        public string Domain { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var page = ResolvePageOrThrow();
                var cookies = PageService.GetCookiesAsync(page).GetAwaiter().GetResult();

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