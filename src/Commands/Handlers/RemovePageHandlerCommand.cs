using System;
using System.Management.Automation;
using PuppeteerSharp;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Handlers
{
    [Cmdlet(VerbsCommon.Remove, "PupPageHandler")]
    [OutputType(typeof(void))]
    public class RemovePageHandlerCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to remove the handler from")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The page event to remove the handler for")]
        public PupPageEvent Event { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Page == null || !Page.Running)
                {
                    throw new InvalidOperationException("Page is not valid or not running.");
                }

                RemoveHandler();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemovePageHandlerError", ErrorCategory.InvalidOperation, Page));
            }
        }

        private void RemoveHandler()
        {
            lock (Page.HandlersLock)
            {
                if (!Page.EventHandlers.TryGetValue(Event, out var existing))
                {
                    WriteVerbose($"No handler registered for {Event}");
                    return;
                }

                switch (Event)
                {
                    case PupPageEvent.Dialog:
                        if (existing.NativeHandler is EventHandler<DialogEventArgs> dialogHandler)
                            Page.Page.Dialog -= dialogHandler;
                        break;
                    case PupPageEvent.Console:
                        if (existing.NativeHandler is EventHandler<ConsoleEventArgs> consoleHandler)
                            Page.Page.Console -= consoleHandler;
                        break;
                    case PupPageEvent.PageError:
                        if (existing.NativeHandler is EventHandler<PageErrorEventArgs> errorHandler)
                            Page.Page.PageError -= errorHandler;
                        break;
                    case PupPageEvent.Load:
                        if (existing.NativeHandler is EventHandler loadHandler)
                            Page.Page.Load -= loadHandler;
                        break;
                    case PupPageEvent.DOMContentLoaded:
                        if (existing.NativeHandler is EventHandler domHandler)
                            Page.Page.DOMContentLoaded -= domHandler;
                        break;
                    case PupPageEvent.Request:
                        if (existing.NativeHandler is EventHandler<RequestEventArgs> reqHandler)
                            Page.Page.Request -= reqHandler;
                        break;
                    case PupPageEvent.RequestFinished:
                        if (existing.NativeHandler is EventHandler<RequestEventArgs> reqFinHandler)
                            Page.Page.RequestFinished -= reqFinHandler;
                        break;
                    case PupPageEvent.RequestFailed:
                        if (existing.NativeHandler is EventHandler<RequestEventArgs> reqFailHandler)
                            Page.Page.RequestFailed -= reqFailHandler;
                        break;
                    case PupPageEvent.Response:
                        if (existing.NativeHandler is EventHandler<ResponseCreatedEventArgs> respHandler)
                            Page.Page.Response -= respHandler;
                        break;
                    case PupPageEvent.FrameAttached:
                        if (existing.NativeHandler is EventHandler<FrameEventArgs> faHandler)
                            Page.Page.FrameAttached -= faHandler;
                        break;
                    case PupPageEvent.FrameDetached:
                        if (existing.NativeHandler is EventHandler<FrameEventArgs> fdHandler)
                            Page.Page.FrameDetached -= fdHandler;
                        break;
                    case PupPageEvent.FrameNavigated:
                        if (existing.NativeHandler is EventHandler<FrameNavigatedEventArgs> fnHandler)
                            Page.Page.FrameNavigated -= fnHandler;
                        break;
                    case PupPageEvent.FileChooser:
                        // FileChooser uses WaitForFileChooserAsync, not events - nothing to remove
                        break;
                    case PupPageEvent.WorkerCreated:
                        if (existing.NativeHandler is EventHandler<WorkerEventArgs> wcHandler)
                            Page.Page.WorkerCreated -= wcHandler;
                        break;
                    case PupPageEvent.WorkerDestroyed:
                        if (existing.NativeHandler is EventHandler<WorkerEventArgs> wdHandler)
                            Page.Page.WorkerDestroyed -= wdHandler;
                        break;
                    case PupPageEvent.Close:
                        if (existing.NativeHandler is EventHandler closeHandler)
                            Page.Page.Close -= closeHandler;
                        break;
                }

                Page.EventHandlers.Remove(Event);
            }

            WriteVerbose($"Page handler removed for {Event}");
        }
    }
}
