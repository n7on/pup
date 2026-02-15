using Pup.Services;
using Pup.Transport;

public static class ServiceFactory
{
    public static ISupportedBrowserService CreateSupportedBrowserService()
    {
        return new SupportedBrowserService();
    }
    public static IBrowserService CreateBrowserService(PupBrowser browser)
    {
        return new BrowserService(browser);
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

    public static IFrameService CreateFrameService(PupFrame frame)
    {
        return new FrameService(frame);
    }

    public static IRecordingService CreateRecordingService(PupPage page)
    {
        return new RecordingService(page);
    }

    public static IConsoleService CreateConsoleService(PupPage page)
    {
        return new ConsoleService(page);
    }

    public static ICdpService CreateCdpService(PupPage page)
    {
        return new CdpService(page);
    }

    public static ICertificateService CreateCertificateService(PupPage page)
    {
        var cdpService = CreateCdpService(page);
        return new CertificateService(page, cdpService);
    }
}