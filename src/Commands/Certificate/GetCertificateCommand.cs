using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Pup.Services;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Certificate
{
    [Cmdlet(VerbsCommon.Get, "PupCertificate")]
    [OutputType(typeof(PupSecurityDetails), typeof(PupCertificate))]
    public class GetCertificateCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get certificates from")]
        public PupPage Page { get; set; }

        [Parameter(
            HelpMessage = "Fetch certificate for specific origin (active mode). Example: https://github.com")]
        public string Origin { get; set; }

        [Parameter(
            HelpMessage = "Filter captured security details by URL pattern")]
        public string UrlFilter { get; set; }

        [Parameter(
            HelpMessage = "Only show expired certificates")]
        public SwitchParameter Expired { get; set; }

        [Parameter(
            HelpMessage = "Show certificates expiring within N days")]
        public int? ExpiringWithin { get; set; }

        [Parameter(
            HelpMessage = "Deduplicate results by SubjectName")]
        public SwitchParameter Unique { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var certificateService = ServiceFactory.CreateCertificateService(Page);

                if (!string.IsNullOrEmpty(Origin))
                {
                    // Active mode: fetch certificate for specific origin
                    var cert = certificateService.GetCertificateAsync(Origin).GetAwaiter().GetResult();
                    WriteObject(cert);
                }
                else
                {
                    // Passive mode: get captured security details
                    var details = certificateService.GetCapturedSecurityDetails();

                    // Apply filters
                    details = ApplyFilters(details);

                    WriteObject(details.ToArray(), true);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                WriteError(new ErrorRecord(
                    new Exception($"Failed to get certificate: {errorMessage}", ex),
                    "GetCertificateError",
                    ErrorCategory.ReadError,
                    Page));
            }
        }

        private List<PupSecurityDetails> ApplyFilters(List<PupSecurityDetails> details)
        {
            var results = details;

            // Filter by URL pattern
            if (!string.IsNullOrEmpty(UrlFilter))
            {
                results = results.Where(d =>
                    d.Url != null && d.Url.IndexOf(UrlFilter, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }

            // Filter by expired
            if (Expired.IsPresent)
            {
                results = results.Where(d => d.IsExpired).ToList();
            }

            // Filter by expiring within N days
            if (ExpiringWithin.HasValue)
            {
                results = results.Where(d =>
                    d.DaysUntilExpiry.HasValue && d.DaysUntilExpiry.Value <= ExpiringWithin.Value
                ).ToList();
            }

            // Deduplicate by SubjectName
            if (Unique.IsPresent)
            {
                results = results
                    .Where(d => !string.IsNullOrEmpty(d.SubjectName))
                    .GroupBy(d => d.SubjectName)
                    .Select(g => g.First())
                    .ToList();
            }

            return results;
        }
    }
}
