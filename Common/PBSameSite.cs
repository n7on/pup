using System;
using PuppeteerSharp;

namespace PowerBrowser.Common
{

    public enum PBSameSite
    {
        /// <summary>
        /// No SameSite attribute.
        /// </summary>
        None,

        /// <summary>
        /// SameSite Lax.
        /// </summary>
        Lax,

        /// <summary>
        /// SameSite Strict.
        /// </summary>
        Strict,

        /// <summary>
        /// SameSite Extended.
        /// </summary>
        Extended
    }

    public static class PBSameSiteExtensions
    {
        public static SameSite? ToPuppeteerSameSiteMode(this PBSameSite? sameSite)
        {
            return sameSite switch
            {
                PBSameSite.None => SameSite.None,
                PBSameSite.Lax => SameSite.Lax,
                PBSameSite.Strict => SameSite.Strict,
                PBSameSite.Extended => SameSite.Extended,
                _ => SameSite.None,
            };
        }
        public static PBSameSite? ToSupportedPBSameSite(this SameSite? sameSite)
        {
            return sameSite switch
            {
                SameSite.None => PBSameSite.None,
                SameSite.Lax => PBSameSite.Lax,
                SameSite.Strict => PBSameSite.Strict,
                SameSite.Extended => PBSameSite.Extended,
                _ => PBSameSite.None,
            };
        }
    }
}