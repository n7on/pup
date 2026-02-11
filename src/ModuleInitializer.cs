using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Reflection;

namespace Pup
{
    /// <summary>
    /// Module initializer that runs when the Pup module is imported.
    /// Handles TLS 1.2 for .NET Framework and assembly resolution for Windows PowerShell 5.1.
    /// </summary>
    public class ModuleInitializer : IModuleAssemblyInitializer
    {
        private static readonly string _assemblyDir =
            Path.GetDirectoryName(typeof(ModuleInitializer).Assembly.Location);

        public void OnImport()
        {
            // Enable TLS 1.2 for .NET Framework (PowerShell 5.1)
            // This is required because .NET Framework 4.6.x defaults to TLS 1.0/1.1
            // which is rejected by most modern endpoints including Chrome DevTools Protocol
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // Windows PowerShell 5.1 (.NET Framework) does not resolve dependency assemblies
            // from the module directory automatically. Register a handler so PuppeteerSharp's
            // dependencies (System.Text.Json, Microsoft.Bcl.AsyncInterfaces, etc.) are found.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        private static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            var candidatePath = Path.Combine(_assemblyDir, assemblyName.Name + ".dll");

            if (File.Exists(candidatePath))
            {
                return Assembly.LoadFrom(candidatePath);
            }

            return null;
        }
    }
}
