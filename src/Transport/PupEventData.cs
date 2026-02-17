using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Pup.Transport
{
    // ==================== Browser Events ====================

    public class PupPopupCreatedEvent
    {
        public PupPage Page { get; set; }
        public string Url => Page?.Url;
        public string Title => Page?.Title;
    }

    public class PupPageCreatedEvent
    {
        public PupPage Page { get; set; }
        public string Url => Page?.Url;
        public string Title => Page?.Title;
    }

    public class PupPageClosedEvent
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string TargetId { get; set; }
    }

    public class PupDisconnectedEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    // ==================== Page Events ====================

    public class PupDialogEvent
    {
        private readonly Dialog _dialog;

        public PupDialogEvent(Dialog dialog)
        {
            _dialog = dialog;
            Type = dialog.DialogType.ToString();
            Message = dialog.Message;
            DefaultValue = dialog.DefaultValue;
        }

        public string Type { get; }
        public string Message { get; }
        public string DefaultValue { get; }

        public void Accept(string promptText = null)
        {
            if (promptText != null)
                _dialog.Accept(promptText).GetAwaiter().GetResult();
            else
                _dialog.Accept().GetAwaiter().GetResult();
        }

        public void Dismiss()
        {
            _dialog.Dismiss().GetAwaiter().GetResult();
        }
    }

    public class PupConsoleEvent
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public int? LineNumber { get; set; }
        public int? ColumnNumber { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupPageErrorEvent
    {
        public string Message { get; set; }
        public string Stack { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupLoadEvent
    {
        public string Url { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupDOMContentLoadedEvent
    {
        public string Url { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupRequestEvent
    {
        public string RequestId { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string ResourceType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string PostData { get; set; }
        public bool IsNavigationRequest { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupRequestFinishedEvent
    {
        public string RequestId { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public int? Status { get; set; }
        public string StatusText { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupRequestFailedEvent
    {
        public string RequestId { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public string ErrorText { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupResponseEvent
    {
        public string RequestId { get; set; }
        public string Url { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string MimeType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public bool FromCache { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupFrameAttachedEvent
    {
        public PupFrame Frame { get; set; }
        public string Url => Frame?.Url;
        public string Name => Frame?.Name;
    }

    public class PupFrameDetachedEvent
    {
        public string FrameId { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class PupFrameNavigatedEvent
    {
        public PupFrame Frame { get; set; }
        public string Url => Frame?.Url;
        public string Name => Frame?.Name;
    }

    public class PupDownloadEvent
    {
        public string Url { get; set; }
        public string SuggestedFilename { get; set; }
        public string Guid { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupFileChooserEvent
    {
        private readonly FileChooser _fileChooser;

        public PupFileChooserEvent(FileChooser fileChooser)
        {
            _fileChooser = fileChooser;
            IsMultiple = fileChooser.IsMultiple;
        }

        public bool IsMultiple { get; }

        public void SetFiles(params string[] filePaths)
        {
            _fileChooser.AcceptAsync(filePaths).GetAwaiter().GetResult();
        }

        public void Cancel()
        {
            _fileChooser.CancelAsync().GetAwaiter().GetResult();
        }
    }

    public class PupWorkerCreatedEvent
    {
        public string Url { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupWorkerDestroyedEvent
    {
        public string Url { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PupCloseEvent
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
