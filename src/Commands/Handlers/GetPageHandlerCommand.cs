using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Handlers
{
    [Cmdlet(VerbsCommon.Get, "PupPageHandler")]
    [OutputType(typeof(PSObject))]
    public class GetPageHandlerCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to get handlers for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Filter by specific event type")]
        public PupPageEvent? Event { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Page == null || !Page.Running)
                {
                    throw new InvalidOperationException("Page is not valid or not running.");
                }

                lock (Page.HandlersLock)
                {
                    foreach (var kvp in Page.EventHandlers)
                    {
                        if (Event.HasValue && kvp.Key != Event.Value)
                            continue;

                        var handler = kvp.Value;
                        var result = new PSObject();
                        result.Properties.Add(new PSNoteProperty("Event", kvp.Key));
                        result.Properties.Add(new PSNoteProperty("Action", handler.Action));
                        result.Properties.Add(new PSNoteProperty("HasScriptBlock", handler.ScriptBlock != null));
                        WriteObject(result);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPageHandlerError", ErrorCategory.InvalidOperation, Page));
            }
        }
    }
}
