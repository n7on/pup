using System;
using PuppeteerSharp;
using System.IO;
using System.Linq;
using PowerBrowser.Transport;
using System.Management.Automation;
using System.Collections.Generic;
using PowerBrowser.Common;

namespace PowerBrowser.Services
{

    public class BrowserService : IBrowserService
    {
        private const string RunningBrowsersKey = "RunningBrowsers";

        public bool IsBrowserTypeInstalled(PBSupportedBrowser browserType)
        {
            var path = GetBrowserTypeInstallPath(browserType);
            return Directory.Exists(path);
        }
        private readonly SessionStateService<PBBrowser> _sessionStateService;

        public BrowserService(SessionState sessionState)
        {
            _sessionStateService = new SessionStateService<PBBrowser>(sessionState, RunningBrowsersKey);
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Cleanup();
        }

        private string GetBrowserTypeInstallPath(PBSupportedBrowser supportedBrowser)
        {
            var storagePath = GetBrowserInstallPath();
            return Path.Combine(storagePath, supportedBrowser.ToString());
        }

        private string GetBrowserInstallPath()
        {
            var browserPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PowerBrowser",
                "Browsers"
            );

            return browserPath;
        }

        public static string[] GetInstalledBrowserTypes()
        {
            var storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PowerBrowser", "Browsers"
            );

            if (!Directory.Exists(storagePath))
            {
                return Array.Empty<string>();
            }
            var supportedBrowserNames = Enum.GetNames(typeof(PBSupportedBrowser));
            return Directory.GetDirectories(storagePath)
                .Where(dir => supportedBrowserNames.Contains(Path.GetFileName(dir)))
                .Select(dir => Path.GetFileName(dir))
                .ToArray();
        }
        public PBBrowser GetBrowser(PBSupportedBrowser browserType)
        {
            return GetBrowsers().FirstOrDefault(b => b.BrowserType == browserType);
        }
        public List<PBBrowser> GetBrowsers()
        {
            var storagePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PowerBrowser", "Browsers"
            );

            if (!Directory.Exists(storagePath))
            {
                return new List<PBBrowser>();
            }
            var supportedBrowserNames = Enum.GetNames(typeof(PBSupportedBrowser));
            var browsers = Directory.GetDirectories(storagePath)
                .Where(dir => supportedBrowserNames.Contains(Path.GetFileName(dir)))
                .Select(dir => {
                    var key = Path.GetFileName(dir);
                    var browser = _sessionStateService.Get(key);
                    return browser ?? new PBBrowser(Path.GetFileName(dir).ToPBSupportedBrowser(), dir);
                }).ToList();
            
            return browsers;
        }

        public bool RemoveBrowser(PBBrowser browser)
        {
            if (Directory.Exists(browser.Path))
            {
                Directory.Delete(browser.Path, true);
                return true;
            }
            return false;
        }


        public void DownloadBrowser(PBSupportedBrowser browserType)
        {
            var namedBrowserPath = GetBrowserTypeInstallPath(browserType);
            Directory.CreateDirectory(namedBrowserPath);

            var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            {
                Path = namedBrowserPath,
                Browser = (SupportedBrowser)Enum.Parse(typeof(SupportedBrowser), browserType.ToString())
            });

            browserFetcher.DownloadAsync().GetAwaiter().GetResult();
        }

        public bool StopBrowser(PBBrowser browser)
        {

            if (!browser.Running)
            {
                return false;
            }

            browser.Browser.CloseAsync().GetAwaiter().GetResult();

            _sessionStateService.Remove(browser.BrowserType.ToString());

            return true;
        }
        public PBBrowser StartBrowser(PBSupportedBrowser browserType, bool headless, int width, int height)
        {
            var installedBrowser = GetBrowser(browserType);
            if (installedBrowser != null && installedBrowser.Running)
            {
                StopBrowser(installedBrowser);
            }

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
            var defaultPage = browser.PagesAsync().GetAwaiter().GetResult().FirstOrDefault();
            if (defaultPage != null && defaultPage.Url == "about:blank")
                defaultPage.CloseAsync().GetAwaiter().GetResult();

            var pbrowser = new PBBrowser(
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
            GetBrowsers().Where(b => b.Running).ToList().ForEach(b => StopBrowser(b));
        }
    }
}