using System;
using System.IO;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPageScreenshot")]
    [OutputType(typeof(byte[]))]
    public class GetPageScreenshotCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to capture")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Path to save the screenshot file (optional)")]
        public string FilePath { get; set; }

        [Parameter(HelpMessage = "Take screenshot of full page (default: false - visible area only)")]
        public SwitchParameter FullPage { get; set; }

        [Parameter(HelpMessage = "Return screenshot data as byte array")]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page); 
                // Validate file path if provided
                if (!string.IsNullOrEmpty(FilePath))
                {
                    var directory = Path.GetDirectoryName(Path.GetFullPath(FilePath));
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                var screenshotData = pageService.GetPageScreenshotAsync(FilePath, FullPage.IsPresent).GetAwaiter().GetResult();

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
                WriteError(new ErrorRecord(ex, "GetPageScreenshotError", ErrorCategory.ReadError, null));
            }
        }
    }
}