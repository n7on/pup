using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Frame
{
    [Cmdlet(VerbsCommon.Get, "PupFrame")]
    [OutputType(typeof(PupFrame))]
    public class GetFrameCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Page to get frames from")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Filter frames by name")]
        public string Name { get; set; }

        [Parameter(HelpMessage = "Filter frames by URL (supports wildcards)")]
        public string Url { get; set; }

        [Parameter(HelpMessage = "Include the main frame in results")]
        public SwitchParameter IncludeMain { get; set; }

        [Parameter(HelpMessage = "Return only the first matching frame")]
        public SwitchParameter First { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var frames = new List<PupFrame>();
                var allFrames = Page.Page.Frames;

                foreach (var frame in allFrames)
                {
                    var pupFrame = PupFrame.CreateAsync(frame).GetAwaiter().GetResult();

                    // Skip main frame unless explicitly requested
                    if (pupFrame.IsMainFrame && !IncludeMain.IsPresent)
                        continue;

                    if (!string.IsNullOrEmpty(Name) && !WildcardMatch(pupFrame.Name, Name))
                        continue;

                    if (!string.IsNullOrEmpty(Url) && !WildcardMatch(pupFrame.Url, Url))
                        continue;

                    frames.Add(pupFrame);

                    if (First.IsPresent)
                        break;
                }

                foreach (var frame in frames)
                {
                    WriteObject(frame);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetFrameFailed", ErrorCategory.OperationStopped, null));
            }
        }

        private bool WildcardMatch(string value, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            var wildcardPattern = new WildcardPattern(pattern, WildcardOptions.IgnoreCase);
            return wildcardPattern.IsMatch(value ?? string.Empty);
        }
    }
}
