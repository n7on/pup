using System.Management.Automation;
using PowerBrowser.Services;

public static class ServiceFactory
{
    public static IBrowserService CreateBrowserService(SessionState sessionState)
    {
        var browserService = new BrowserService(sessionState);
        return browserService;
    }

    public static IPageService CreatePageService(SessionState sessionState)
    {
        var pageService = new PageService(sessionState);
        return pageService;
    }

    public static IElementService CreateElementService()
    {
        var elementService = new ElementService();
        return elementService;
    }
}