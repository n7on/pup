using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.Json;
using PuppeteerSharp;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Handlers
{
    [Cmdlet(VerbsCommon.Set, "PupPageHandler")]
    [OutputType(typeof(void))]
    public class SetPageHandlerCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to set the handler on")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "ScriptBlock",
            HelpMessage = "The page event to handle")]
        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "Action",
            HelpMessage = "The page event to handle")]
        public PupPageEvent Event { get; set; }

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
            HelpMessage = "Action to take when the event occurs (Accept/Dismiss for dialogs, Ignore to suppress)")]
        public PupHandlerAction Action { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Page == null || !Page.Running)
                {
                    throw new InvalidOperationException("Page is not valid or not running.");
                }

                SetHandler();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetPageHandlerError", ErrorCategory.InvalidOperation, Page));
            }
        }

        private void SetHandler()
        {
            // Remove existing handler if any
            RemoveExistingHandler();

            // Use GetNewClosure() to capture the current session state variables
            var scriptBlockWithClosure = ScriptBlock?.GetNewClosure();
            var handler = new PageEventHandler
            {
                Event = Event,
                ScriptBlock = scriptBlockWithClosure,
                SessionState = SessionState,
                Action = ParameterSetName == "Action" ? Action : (PupHandlerAction?)null
            };

            switch (Event)
            {
                case PupPageEvent.Dialog:
                    SetupDialogHandler(handler);
                    break;
                case PupPageEvent.Console:
                    SetupConsoleHandler(handler);
                    break;
                case PupPageEvent.PageError:
                    SetupPageErrorHandler(handler);
                    break;
                case PupPageEvent.Load:
                    SetupLoadHandler(handler);
                    break;
                case PupPageEvent.DOMContentLoaded:
                    SetupDOMContentLoadedHandler(handler);
                    break;
                case PupPageEvent.Request:
                    SetupRequestHandler(handler);
                    break;
                case PupPageEvent.RequestFinished:
                    SetupRequestFinishedHandler(handler);
                    break;
                case PupPageEvent.RequestFailed:
                    SetupRequestFailedHandler(handler);
                    break;
                case PupPageEvent.Response:
                    SetupResponseHandler(handler);
                    break;
                case PupPageEvent.FrameAttached:
                    SetupFrameAttachedHandler(handler);
                    break;
                case PupPageEvent.FrameDetached:
                    SetupFrameDetachedHandler(handler);
                    break;
                case PupPageEvent.FrameNavigated:
                    SetupFrameNavigatedHandler(handler);
                    break;
                case PupPageEvent.Download:
                    SetupDownloadHandler(handler);
                    break;
                case PupPageEvent.FileChooser:
                    SetupFileChooserHandler(handler);
                    break;
                case PupPageEvent.WorkerCreated:
                    SetupWorkerCreatedHandler(handler);
                    break;
                case PupPageEvent.WorkerDestroyed:
                    SetupWorkerDestroyedHandler(handler);
                    break;
                case PupPageEvent.Close:
                    SetupCloseHandler(handler);
                    break;
            }

            lock (Page.HandlersLock)
            {
                Page.EventHandlers[Event] = handler;
            }

            WriteVerbose($"Page handler set for {Event}");
        }

        private void SetupDialogHandler(PageEventHandler handler)
        {
            EventHandler<DialogEventArgs> nativeHandler = async (sender, e) =>
            {
                try
                {
                    if (handler.Action == PupHandlerAction.Accept)
                    {
                        await e.Dialog.Accept().ConfigureAwait(false);
                        return;
                    }
                    if (handler.Action == PupHandlerAction.Dismiss)
                    {
                        await e.Dialog.Dismiss().ConfigureAwait(false);
                        return;
                    }

                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupDialogEvent(e.Dialog);
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.Dialog += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupConsoleHandler(PageEventHandler handler)
        {
            EventHandler<ConsoleEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupConsoleEvent
                        {
                            Type = e.Message.Type.ToString(),
                            Text = e.Message.Text,
                            Url = e.Message.Location?.URL,
                            LineNumber = e.Message.Location?.LineNumber,
                            ColumnNumber = e.Message.Location?.ColumnNumber
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.Console += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupPageErrorHandler(PageEventHandler handler)
        {
            EventHandler<PageErrorEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupPageErrorEvent
                        {
                            Message = e.Message
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.PageError += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupLoadHandler(PageEventHandler handler)
        {
            EventHandler nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupLoadEvent { Url = Page.Page.Url };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.Load += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupDOMContentLoadedHandler(PageEventHandler handler)
        {
            EventHandler nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupDOMContentLoadedEvent { Url = Page.Page.Url };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.DOMContentLoaded += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupRequestHandler(PageEventHandler handler)
        {
            EventHandler<RequestEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupRequestEvent
                        {
                            RequestId = Guid.NewGuid().ToString("N"),
                            Url = e.Request.Url,
                            Method = e.Request.Method.ToString(),
                            ResourceType = e.Request.ResourceType.ToString(),
                            Headers = e.Request.Headers?.ToDictionary(h => h.Key, h => h.Value),
                            PostData = e.Request.PostData,
                            IsNavigationRequest = e.Request.IsNavigationRequest
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.Request += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupRequestFinishedHandler(PageEventHandler handler)
        {
            EventHandler<RequestEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupRequestFinishedEvent
                        {
                            RequestId = Guid.NewGuid().ToString("N"),
                            Url = e.Request.Url,
                            Method = e.Request.Method.ToString(),
                            Status = e.Request.Response != null ? (int?)e.Request.Response.Status : null,
                            StatusText = e.Request.Response?.StatusText
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.RequestFinished += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupRequestFailedHandler(PageEventHandler handler)
        {
            EventHandler<RequestEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupRequestFailedEvent
                        {
                            RequestId = Guid.NewGuid().ToString("N"),
                            Url = e.Request.Url,
                            Method = e.Request.Method.ToString(),
                            ErrorText = e.Request.FailureText
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.RequestFailed += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupResponseHandler(PageEventHandler handler)
        {
            EventHandler<ResponseCreatedEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupResponseEvent
                        {
                            RequestId = Guid.NewGuid().ToString("N"),
                            Url = e.Response.Url,
                            Status = (int)e.Response.Status,
                            StatusText = e.Response.StatusText,
                            Headers = e.Response.Headers?.ToDictionary(h => h.Key, h => h.Value),
                            FromCache = e.Response.FromCache
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.Response += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupFrameAttachedHandler(PageEventHandler handler)
        {
            EventHandler<FrameEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var pupFrame = PupFrame.CreateAsync(e.Frame).GetAwaiter().GetResult();
                        var eventData = new PupFrameAttachedEvent { Frame = pupFrame };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.FrameAttached += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupFrameDetachedHandler(PageEventHandler handler)
        {
            EventHandler<FrameEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupFrameDetachedEvent
                        {
                            FrameId = e.Frame.Id,
                            Url = e.Frame.Url,
                            Name = "" // Name is deprecated and not reliable during detach
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.FrameDetached += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupFrameNavigatedHandler(PageEventHandler handler)
        {
            EventHandler<FrameNavigatedEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var pupFrame = PupFrame.CreateAsync(e.Frame).GetAwaiter().GetResult();
                        var eventData = new PupFrameNavigatedEvent { Frame = pupFrame };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.FrameNavigated += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupDownloadHandler(PageEventHandler handler)
        {
            var downloads = new ConcurrentDictionary<string, PupDownloadEvent>();

            EventHandler<MessageEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (e.MessageID == "Browser.downloadWillBegin")
                    {
                        var guid = e.MessageData.GetProperty("guid").GetString();
                        var eventData = new PupDownloadEvent
                        {
                            Guid = guid,
                            Url = e.MessageData.GetProperty("url").GetString(),
                            SuggestedFilename = e.MessageData.GetProperty("suggestedFilename").GetString(),
                            State = "started"
                        };
                        downloads[guid] = eventData;

                        handler.ScriptBlock?.Invoke(eventData);
                    }
                    else if (e.MessageID == "Browser.downloadProgress")
                    {
                        var guid = e.MessageData.GetProperty("guid").GetString();
                        var state = e.MessageData.GetProperty("state").GetString();

                        if (state == "completed" || state == "canceled")
                        {
                            downloads.TryGetValue(guid, out var startEvent);

                            var eventData = new PupDownloadEvent
                            {
                                Guid = guid,
                                Url = startEvent?.Url,
                                SuggestedFilename = startEvent?.SuggestedFilename,
                                State = state,
                                TotalBytes = e.MessageData.TryGetProperty("totalBytes", out var tb) ? (long?)tb.GetInt64() : null
                            };

                            if (state == "completed" && Page.DownloadPath != null && startEvent?.SuggestedFilename != null)
                            {
                                var guidPath = System.IO.Path.Combine(Page.DownloadPath, guid);
                                var finalPath = System.IO.Path.Combine(Page.DownloadPath, startEvent.SuggestedFilename);
                                try
                                {
                                    if (File.Exists(finalPath))
                                        File.Delete(finalPath);
                                    if (File.Exists(guidPath))
                                        File.Move(guidPath, finalPath);
                                    eventData.Path = finalPath;
                                }
                                catch
                                {
                                    eventData.Path = File.Exists(guidPath) ? guidPath : null;
                                }
                            }

                            handler.ScriptBlock?.Invoke(eventData);
                            downloads.TryRemove(guid, out _);
                        }
                    }
                }
                catch { }
            };

            Page.Page.Client.MessageReceived += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupFileChooserHandler(PageEventHandler handler)
        {
            // FileChooser in PuppeteerSharp uses WaitForFileChooserAsync, not events
            // This handler is a no-op - use WaitForFileChooserAsync directly instead
            WriteWarning("FileChooser uses WaitForFileChooserAsync pattern in PuppeteerSharp. Use Send-PupFile command instead.");
        }

        private void SetupWorkerCreatedHandler(PageEventHandler handler)
        {
            EventHandler<WorkerEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupWorkerCreatedEvent { Url = e.Worker.Url };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.WorkerCreated += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupWorkerDestroyedHandler(PageEventHandler handler)
        {
            EventHandler<WorkerEventArgs> nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupWorkerDestroyedEvent { Url = e.Worker.Url };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.WorkerDestroyed += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void SetupCloseHandler(PageEventHandler handler)
        {
            EventHandler nativeHandler = (sender, e) =>
            {
                try
                {
                    if (handler.ScriptBlock != null)
                    {
                        var eventData = new PupCloseEvent
                        {
                            Url = Page.Url,
                            Title = Page.Title
                        };
                        handler.ScriptBlock.Invoke(eventData);
                    }
                }
                catch { }
            };

            Page.Page.Close += nativeHandler;
            handler.NativeHandler = nativeHandler;
        }

        private void RemoveExistingHandler()
        {
            lock (Page.HandlersLock)
            {
                if (!Page.EventHandlers.TryGetValue(Event, out var existing))
                    return;

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
                    case PupPageEvent.Download:
                        if (existing.NativeHandler is EventHandler<MessageEventArgs> dlHandler)
                            Page.Page.Client.MessageReceived -= dlHandler;
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
        }
    }
}
