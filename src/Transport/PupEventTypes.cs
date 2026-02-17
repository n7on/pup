namespace Pup.Transport
{
    public enum PupBrowserEvent
    {
        PopupCreated,
        PageCreated,
        PageClosed,
        Disconnected
    }

    public enum PupPageEvent
    {
        Dialog,
        Console,
        PageError,
        Load,
        DOMContentLoaded,
        Request,
        RequestFinished,
        RequestFailed,
        Response,
        FrameAttached,
        FrameDetached,
        FrameNavigated,
        Download,
        FileChooser,
        WorkerCreated,
        WorkerDestroyed,
        Close
    }

    public enum PupHandlerAction
    {
        // For PopupCreated
        Dismiss,

        // For Dialog
        Accept,
        // Dismiss also applies

        // Generic
        Ignore
    }
}
