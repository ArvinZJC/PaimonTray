using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The role model.
    /// </summary>
    public class Role
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
    } // end class Role
} // end namespace PaimonTray.Models