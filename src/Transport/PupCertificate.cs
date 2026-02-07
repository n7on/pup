using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Pup.Transport
{
    /// <summary>
    /// Certificate retrieved via Network.getCertificate CDP method
    /// </summary>
    public class PupCertificate
    {
        public string Origin { get; set; }
        public List<string> CertificateChain { get; set; } = new List<string>();

        // Parsed from first certificate in chain (leaf certificate)
        public string SubjectName { get; set; }
        public string Issuer { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string SerialNumber { get; set; }
        public string Thumbprint { get; set; }
        public List<string> SubjectAlternativeNames { get; set; } = new List<string>();

        public bool IsValid => ValidFrom.HasValue && ValidTo.HasValue
            && ValidFrom.Value <= DateTime.UtcNow
            && ValidTo.Value >= DateTime.UtcNow;

        public int? DaysUntilExpiry => ValidTo.HasValue
            ? (int?)Math.Ceiling((ValidTo.Value - DateTime.UtcNow).TotalDays)
            : null;

        public override string ToString()
        {
            return $"{SubjectName} (expires {ValidTo:yyyy-MM-dd}, {DaysUntilExpiry} days)";
        }

        /// <summary>
        /// Parse the leaf certificate from the DER-encoded chain
        /// </summary>
        internal void ParseLeafCertificate()
        {
            if (CertificateChain == null || CertificateChain.Count == 0)
                return;

            try
            {
                var derBytes = Convert.FromBase64String(CertificateChain[0]);
                using (var cert = new X509Certificate2(derBytes))
                {
                    SubjectName = cert.Subject;
                    Issuer = cert.Issuer;
                    ValidFrom = cert.NotBefore.ToUniversalTime();
                    ValidTo = cert.NotAfter.ToUniversalTime();
                    SerialNumber = cert.SerialNumber;
                    Thumbprint = cert.Thumbprint;

                    // Parse Subject Alternative Names from extensions
                    foreach (var ext in cert.Extensions)
                    {
                        if (ext.Oid?.Value == "2.5.29.17") // Subject Alternative Name OID
                        {
                            var sanExtension = ext.Format(false);
                            if (!string.IsNullOrEmpty(sanExtension))
                            {
                                // Parse the formatted SAN string
                                foreach (var entry in sanExtension.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var trimmed = entry.Trim();
                                    if (trimmed.StartsWith("DNS Name=", StringComparison.OrdinalIgnoreCase))
                                    {
                                        SubjectAlternativeNames.Add(trimmed.Substring(9));
                                    }
                                    else if (trimmed.StartsWith("DNS:", StringComparison.OrdinalIgnoreCase))
                                    {
                                        SubjectAlternativeNames.Add(trimmed.Substring(4));
                                    }
                                    else if (!trimmed.Contains("=") && !trimmed.Contains(":"))
                                    {
                                        SubjectAlternativeNames.Add(trimmed);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // If parsing fails, leave fields as null
            }
        }
    }
}
