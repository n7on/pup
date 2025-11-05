using System;
using System.IO;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PageScreenshot")]
    [OutputType(typeof(byte[]))]
    public class GetPageScreenshotCommand : PageBaseCommand
    {
        [Parameter(HelpMessage = "Path to save the screenshot file (optional)")]
        public string FilePath { get; set; }

        [Parameter(HelpMessage = "Take screenshot of full page (default: false - visible area only)")]
        public SwitchParameter FullPage { get; set; }

        [Parameter(HelpMessage = "Return screenshot data as byte array (default: true)")]
        public SwitchParameter PassThru { get; set; } = true;

        protected override void ProcessRecord()
        {
            try
            {
                var page = ResolvePageOrThrow();
                
                // Validate file path if provided
                if (!string.IsNullOrEmpty(FilePath))
                {
                    var directory = Path.GetDirectoryName(Path.GetFullPath(FilePath));
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                var screenshotData = PageService.GetPageScreenshotAsync(page, FilePath, FullPage.IsPresent).GetAwaiter().GetResult();

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