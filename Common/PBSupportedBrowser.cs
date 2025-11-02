using System;
using PuppeteerSharp;

namespace PowerBrowser.Common
{

    public enum PBSupportedBrowser
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

        public static PBSupportedBrowser ToSupportedPBrowser(this SupportedBrowser browserType)
        {
            if (Enum.TryParse<PBSupportedBrowser>(browserType.ToString(), out var result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid browser type: {browserType}");
        }
        public static PBSupportedBrowser ToSupportedPBrowser(this string browserType)
        {
            if (Enum.TryParse<PBSupportedBrowser>(browserType, true, out var result))
            {
                return result;
            }
            throw new ArgumentException($"Invalid browser type: {browserType}");
        }
        public static string GetFriendlyName(this PBSupportedBrowser browser)
        {
            return browser switch
            {
                PBSupportedBrowser.Chrome => "Google Chrome",
                PBSupportedBrowser.Firefox => "Mozilla Firefox",
                PBSupportedBrowser.Chromium => "Chromium",
                PBSupportedBrowser.ChromeHeadlessShell => "Chrome Headless Shell",
                _ => "Unknown Browser"
            };
        }
    }
}