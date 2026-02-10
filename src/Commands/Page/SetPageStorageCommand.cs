using System;
using System.Collections.Generic;
using System.Collections;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Set, "PupPageStorage")]
    [OutputType(typeof(void))]
    public class SetPageStorageCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to set storage on")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Storage type: Local or Session (default: Local)")]
        [ValidateSet("Local", "Session", IgnoreCase = true)]
        public string Type { get; set; } = "Local";

        [Parameter(HelpMessage = "Key to set")]
        public string Key { get; set; }

        [Parameter(HelpMessage = "Value to set")]
        public string Value { get; set; }

        [Parameter(HelpMessage = "Hashtable of key/value pairs to set")]
        public Hashtable Items { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                if (Items != null)
                {
                    foreach (DictionaryEntry entry in Items)
                    {
                        dict[entry.Key.ToString()] = entry.Value?.ToString() ?? string.Empty;
                    }
                }

                if (!string.IsNullOrEmpty(Key))
                {
                    dict[Key] = Value ?? string.Empty;
                }

                if (dict.Count == 0)
                {
                    ThrowTerminatingError(new ErrorRecord(
                        new ArgumentException("Provide -Items or -Key/-Value to set storage entries."),
                        "MissingStorageInput",
                        ErrorCategory.InvalidArgument,
                        null));
                }

                pageService.SetStorageAsync(Type, dict).GetAwaiter().GetResult();
                WriteVerbose($"Set {dict.Count} storage entrie(s) in {Type} storage");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetPageStorageError", ErrorCategory.WriteError, null));
            }
        }
    }
}
