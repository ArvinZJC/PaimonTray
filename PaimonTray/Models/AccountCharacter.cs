using System;

namespace PaimonTray.Models
{
    /// <summary>
    /// The account's character model.
    /// </summary>
    internal class AccountCharacter
    {
        #region Properties

        /// <summary>
        /// The account's cookies.
        /// </summary>
        public string Cookies { get; set; }

        /// <summary>
        /// The character's flag indicating if the character is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The account's key in the application data.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The character's level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// The account's nickname.
        /// </summary>
        public string NicknameAccount { get; set; }

        /// <summary>
        /// The character's nickname.
        /// </summary>
        public string NicknameCharacter { get; set; }

        /// <summary>
        /// The character's region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The account's server.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The account's status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The account's last update time.
        /// </summary>
        public DateTimeOffset? TimeUpdateLast { get; set; }

        /// <summary>
        /// The account's UID.
        /// </summary>
        public string UidAccount { get; set; }

        /// <summary>
        /// The character UID.
        /// </summary>
        public string UidCharacter { get; set; }

        #endregion Properties
    } // end class AccountCharacter
} // end namespace PaimonTray.Models