using System.Management.Automation;
using System.Net;

namespace Pup
{
    /// <summary>
    /// Module initializer that runs when the Pup module is imported.
    /// This ensures TLS 1.2 is enabled for .NET Framework (PowerShell 5.1).
    /// </summary>
    public class ModuleInitializer : IModuleAssemblyInitializer
    {
        public void OnImport()
        {
            // Enable TLS 1.2 for .NET Framework (PowerShell 5.1)
            // This is required because .NET Framework 4.6.x defaults to TLS 1.0/1.1
            // which is rejected by most modern endpoints including Chrome DevTools Protocol
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }
    }
}
