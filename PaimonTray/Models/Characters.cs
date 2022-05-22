using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The characters model.
    /// </summary>
    internal class Characters
    {
        #region Properties

        /// <summary>
        /// The character data.
        /// </summary>
        public Dictionary<string, ImmutableList<Character>> Data { get; set; }

        /// <summary>
        /// The response message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The response return code.
        /// </summary>
        [JsonPropertyName("retcode")]
        public int ReturnCode { get; set; }

        #endregion Properties
    } // end class Characters
} // end namespace PaimonTray.Models