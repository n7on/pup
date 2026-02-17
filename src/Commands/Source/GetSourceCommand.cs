using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;
using Pup.Services;

namespace Pup.Commands.Source
{
    [Cmdlet(VerbsCommon.Get, "PupSource")]
    [OutputType(typeof(string))]
    public class GetSourceCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "FromPage",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get HTML source from")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "FromFrame",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The frame to get HTML source from")]
        public PupFrame Frame { get; set; }

        [Parameter(HelpMessage = "Save the HTML to a file path")]
        public string FilePath { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string html;
                if (ParameterSetName == "FromFrame")
                {
                    var frameService = ServiceFactory.CreateFrameService(Frame);
                    html = frameService.GetSourceAsync().GetAwaiter().GetResult();
                }
                else
                {
                    var pageService = ServiceFactory.CreatePageService(Page);
                    html = pageService.ExecuteScriptAsync<string>("() => document.documentElement.outerHTML").GetAwaiter().GetResult();
                }

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
                WriteError(new ErrorRecord(ex, "GetSourceError", ErrorCategory.ReadError, Page));
            }
        }
    }
}
