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
        public string Cookies { get; init; }

        /// <summary>
        /// The character's flag indicating if the character is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The account's key in the application data.
        /// </summary>
        public string Key { get; init; }

        /// <summary>
        /// The character's level.
        /// </summary>
        public string Level { get; init; }

        /// <summary>
        /// The account's nickname.
        /// </summary>
        public string NicknameAccount { get; init; }

        /// <summary>
        /// The character's nickname.
        /// </summary>
        public string NicknameCharacter { get; init; }

        /// <summary>
        /// The character's region.
        /// </summary>
        public string Region { get; init; }

        /// <summary>
        /// The account's server.
        /// </summary>
        public string Server { get; init; }

        /// <summary>
        /// The account's status.
        /// </summary>
        public string Status { get; init; }

        /// <summary>
        /// The account's last update time.
        /// </summary>
        public DateTimeOffset? TimeUpdateLast { get; init; }

        /// <summary>
        /// The account UID.
        /// </summary>
        public string UidAccount { get; init; }

        /// <summary>
        /// The character UID.
        /// </summary>
        public string UidCharacter { get; init; }

        #endregion Properties
    } // end class AccountCharacter
} // end namespace PaimonTray.Models