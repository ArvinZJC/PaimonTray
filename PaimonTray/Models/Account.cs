using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The account model.
    /// </summary>
    public class Account
    {
        #region Properties

        /// <summary>
        /// The avatar. Should be a code.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// The nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The UID.
        /// </summary>
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        #endregion Properties
    } // end class Account
} // end namespace PaimonTray.Models