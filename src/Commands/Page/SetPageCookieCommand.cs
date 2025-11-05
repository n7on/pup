using System;
using System.Management.Automation;
using PowerBrowser.Transport;
using PowerBrowser.Common;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Set, "PageCookie")]
    [OutputType(typeof(void))]
    public class SetPageCookieCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            HelpMessage = "Name of the cookie")]
        public string Name { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Value of the cookie")]
        public string Value { get; set; }

        [Parameter(HelpMessage = "Domain for the cookie (defaults to current page domain)")]
        public string Domain { get; set; }

        [Parameter(HelpMessage = "Path for the cookie (defaults to '/')")]
        public string Path { get; set; } = "/";

        [Parameter(HelpMessage = "Expiration date for the cookie")]
        public DateTime? Expires { get; set; }

        [Parameter(HelpMessage = "Mark cookie as HTTP only")]
        public SwitchParameter HttpOnly { get; set; }

        [Parameter(HelpMessage = "Mark cookie as secure (HTTPS only)")]
        public SwitchParameter Secure { get; set; }

        [Parameter(HelpMessage = "SameSite policy for the cookie")]
        public PBSameSite? SameSite { get; set; }

        [Parameter(
            ValueFromPipeline = true,
            HelpMessage = "Cookie object(s) to set")]
        public PBCookie[] Cookies { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var page = ResolvePageOrThrow();

                if (Cookies != null && Cookies.Length > 0)
                {
                    // Set multiple cookies from pipeline
                    PageService.SetCookiesAsync(page, Cookies).GetAwaiter().GetResult();
                }
                else if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Value))
                {

                    bool? httpOnlyValue = null;
                    if (HttpOnly.IsPresent)
                    {
                        httpOnlyValue = HttpOnly.ToBool();
                    }

                    bool? secureValue = null;
                    if (Secure.IsPresent)
                    {
                        secureValue = Secure.ToBool();
                    }

                    var cookie = new PBCookie(
                        name: Name,
                        url: null,
                        value: Value,
                        domain: Domain,
                        path: Path,
                        expires: Expires,
                        httpOnly: httpOnlyValue,
                        secure: secureValue,
                        sameSite: SameSite
                    );
                    PageService.SetCookiesAsync(page, new[] { cookie }).GetAwaiter().GetResult();
                }
                else
                {
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException("Either provide Name and Value parameters, or pipe Cookie objects."),
                        "InvalidParameters",
                        ErrorCategory.InvalidArgument,
                        null));
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetPageCookieError", ErrorCategory.WriteError, null));
            }
        }
    }
}