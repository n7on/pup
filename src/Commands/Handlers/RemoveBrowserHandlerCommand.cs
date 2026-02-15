using System;
using System.Management.Automation;
using PuppeteerSharp;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Handlers
{
    [Cmdlet(VerbsCommon.Remove, "PupBrowserHandler")]
    [OutputType(typeof(void))]
    public class RemoveBrowserHandlerCommand : BrowserBaseCommand
    {
        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The browser event to remove the handler for")]
        public PupBrowserEvent Event { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Browser = ResolveBrowserOrThrow();
                if (!Browser.Running)
                {
                    throw new InvalidOperationException("Browser is not running.");
                }

                RemoveHandler();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveBrowserHandlerError", ErrorCategory.InvalidOperation, Browser));
            }
        }

        private void RemoveHandler()
        {
            lock (Browser.HandlersLock)
            {
                if (!Browser.EventHandlers.TryGetValue(Event, out var existing))
                {
                    WriteVerbose($"No handler registered for {Event}");
                    return;
                }

                switch (Event)
                {
                    case PupBrowserEvent.PopupCreated:
                    case PupBrowserEvent.PageCreated:
                        if (existing.NativeHandler is EventHandler<TargetChangedArgs> tcHandler)
                            Browser.Browser.TargetCreated -= tcHandler;
                        break;
                    case PupBrowserEvent.PageClosed:
                        if (existing.NativeHandler is EventHandler<TargetChangedArgs> tdHandler)
                            Browser.Browser.TargetDestroyed -= tdHandler;
                        break;
                    case PupBrowserEvent.Disconnected:
                        if (existing.NativeHandler is EventHandler dHandler)
                            Browser.Browser.Disconnected -= dHandler;
                        break;
                }

                Browser.EventHandlers.Remove(Event);
            }

            WriteVerbose($"Browser handler removed for {Event}");
        }
    }
}
