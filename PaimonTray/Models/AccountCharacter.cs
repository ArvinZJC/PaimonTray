using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The account's character model.
    /// </summary>
    internal class AccountCharacter
    {
        #region Properties

        /// <summary>
        /// The account's key in the application data.
        /// </summary>
        public string Key { get; set; } // It is put first because of being used to order grouped characters.

        /// <summary>
        /// The account's nickname.
        /// </summary>
        public string ANickname { get; set; }

        /// <summary>
        /// The account's UID.
        /// </summary>
        public string AUid { get; set; }

        /// <summary>
        /// The character's nickname.
        /// </summary>
        [JsonIgnore]
        public string CNickname { get; set; }

        /// <summary>
        /// The character's UID.
        /// </summary>
        [JsonIgnore]
        public string CUid { get; set; }

        /// <summary>
        /// The character's level.
        /// </summary>
        [JsonIgnore]
        public string Level { get; set; }

        /// <summary>
        /// The character's region.
        /// </summary>
        [JsonIgnore]
        public string Region { get; set; }

        /// <summary>
        /// The account's server.
        /// </summary>
        public string Server { get; set; }

        #endregion Properties
    } // end class AccountCharacter
} // end namespace PaimonTray.Models