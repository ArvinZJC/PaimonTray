namespace PaimonTray.Models.RealTimeNotes
{
    /// <summary>
    /// The real-time notes' general note model.
    /// </summary>
    internal class GeneralNote : ExpeditionNote
    {
        #region Properties

        /// <summary>
        /// The explanation.
        /// </summary>
        public string Explanation { get; set; }

        /// <summary>
        /// The title.
        /// </summary>
        public string Title { get; set; }

        #endregion Properties
    } // end class GeneralNote
} // end namespace PaimonTray.Models.RealTimeNotes