using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Remove, "PupPageCookie")]
    [OutputType(typeof(void))]
    public class RemovePageCookieCommand : PSCmdlet
    {
        private const string ByFilter = "ByFilter";
        private const string ByObject = "ByObject";

        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = ByFilter)]
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = ByObject)]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            ParameterSetName = ByFilter,
            HelpMessage = "Cookie name to remove (supports wildcards)")]
        public string Name { get; set; }

        [Parameter(
            ParameterSetName = ByFilter,
            HelpMessage = "Domain filter (supports wildcards; defaults to current page host when deleting by name)")]
        public string Domain { get; set; }

        [Parameter(
            ParameterSetName = ByFilter,
            HelpMessage = "Path filter")]
        public string Path { get; set; }

        [Parameter(
            ParameterSetName = ByFilter,
            HelpMessage = "Url for the cookie (used when domain is not provided)")]
        public string Url { get; set; }

        [Parameter(
            ParameterSetName = ByFilter,
            HelpMessage = "Remove all cookies")]
        public SwitchParameter All { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = ByObject,
            HelpMessage = "Cookie object(s) to remove")]
        public PupCookie[] Cookies { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var cookiesToDelete = new List<PupCookie>();

                if (ParameterSetName == ByObject)
                {
                    if (Cookies != null && Cookies.Length > 0)
                    {
                        cookiesToDelete.AddRange(Cookies.Where(c => c != null));
                    }
                }
                else
                {
                    var allCookies = pageService.GetCookiesAsync().GetAwaiter().GetResult();

                    if (!All.IsPresent && string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Domain))
                    {
                        ThrowTerminatingError(new ErrorRecord(
                            new ArgumentException("Specify -Name, -Domain, or -All to delete cookies."),
                            "MissingCookieName",
                            ErrorCategory.InvalidArgument,
                            null));
                    }

                    var filtered = allCookies;

                    if (!string.IsNullOrEmpty(Name))
                    {
                        var namePattern = new WildcardPattern(Name, WildcardOptions.IgnoreCase);
                        filtered = filtered.Where(c => namePattern.IsMatch(c.Name ?? string.Empty)).ToList();
                    }

                    if (!string.IsNullOrEmpty(Domain))
                    {
                        var domainPattern = new WildcardPattern(Domain, WildcardOptions.IgnoreCase);
                        filtered = filtered.Where(c => domainPattern.IsMatch(c.Domain ?? string.Empty)).ToList();
                    }
                    else if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(Name))
                    {
                        // If deleting by name and no domain, attempt to scope to the provided URL host
                        try
                        {
                            var host = new Uri(Url).Host;
                            filtered = filtered.Where(c => string.Equals(c.Domain, host, StringComparison.OrdinalIgnoreCase)).ToList();
                        }
                        catch
                        {
                            // ignore parse issues, fallback to name-only filter
                        }
                    }
                    else if (!string.IsNullOrEmpty(Page?.Url) && !string.IsNullOrEmpty(Name))
                    {
                        try
                        {
                            var host = new Uri(Page.Url).Host;
                            filtered = filtered.Where(c => string.Equals(c.Domain, host, StringComparison.OrdinalIgnoreCase)).ToList();
                        }
                        catch
                        {
                            // ignore parse issues
                        }
                    }

                    if (!string.IsNullOrEmpty(Path))
                    {
                        var pathPattern = new WildcardPattern(Path, WildcardOptions.IgnoreCase);
                        filtered = filtered.Where(c => pathPattern.IsMatch(c.Path ?? string.Empty)).ToList();
                    }

                    cookiesToDelete.AddRange(filtered);
                }

                if (cookiesToDelete.Count == 0)
                {
                    WriteVerbose("No cookies matched the provided filters to remove.");
                    return;
                }

                pageService.DeleteCookiesAsync(cookiesToDelete.ToArray()).GetAwaiter().GetResult();
                WriteVerbose($"Removed {cookiesToDelete.Count} cookie(s).");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemovePageCookieError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
