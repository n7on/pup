using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Handlers
{
    [Cmdlet(VerbsCommon.Get, "PupBrowserHandler")]
    [OutputType(typeof(PSObject))]
    public class GetBrowserHandlerCommand : BrowserBaseCommand
    {
        [Parameter(
            Position = 1,
            HelpMessage = "Filter by specific event type")]
        public PupBrowserEvent? Event { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Browser = ResolveBrowserOrThrow();
                if (!Browser.Running)
                {
                    throw new InvalidOperationException("Browser is not running.");
                }

                lock (Browser.HandlersLock)
                {
                    foreach (var kvp in Browser.EventHandlers)
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
                WriteError(new ErrorRecord(ex, "GetBrowserHandlerError", ErrorCategory.InvalidOperation, Browser));
            }
        }
    }
}
