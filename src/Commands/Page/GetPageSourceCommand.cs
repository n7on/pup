using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPageSource")]
    [OutputType(typeof(string))]
    public class GetPageSourceCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Save the HTML to a file path")]
        public string FilePath { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var html = pageService.ExecuteScriptAsync<string>("() => document.documentElement.outerHTML").GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(FilePath))
                {
                    var fullPath = System.IO.Path.GetFullPath(FilePath);
                    var dir = System.IO.Path.GetDirectoryName(fullPath);
                    if (!System.IO.Directory.Exists(dir))
                    {
                        System.IO.Directory.CreateDirectory(dir);
                    }
                    System.IO.File.WriteAllText(fullPath, html);
                    WriteVerbose($"Page source saved to {fullPath}");
                }

                WriteObject(html);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageSourceError", ErrorCategory.ReadError, Page));
            }
        }
    }
}
