using PuppeteerSharp;

namespace Pup.Transport
{

    public enum PupSameSite
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

    public static class PupSameSiteExtensions
    {
        public static SameSite? ToPuppeteerSameSiteMode(this PupSameSite? sameSite)
        {
            return sameSite switch
            {
                PupSameSite.None => SameSite.None,
                PupSameSite.Lax => SameSite.Lax,
                PupSameSite.Strict => SameSite.Strict,
                PupSameSite.Extended => SameSite.Extended,
                _ => SameSite.None,
            };
        }
        public static PupSameSite? ToPupSameSite(this SameSite? sameSite)
        {
            return sameSite switch
            {
                SameSite.None => PupSameSite.None,
                SameSite.Lax => PupSameSite.Lax,
                SameSite.Strict => PupSameSite.Strict,
                SameSite.Extended => PupSameSite.Extended,
                _ => PupSameSite.None,
            };
        }
    }
}
