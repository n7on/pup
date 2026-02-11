using System;
using System.Management.Automation;
using Pup.Services;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Get, "PupElementPattern")]
    [OutputType(typeof(PupElementPattern[]))]
    public class GetElementPatternCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The element to generate patterns for")]
        public PupElement Element { get; set; }

        [Parameter(HelpMessage = "Only return patterns with at least this many matches")]
        public int MinMatches { get; set; } = 1;

        [Parameter(HelpMessage = "Only return patterns with at most this many matches")]
        public int MaxMatches { get; set; } = int.MaxValue;

        [Parameter(HelpMessage = "Filter by pattern type (ByTag, ByClass, ByParentClass, ByStructure, etc.)")]
        public string Type { get; set; }

        [Parameter(HelpMessage = "Go up N levels to ancestor before generating patterns (0=element itself, 1=parent, 2=grandparent)")]
        public int Depth { get; set; } = 0;

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = new ElementService(Element);
                var patterns = elementService.GetElementPatternsAsync(Depth).GetAwaiter().GetResult();

                foreach (var pattern in patterns)
                {
                    // Apply filters
                    if (pattern.MatchCount < MinMatches)
                        continue;
                    if (pattern.MatchCount > MaxMatches)
                        continue;
                    if (!string.IsNullOrEmpty(Type) &&
                        !pattern.Type.Equals(Type, StringComparison.OrdinalIgnoreCase))
                        continue;

                    WriteObject(pattern);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetElementPatternFailed", ErrorCategory.OperationStopped, Element));
            }
        }
    }
}
