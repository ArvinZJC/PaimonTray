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
        public string Explanation { get; set; }

        /// <summary>
        /// The status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The image URI.
        /// </summary>
        public Uri UriImage { get; set; }

        #endregion Properties
    } // end class RealTimeNote
} // end namespace PaimonTray.Models