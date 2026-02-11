using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Clear, "PupPageStorage")]
    [OutputType(typeof(void))]
    public class ClearPageStorageCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to clear storage from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Storage type: Local or Session (default: Local)")]
        [ValidateSet("Local", "Session", IgnoreCase = true)]
        public string Type { get; set; } = "Local";

        [Parameter(HelpMessage = "Key to remove (omit to clear all)")]
        public string Key { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                pageService.ClearStorageAsync(Type, Key).GetAwaiter().GetResult();
                WriteVerbose(string.IsNullOrEmpty(Key)
                    ? $"Cleared all entries from {Type} storage"
                    : $"Removed '{Key}' from {Type} storage");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ClearPageStorageError", ErrorCategory.WriteError, null));
            }
        }
    }
}
