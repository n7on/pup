using System.Collections.Generic;
using System.Threading.Tasks;
using Pup.Common;
using Pup.Transport;

    public interface ISupportedBrowserService
    {
        void Cleanup();
        void DownloadBrowser(PupSupportedBrowser browserType);
        PupBrowser GetBrowser(PupSupportedBrowser browserType);
        List<PupBrowser> GetBrowsers();
        bool IsBrowserTypeInstalled(PupSupportedBrowser browserType);
        PupBrowser StartBrowser(PupSupportedBrowser browserType, bool headless, int width, int height);
    }