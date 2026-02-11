using System;
using System.IO;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Get, "PupElementScreenshot")]
    [OutputType(typeof(byte[]))]
    public class GetElementScreenshotCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Element to screenshot")]
        public PupElement Element { get; set; }

        [Parameter(HelpMessage = "Path to save the screenshot file")]
        public string FilePath { get; set; }

        [Parameter(HelpMessage = "Return screenshot data as byte array")]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                if (!string.IsNullOrEmpty(FilePath))
                {
                    var directory = Path.GetDirectoryName(Path.GetFullPath(FilePath));
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                var screenshotData = elementService.GetScreenshotAsync(FilePath).GetAwaiter().GetResult();

                if (PassThru.IsPresent)
                {
                    WriteObject(screenshotData);
                }

                if (!string.IsNullOrEmpty(FilePath))
                {
                    WriteVerbose($"Screenshot saved to: {Path.GetFullPath(FilePath)}");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementScreenshotError", ErrorCategory.ReadError, Element));
            }
        }
    }
}
