using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Get, "PupPageConsole")]
    [OutputType(typeof(PupConsoleEntry[]))]
    public class GetPageConsoleCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to get console messages from")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Return captured entries to the pipeline (default: true)")]
        public SwitchParameter PassThru { get; set; } = true;

        protected override void ProcessRecord()
        {
            try
            {
                var entries = new List<PupConsoleEntry>();
                lock (Page.ConsoleLock)
                {
                    foreach (var entry in Page.ConsoleEntries)
                    {
                        entries.Add(new PupConsoleEntry
                        {
                            Type = entry.Type,
                            Text = entry.Text,
                            Url = entry.Url,
                            LineNumber = entry.LineNumber,
                            ColumnNumber = entry.ColumnNumber,
                            Timestamp = entry.Timestamp
                        });
                    }
                }

                if (PassThru.IsPresent)
                {
                    WriteObject(entries.ToArray(), true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageConsoleError", ErrorCategory.ReadError, Page));
            }
        }
    }
}
