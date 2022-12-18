using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The character model.
    /// </summary>
    // ReSharper disable ClassNeverInstantiated.Global, UnusedAutoPropertyAccessor.Global
    public class Character
    {
        #region Properties

        /// <summary>
        /// The level.
        /// </summary>
        public int? Level { get; init; }

        /// <summary>
        /// The nickname.
        /// </summary>
        public string Nickname { get; init; }

        /// <summary>
        /// The region.
        /// </summary>
        public string Region { get; init; }

        /// <summary>
        /// The character UID.
        /// </summary>
        [JsonPropertyName("game_uid")]
        public string Uid { get; init; }

        #endregion Properties
    } // end class Character
} // end namespace PaimonTray.Models