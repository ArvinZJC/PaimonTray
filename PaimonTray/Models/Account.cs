using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The account model.
    /// </summary>
    internal class Account
    {
        /// <summary>
        /// The role data.
        /// </summary>
        public Dictionary<string, ImmutableList<Role>> Data { get; set; }

        /// <summary>
        /// The response message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The response return code.
        /// </summary>
        [JsonPropertyName("retcode")]
        public int ReturnCode { get; set; }
    } // end class Account
} // end namespace PaimonTray.Models