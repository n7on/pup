

using System.Collections.Generic;
using PowerBrowser.Common;
using PowerBrowser.Transport;

public interface IBrowserService
{

    bool IsBrowserTypeInstalled(PBSupportedBrowser browserType);
    PBBrowser GetBrowser(PBSupportedBrowser browserType);
    List<PBBrowser> GetBrowsers();

    bool RemoveBrowser(PBBrowser browser);

    void DownloadBrowser(PBSupportedBrowser browserType);

    bool StopBrowser(PBBrowser browser);

    PBBrowser StartBrowser(PBSupportedBrowser browserType, bool headless, int width, int height);

}