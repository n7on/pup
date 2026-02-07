using System.ComponentModel.DataAnnotations;
using System.Management.Automation;
using Pup.Services;
using Pup.Transport;

public static class ServiceFactory
{
    public static ISupportedBrowserService CreateSupportedBrowserService(SessionState sessionState)
    {
        var supportedBrowserService = new SupportedBrowserService(sessionState);
        return supportedBrowserService;
    }
    public static IBrowserService CreateBrowserService(PupBrowser browser, SessionState sessionState)
    {
        var browserService = new BrowserService(browser, sessionState);
        return browserService;
    }

    public static IPageService CreatePageService(PupPage page)
    {
        var pageService = new PageService(page);
        return pageService;
    }

    public static IElementService CreateElementService(PupElement element)
    {
        var elementService = new ElementService(element);
        return elementService;
    }

    public static IRecordingService CreateRecordingService()
    {
        return new RecordingService();
    }

    public static IConsoleService CreateConsoleService(PupPage page)
    {
        return new ConsoleService(page);
    }

    public static ICdpService CreateCdpService(PupPage page)
    {
        return new CdpService(page);
    }
}