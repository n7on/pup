using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;
using Pup.Services;

namespace Pup.Commands.Text
{
    [Cmdlet(VerbsCommon.Select, "PupText")]
    [OutputType(typeof(string))]
    public class SelectTextCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "FromPage",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to extract text from")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "FromFrame",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The frame to extract text from")]
        public PupFrame Frame { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The regex pattern to match against the page text")]
        public string Pattern { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string result;
                var escapedPattern = Pattern.Replace("\\", "\\\\").Replace("'", "\\'");
                var script = $"() => {{ const m = document.body.innerText.match(new RegExp('{escapedPattern}')); return m ? (m[1] ?? m[0]) : null; }}";

                if (ParameterSetName == "FromFrame")
                {
                    var frameService = ServiceFactory.CreateFrameService(Frame);
                    result = frameService.ExecuteScriptAsync<string>(script).GetAwaiter().GetResult();
                }
                else
                {
                    var pageService = ServiceFactory.CreatePageService(Page);
                    result = pageService.ExecuteScriptAsync<string>(script).GetAwaiter().GetResult();
                }

                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SelectTextError", ErrorCategory.ReadError, Page));
            }
        }
    }
}
