namespace PaimonTray.Models
{
    /// <summary>
    /// The account's character model.
    /// </summary>
    internal class AccountCharacter : Character
    {
        #region Properties

        /// <summary>
        /// The account ID.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// The server.
        /// </summary>
        public string Server { get; set; }

        #endregion Properties
    } // end class AccountCharacter
} // end namespace PaimonTray.Models