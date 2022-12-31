using PaimonTray.Helpers;
using System.Text.Json.Serialization;

namespace PaimonTray.Models
{
    /// <summary>
    /// The GeeTest challenge result model.
    /// </summary>
    // ReSharper disable MemberCanBePrivate.Global, UnusedAutoPropertyAccessor.Global
    internal class GeeTestChallengeResult
    {
        #region Constructors

        /// <summary>
        /// Initialise a GeeTest challenge result model.
        /// </summary>
        /// <param name="challenge">The challenge</param>
        /// <param name="validation">The validation.</param>
        public GeeTestChallengeResult(string challenge, string validation)
        {
            Challenge = challenge;
            SecCode = $"{validation}|jordan";
            Validation = validation;
        } // end constructor GeeTestChallengeResult

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The challenge.
        /// </summary>
        [JsonPropertyName(AccountsHelper.KeyGeeTestChallenge)]
        public string Challenge { get; }

        /// <summary>
        /// The certificate for the secondary verification.
        /// </summary>
        [JsonPropertyName(AccountsHelper.KeyGeeTestSecCode)]
        public string SecCode { get; }

        /// <summary>
        /// The validation.
        /// </summary>
        [JsonPropertyName(AccountsHelper.KeyGeeTestValidation)]
        public string Validation { get; }

        #endregion Properties
    } // end class GeeTestChallengeResult
} // end namespace PaimonTray.Models