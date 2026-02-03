using System;
using PuppeteerSharp;
using System.IO;
using System.Linq;
using Pup.Transport;
using System.Management.Automation;
using System.Collections.Generic;
using Pup.Common;

namespace Pup.Services
{

    public interface ISupportedBrowserService
    {
        void Cleanup();
        PupBrowser DownloadBrowser(PupSupportedBrowser browserType);
        PupBrowser GetBrowser(PupSupportedBrowser browserType);
        List<PupBrowser> GetBrowsers();
        bool IsBrowserTypeInstalled(PupSupportedBrowser browserType);
        PupBrowser StartBrowser(PupSupportedBrowser browserType, bool headless, int width, int height);
    }

    public class SupportedBrowserService : ISupportedBrowserService
    {
        protected readonly SessionStateService<PupBrowser> _sessionStateService;
        private const string RunningBrowsersKey = "RunningBrowsers";

        public SupportedBrowserService(SessionState sessionState)
        {
            _sessionStateService = new SessionStateService<PupBrowser>(sessionState, RunningBrowsersKey);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Cleanup();
        }

        public bool IsBrowserTypeInstalled(PupSupportedBrowser browserType)
        {
            var path = GetBrowserTypeInstallPath(browserType);
            return Directory.Exists(path);
        }

        private static string GetBrowserTypeInstallPath(PupSupportedBrowser supportedBrowser)
        {
            var storagePath = GetBrowserInstallPath();
            return Path.Combine(storagePath, supportedBrowser.ToString());
        }

        private static string GetBrowserInstallPath()
        {
            var browserPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Pup",
                "Browsers"
            );

            return browserPath;
        }

        public static string[] GetInstalledBrowserTypes()
        {
            var storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Pup", "Browsers"
            );

            if (!Directory.Exists(storagePath))
            {
                return Array.Empty<string>();
            }
            var supportedBrowserNames = Enum.GetNames(typeof(PupSupportedBrowser));
            return Directory.GetDirectories(storagePath)
                .Where(dir => supportedBrowserNames.Contains(Path.GetFileName(dir)))
                .Select(dir => Path.GetFileName(dir))
                .ToArray();
        }
        public PupBrowser GetBrowser(PupSupportedBrowser browserType)
        {
            return GetBrowsers().FirstOrDefault(b => b.BrowserType == browserType);
        }

        public List<PupBrowser> GetBrowsers()
        {
            var storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Pup", "Browsers"
            );

            if (!Directory.Exists(storagePath))
            {
                return new List<PupBrowser>();
            }
            var supportedBrowserNames = Enum.GetNames(typeof(PupSupportedBrowser));
            var browsers = Directory.GetDirectories(storagePath)
                .Where(dir => supportedBrowserNames.Contains(Path.GetFileName(dir)))
                .Select(dir =>
                {
                    var key = Path.GetFileName(dir);
                    var browser = _sessionStateService.Get(key);
                    return browser ?? new PupBrowser(Path.GetFileName(dir).ToPBSupportedBrowser(), dir);
                }).ToList();

            return browsers;
        }

        public PupBrowser DownloadBrowser(PupSupportedBrowser browserType)
        {
            var namedBrowserPath = GetBrowserTypeInstallPath(browserType);
            Directory.CreateDirectory(namedBrowserPath);

            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = namedBrowserPath,
                Browser = (SupportedBrowser)Enum.Parse(typeof(SupportedBrowser), browserType.ToString())
            });

            browserFetcher.DownloadAsync().GetAwaiter().GetResult();

            return GetBrowser(browserType);
        }

        public PupBrowser StartBrowser(PupSupportedBrowser browserType, bool headless, int width, int height)
        {
            var path = GetBrowserTypeInstallPath(browserType);
            var browserTypeName = browserType.ToString();
            var launchOptions = new LaunchOptions
            {
                Headless = headless,
                DefaultViewport = new ViewPortOptions
                {
                    Width = width,
                    Height = height
                }
            };

            // Set the executable path using BrowserFetcher
            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = path,
                Browser = (SupportedBrowser)Enum.Parse(typeof(SupportedBrowser), browserTypeName)
            });

            var installedBrowsers = browserFetcher.GetInstalledBrowsers().ToArray();
            if (installedBrowsers.Length == 0)
            {
                throw new InvalidOperationException($"No browser installations found in {path}");
            }

            var browserInfo = installedBrowsers[0]; // Use the first (and should be only) installation
            launchOptions.ExecutablePath = browserInfo.GetExecutablePath();

            var browser = Puppeteer.LaunchAsync(launchOptions).GetAwaiter().GetResult();

            //default page close
            var pages = browser.PagesAsync().GetAwaiter().GetResult();
            var defaultPage = pages.FirstOrDefault();

            // Closing the only page in a headful browser will close the window/process.
            // Only close the default about:blank tab when running headless or when other pages exist.
            if (defaultPage != null && defaultPage.Url == "about:blank" && (headless || pages.Length > 1))
            {
                defaultPage.CloseAsync().GetAwaiter().GetResult();
            }

            var pbrowser = new PupBrowser(
                browser,
                headless,
                $"{width}x{height}",
                path
            );

            _sessionStateService.Save(browserTypeName, pbrowser);

            return pbrowser;
        }
        public void Cleanup()
        {
            GetBrowsers().Where(b => b.Running).ToList().ForEach(b => b.Browser.CloseAsync().GetAwaiter().GetResult());
        }
    }
}
