using System;
using System.IO;
using System.Reflection;

namespace Pup.Services
{
    public static class EmbeddedResourceService
    {
        public static string LoadScript(string scriptName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Pup.Scripts.{scriptName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
