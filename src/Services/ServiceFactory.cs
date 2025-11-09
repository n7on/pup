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
}