using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The character model.
    /// </summary>
    public class Character
    {
        #region Properties

        /// <summary>
        /// The level.
        /// </summary>
        public int? Level { get; set; }

        /// <summary>
        /// The nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The UID.
        /// </summary>
        [JsonPropertyName("game_uid")]
        public string Uid { get; set; }

        #endregion Properties
    } // end class Character
} // end namespace PaimonTray.Models