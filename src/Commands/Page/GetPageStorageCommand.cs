using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPageStorage")]
    [OutputType(typeof(PSObject))]
    public class GetPageStorageCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get storage from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Storage type: Local or Session (default: Local)")]
        [ValidateSet("Local", "Session", IgnoreCase = true)]
        public string Type { get; set; } = "Local";

        [Parameter(HelpMessage = "Specific key to retrieve")]
        public string Key { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                var storage = pageService.GetStorageAsync(Type).GetAwaiter().GetResult();

                if (!string.IsNullOrEmpty(Key))
                {
                    if (storage.TryGetValue(Key, out var value))
                    {
                        WriteObject(new PSObject(new Dictionary<string, object> {
                            { "Key", Key },
                            { "Value", value },
                            { "Type", Type }
                        }));
                    }
                    return;
                }

                foreach (var kvp in storage)
                {
                    WriteObject(new PSObject(new Dictionary<string, object> {
                        { "Key", kvp.Key },
                        { "Value", kvp.Value },
                        { "Type", Type }
                    }));
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageStorageError", ErrorCategory.ReadError, null));
            }
        }
    }
}
