using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Pup.Transport;

namespace Pup.Services
{
    public interface ICertificateService
    {
        List<PupSecurityDetails> GetCapturedSecurityDetails();
        Task<PupCertificate> GetCertificateAsync(string origin);
    }

    public class CertificateService : ICertificateService
    {
        private readonly PupPage _page;
        private readonly ICdpService _cdpService;

        public CertificateService(PupPage page, ICdpService cdpService)
        {
            _page = page;
            _cdpService = cdpService;
        }

        public List<PupSecurityDetails> GetCapturedSecurityDetails()
        {
            var results = new List<PupSecurityDetails>();

            lock (_page.NetworkLock)
            {
                foreach (var entry in _page.NetworkEntries)
                {
                    if (entry.SecurityDetails != null)
                    {
                        var details = entry.SecurityDetails.Clone();
                        details.Url = entry.Url;
                        details.RemoteAddress = entry.RemoteAddress;
                        results.Add(details);
                    }
                }
            }

            return results;
        }

        public async Task<PupCertificate> GetCertificateAsync(string origin)
        {
            var response = await _cdpService.SendAsync<JsonElement>(
                "Network.getCertificate",
                new Dictionary<string, object> { { "origin", origin } }
            ).ConfigureAwait(false);

            var certificate = new PupCertificate
            {
                Origin = origin
            };

            if (response.TryGetProperty("tableNames", out var tableNames) &&
                tableNames.ValueKind == JsonValueKind.Array)
            {
                foreach (var certData in tableNames.EnumerateArray())
                {
                    if (certData.ValueKind == JsonValueKind.String)
                    {
                        certificate.CertificateChain.Add(certData.GetString());
                    }
                }
            }

            // Parse the leaf certificate to extract details
            certificate.ParseLeafCertificate();

            return certificate;
        }
    }
}
