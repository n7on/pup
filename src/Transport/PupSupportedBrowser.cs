using System;
using PuppeteerSharp;

namespace Pup.Transport
{

    public enum PupSupportedBrowser
    {
        /// <summary>
        /// Chrome.
        /// </summary>
        Chrome,

        /// <summary>
        /// Firefox.
        /// </summary>
        Firefox,

        /// <summary>
        /// Chromium.
        /// </summary>
        Chromium,

        /// <summary>
        /// Chrome headless shell.
        /// </summary>
        ChromeHeadlessShell,
    }
    public static class PBSupportedBrowserExtensions
    {

        public static PupSupportedBrowser ToPBSupportedBrowser(this SupportedBrowser browserType)
        {
            if (Enum.TryParse<PupSupportedBrowser>(browserType.ToString(), out var result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid browser type: {browserType}");
        }
        public static PupSupportedBrowser ToPBSupportedBrowser(this string browserType)
        {
            if (Enum.TryParse<PupSupportedBrowser>(browserType, true, out var result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid browser type: {browserType}");
        }
        public static string GetFriendlyName(this PupSupportedBrowser browser)
        {
            return browser switch
            {
                PupSupportedBrowser.Chrome => "Google Chrome",
                PupSupportedBrowser.Firefox => "Mozilla Firefox",
                PupSupportedBrowser.Chromium => "Chromium",
                PupSupportedBrowser.ChromeHeadlessShell => "Chrome Headless Shell",
                _ => "Unknown Browser"
            };
        }
    }
}
