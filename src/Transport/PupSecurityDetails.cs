using System;
using System.Collections.Generic;

namespace Pup.Transport
{
    /// <summary>
    /// TLS connection details captured from Network.responseReceived events
    /// </summary>
    public class PupSecurityDetails
    {
        public string Protocol { get; set; }
        public string KeyExchange { get; set; }
        public string KeyExchangeGroup { get; set; }
        public string Cipher { get; set; }
        public string Mac { get; set; }
        public string SubjectName { get; set; }
        public List<string> SanList { get; set; } = new List<string>();
        public string Issuer { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string CertificateTransparencyCompliance { get; set; }
        public List<PupSignedCertificateTimestamp> SignedCertificateTimestampList { get; set; } = new List<PupSignedCertificateTimestamp>();

        // Context from the network entry
        public string Url { get; set; }
        public string RemoteAddress { get; set; }

        public bool IsExpired => ValidTo.HasValue && ValidTo.Value < DateTime.UtcNow;

        public int? DaysUntilExpiry => ValidTo.HasValue
            ? (int?)Math.Ceiling((ValidTo.Value - DateTime.UtcNow).TotalDays)
            : null;

        public override string ToString()
        {
            return $"{SubjectName} ({Protocol}, expires {ValidTo:yyyy-MM-dd})";
        }

        public PupSecurityDetails Clone()
        {
            return new PupSecurityDetails
            {
                Protocol = Protocol,
                KeyExchange = KeyExchange,
                KeyExchangeGroup = KeyExchangeGroup,
                Cipher = Cipher,
                Mac = Mac,
                SubjectName = SubjectName,
                SanList = new List<string>(SanList ?? new List<string>()),
                Issuer = Issuer,
                ValidFrom = ValidFrom,
                ValidTo = ValidTo,
                CertificateTransparencyCompliance = CertificateTransparencyCompliance,
                SignedCertificateTimestampList = new List<PupSignedCertificateTimestamp>(
                    SignedCertificateTimestampList ?? new List<PupSignedCertificateTimestamp>()),
                Url = Url,
                RemoteAddress = RemoteAddress
            };
        }
    }

    /// <summary>
    /// Signed Certificate Timestamp from Certificate Transparency logs
    /// </summary>
    public class PupSignedCertificateTimestamp
    {
        public string Status { get; set; }
        public string Origin { get; set; }
        public string LogDescription { get; set; }
        public string LogId { get; set; }
        public DateTime? Timestamp { get; set; }
        public string HashAlgorithm { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string SignatureData { get; set; }
    }
}
