using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The character model.
    /// </summary>
    public class Character
    {
        /// <summary>
        /// The level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// The nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// The region.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The user ID.
        /// </summary>
        [JsonPropertyName("game_uid")]
        public string UserId { get; set; }
    } // end class Character
} // end namespace PaimonTray.Models