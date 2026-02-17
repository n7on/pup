using System;
using PuppeteerSharp;
using System.IO;
using System.Linq;
using Pup.Transport;
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
        PupBrowser StartBrowser(PupSupportedBrowser browserType, bool headless, int? width, int? height, string proxy = null, string userAgent = null, string[] arguments = null);
    }

    public class SupportedBrowserService : ISupportedBrowserService
    {
        // Realistic Chrome user-agent to avoid bot detection
        public const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36";

        static SupportedBrowserService()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => CleanupAll();
        }

        private static void CleanupAll()
        {
            foreach (var browser in BrowserStore.GetAll().Where(b => b.Running))
            {
                browser.Browser.CloseAsync().GetAwaiter().GetResult();
            }
            BrowserStore.Clear();
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
                    var browser = BrowserStore.Get(key);
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

        public PupBrowser StartBrowser(PupSupportedBrowser browserType, bool headless, int? width, int? height, string proxy = null, string userAgent = null, string[] arguments = null)
        {
            var path = GetBrowserTypeInstallPath(browserType);
            var browserTypeName = browserType.ToString();

            // Build browser arguments
            var args = new List<string>();
            if (!string.IsNullOrEmpty(proxy))
            {
                args.Add($"--proxy-server={proxy}");
            }

            // Use realistic UA by default, custom if specified, or "none" to use browser's native UA
            var effectiveUserAgent = string.Equals(userAgent, "none", StringComparison.OrdinalIgnoreCase)
                ? null
                : (string.IsNullOrEmpty(userAgent) ? DefaultUserAgent : userAgent);
            if (!string.IsNullOrEmpty(effectiveUserAgent))
            {
                args.Add($"--user-agent=\"{effectiveUserAgent}\"");
            }

            if (arguments != null)
            {
                args.AddRange(arguments);
            }

            // If maximized or fullscreen, don't set a fixed viewport (let it match window size)
            var isMaximizedOrFullscreen = args.Contains("--start-maximized") || args.Contains("--start-fullscreen");

            var launchOptions = new LaunchOptions
            {
                Headless = headless,
                DefaultViewport = (width.HasValue || height.HasValue)
                    ? new ViewPortOptions
                    {
                        Width = width ?? 1280,
                        Height = height ?? 720
                    }
                    : null,
                Args = args.ToArray()
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

            // Select the correct browser: ChromeHeadlessShell for headless, Chrome for headful
            var targetBrowser = headless ? SupportedBrowser.ChromeHeadlessShell : SupportedBrowser.Chrome;
            var browserInfo = installedBrowsers.FirstOrDefault(b => b.Browser == targetBrowser)
                ?? installedBrowsers[0];

            // Error if user wants GUI but only headless shell is available
            if (!headless && browserInfo.Browser == SupportedBrowser.ChromeHeadlessShell)
            {
                throw new InvalidOperationException(
                    "Cannot run in GUI mode: only ChromeHeadlessShell is installed. " +
                    "Run Install-PupBrowser to install the full browser, or use -Headless.");
            }

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

            BrowserStore.Save(browserTypeName, pbrowser);

            return pbrowser;
        }
        public void Cleanup()
        {
            GetBrowsers().Where(b => b.Running).ToList().ForEach(b => b.Browser.CloseAsync().GetAwaiter().GetResult());
        }
    }
}
