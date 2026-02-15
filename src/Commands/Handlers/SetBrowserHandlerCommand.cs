using System;
using System.Management.Automation;
using PuppeteerSharp;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Handlers
{
    [Cmdlet(VerbsCommon.Set, "PupBrowserHandler")]
    [OutputType(typeof(void))]
    public class SetBrowserHandlerCommand : BrowserBaseCommand
    {
        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "ScriptBlock",
            HelpMessage = "The browser event to handle")]
        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "Action",
            HelpMessage = "The browser event to handle")]
        public PupBrowserEvent Event { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            ParameterSetName = "ScriptBlock",
            HelpMessage = "Script block to execute when the event occurs. Receives event data as parameter.")]
        public ScriptBlock ScriptBlock { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            ParameterSetName = "Action",
            HelpMessage = "Action to take when the event occurs (Dismiss for popups)")]
        public PupHandlerAction Action { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Browser = ResolveBrowserOrThrow();
                if (!Browser.Running)
                {
                    throw new InvalidOperationException("Browser is not running.");
                }

                SetHandler();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetBrowserHandlerError", ErrorCategory.InvalidOperation, Browser));
            }
        }

        private void SetHandler()
        {
            // Remove existing handler if any
            RemoveExistingHandler();

            // Use GetNewClosure() to capture the current session state variables
            var scriptBlockWithClosure = ScriptBlock?.GetNewClosure();
            var handler = new BrowserEventHandler
            {
                Event = Event,
                ScriptBlock = scriptBlockWithClosure,
                SessionState = SessionState,
                Action = ParameterSetName == "Action" ? Action : (PupHandlerAction?)null
            };

            switch (Event)
            {
                case PupBrowserEvent.PopupCreated:
                    SetupPopupCreatedHandler(handler);
                    break;
                case PupBrowserEvent.PageCreated:
                    SetupPageCreatedHandler(handler);
                    break;
                case PupBrowserEvent.PageClosed:
                    SetupPageClosedHandler(handler);
                    break;
                case PupBrowserEvent.Disconnected:
                    SetupDisconnectedHandler(handler);
                    break;
            }

            lock (Browser.HandlersLock)
            {
                Browser.EventHandlers[Event] = handler;
            }

            WriteVerbose($"Browser handler set for {Event}");
        }

        private void SetupPopupCreatedHandler(BrowserEventHandler handler)
        {
            EventHandler<TargetChangedArgs> nativeHandler = async (sender, e) =>
            {
                if (e.Target.Type != TargetType.Page || e.Target.Opener == null)
                    return;

                try
                {
                    var page = await e.Target.PageAsync().ConfigureAwait(false);
                    if (page == null) return;

                    if (handler.Action == PupHandlerAction.Dismiss)
                    {
                        await page.CloseAsync().ConfigureAwait(false);
                        return;
                    }

                    if (handler.ScriptBlock != null)
                    {
                        var browserService = ServiceFactory.CreateBrowserService(Browser);
                        var pupPage = await browserService.InitializePopupPageAsync(page).ConfigureAwait(false);
                        var eventData = new PupPopupCreatedEvent { Page = pupPage };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Browser.Browser.TargetCreated += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupPageCreatedHandler(BrowserEventHandler handler)
        {
            EventHandler<TargetChangedArgs> nativeHandler = async (sender, e) =>
            {
                if (e.Target.Type != TargetType.Page)
                    return;

                try
                {
                    var page = await e.Target.PageAsync().ConfigureAwait(false);
                    if (page == null) return;

                    if (handler.ScriptBlock != null)
                    {
                        var browserService = ServiceFactory.CreateBrowserService(Browser);
                        var pupPage = await browserService.InitializePopupPageAsync(page).ConfigureAwait(false);
                        var eventData = new PupPageCreatedEvent { Page = pupPage };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Browser.Browser.TargetCreated += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupPageClosedHandler(BrowserEventHandler handler)
        {
            EventHandler<TargetChangedArgs> nativeHandler = (sender, e) =>
            {
                if (e.Target.Type != TargetType.Page)
                    return;

                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupPageClosedEvent
                        {
                            Url = e.Target.Url,
                            TargetId = e.Target.TargetId
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Browser.Browser.TargetDestroyed += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupDisconnectedHandler(BrowserEventHandler handler)
        {
            EventHandler nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupDisconnectedEvent();
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Browser.Browser.Disconnected += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void RemoveExistingHandler()
        {
            lock (Browser.HandlersLock)
            {
                if (!Browser.EventHandlers.TryGetValue(Event, out var existing))
                    return;

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
        }
    }
}
