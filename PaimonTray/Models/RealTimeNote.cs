using System;

namespace PaimonTray.Models
{
    /// <summary>
    /// The real-time note model.
    /// </summary>
    public class RealTimeNote
    {
        #region Properties

        /// <summary>
        /// The explanation.
        /// </summary>
        public string Explanation { get; init; }

        /// <summary>
        /// The status.
        /// </summary>
        public string Status { get; init; }

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// The image URI.
        /// </summary>
        public Uri UriImage { get; init; }

        #endregion Properties
    } // end class RealTimeNote
} // end namespace PaimonTray.Models