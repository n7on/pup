using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Input
{
    [Cmdlet(VerbsCommunications.Send, "PupFile")]
    [OutputType(typeof(void))]
    public class SendFileCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The file input element to upload files to")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "Path(s) to the file(s) to upload")]
        public string[] FilePath { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                // Resolve and validate file paths
                var resolvedPaths = FilePath.Select(p =>
                {
                    var resolved = GetUnresolvedProviderPathFromPSPath(p);
                    if (!File.Exists(resolved))
                    {
                        throw new FileNotFoundException($"File not found: {resolved}");
                    }
                    return resolved;
                }).ToArray();

                var elementService = ServiceFactory.CreateElementService(Element);
                elementService.UploadFilesAsync(resolvedPaths).GetAwaiter().GetResult();

                WriteVerbose($"Uploaded {resolvedPaths.Length} file(s): {string.Join(", ", resolvedPaths.Select(Path.GetFileName))}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SendFileError", ErrorCategory.InvalidOperation, Element));
            }
        }
    }
}
