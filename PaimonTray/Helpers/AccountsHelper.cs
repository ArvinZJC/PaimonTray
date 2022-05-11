using PaimonTray.Models;
using Serilog;
using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Web.Http;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The accounts helper.
    /// </summary>
    public class AccountsHelper
    {
        #region Constants

        /// <summary>
        /// The accounts container key.
        /// </summary>
        public const string ContainerKeyAccounts = "accounts";

        /// <summary>
        /// The user ID cookie key.
        /// </summary>
        public const string CookieKeyUserId = "ltuid";

        /// <summary>
        /// The token cookie key.
        /// </summary>
        public const string CookieKeyToken = "ltoken";

        /// <summary>
        /// The Accept header value.
        /// </summary>
        private const string HeaderValueAccept = "application/json, text/plain, */*";

        /// <summary>
        /// The app version RPC header value for the CN server.
        /// </summary>
        private const string HeaderValueAppVersionServerCn = "2.27.2";

        /// <summary>
        /// The app version RPC header value for the global server.
        /// </summary>
        private const string HeaderValueAppVersionServerGlobal = "2.9.0";

        /// <summary>
        /// The User-Agent header value for the CN server.
        /// </summary>
        private const string HeaderValueUserAgentServerCn = $"miHoYoBBS/{HeaderValueAppVersionServerCn}";

        /// <summary>
        /// The User-Agent header value for the global server.
        /// </summary>
        private const string HeaderValueUserAgentServerGlobal = $"miHoYoBBSOversea/{HeaderValueAppVersionServerGlobal}";

        /// <summary>
        /// The characters key.
        /// </summary>
        public const string KeyCharacters = "characters";

        /// <summary>
        /// The cookies key.
        /// </summary>
        public const string KeyCookies = "cookies";

        /// <summary>
        /// The ID key.
        /// </summary>
        public const string KeyId = "id";

        /// <summary>
        /// The IsEnabled key.
        /// </summary>
        public const string KeyIsEnabled = "isEnabled";

        /// <summary>
        /// The level key.
        /// </summary>
        public const string KeyLevel = "level";

        /// <summary>
        /// The list key.
        /// </summary>
        public const string KeyList = "list";

        /// <summary>
        /// The nickname key.
        /// </summary>
        public const string KeyNickname = "nickname";

        /// <summary>
        /// The region key.
        /// </summary>
        public const string KeyRegion = "region";

        /// <summary>
        /// The server key.
        /// </summary>
        public const string KeyServer = "server";

        /// <summary>
        /// The user ID key.
        /// </summary>
        public const string KeyUserId = "userId";

        /// <summary>
        /// The CN server tag.
        /// </summary>
        public const string TagServerCn = "cn";

        /// <summary>
        /// The global server tag.
        /// </summary>
        public const string TagServerGlobal = "global";

        /// <summary>
        /// The URL for the CN server to get characters.
        /// </summary>
        private const string UrlCharactersServerCn =
            "https://api-takumi.mihoyo.com/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";

        /// <summary>
        /// The URL for the global server to get characters.
        /// </summary>
        private const string UrlCharactersServerGlobal =
            "https://api-os-takumi.mihoyo.com/binding/api/getUserGameRolesByCookie?game_biz=hk4e_global";

        #endregion Constants

        #region Fields

        private readonly HttpClient _httpClient;
        private readonly IPropertySet _propertySetAccounts;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the accounts helper.
        /// </summary>
        public AccountsHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept
                .ParseAdd(HeaderValueAccept); // The specific Accept header should be sent with each request.
            _propertySetAccounts = ApplicationData.Current.LocalSettings
                .CreateContainer(ContainerKeyAccounts, ApplicationDataCreateDisposition.Always).Values;
        } // end constructor AccountsHelper

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Get the account's characters.
        /// TODO: reduce the method complexity.
        /// </summary>
        /// <param name="keyAccount">The account key.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Character>> GetCharactersAsync(string keyAccount)
        {
            if (!_propertySetAccounts.ContainsKey(keyAccount))
            {
                Log.Warning($"No such account key ({keyAccount}).");
                return null;
            } // end if

            var applicationDataCompositeValueAccount = (ApplicationDataCompositeValue)_propertySetAccounts[keyAccount];
            string headerValueUserAgent;
            string urlCharacters;

            if (applicationDataCompositeValueAccount[KeyServer] as string == TagServerCn)
            {
                headerValueUserAgent = HeaderValueUserAgentServerCn;
                urlCharacters = UrlCharactersServerCn;
            }
            else
            {
                headerValueUserAgent = HeaderValueUserAgentServerGlobal;
                urlCharacters = UrlCharactersServerGlobal;
            } // end if...else

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(urlCharacters));
            var headers = httpRequestMessage.Headers;

            headers.Cookie.ParseAdd(applicationDataCompositeValueAccount[KeyCookies] as string);
            headers.UserAgent.ParseAdd(headerValueUserAgent);

            var httpResponseMessage = await _httpClient.SendRequestAsync(httpRequestMessage);

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Log.Error($"The HTTP response to get characters was unsuccessful (account key: {keyAccount}).");
                Log.Error(exception.ToString());
                return null;
            } // end try...catch

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var account = JsonSerializer.Deserialize<Account>(httpResponseBody,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (account == null)
            {
                Log.Warning($"Failed to parse the response's body (account key: {keyAccount}):");
                Log.Information(httpResponseBody);
                return null;
            } // end if

            if (account.ReturnCode != 0)
            {
                Log.Warning(
                    $"Failed to get characters from the specific API (account key: {keyAccount}, message: {account.Message}, return code: {account.ReturnCode}).");
                return null;
            } // end if

            if (account.Data.TryGetValue(KeyList, out var characters)) return characters;

            Log.Warning($"Failed to get the character data list (account key: {keyAccount}).");
            return null;
        } // end method GetCharactersAsync

        /// <summary>
        /// Add or update the characters.
        /// </summary>
        /// <param name="characters">The characters.</param>
        /// <param name="keyAccount">The account key.</param>
        public void StoreCharacters(ImmutableList<Character> characters, string keyAccount)
        {
            if (!_propertySetAccounts.ContainsKey(keyAccount))
            {
                Log.Warning($"No such account key ({keyAccount}).");
                return;
            } // end if

            var applicationDataCompositeValueAccount = (ApplicationDataCompositeValue)_propertySetAccounts[keyAccount];

            if (characters.Count == 0)
            {
                applicationDataCompositeValueAccount.Remove(KeyCharacters);
                _propertySetAccounts[keyAccount] = applicationDataCompositeValueAccount;
                return;
            } // end if

            var applicationDataCompositeValueCharacters =
                applicationDataCompositeValueAccount.ContainsKey(KeyCharacters)
                    ? (ApplicationDataCompositeValue)applicationDataCompositeValueAccount[KeyCharacters]
                    : new ApplicationDataCompositeValue();
            var applicationDataCompositeValueCharactersNew = new ApplicationDataCompositeValue();

            foreach (var character in characters)
                applicationDataCompositeValueCharactersNew[character.UserId] = new ApplicationDataCompositeValue
                {
                    [KeyIsEnabled] = applicationDataCompositeValueCharacters.ContainsKey(character.UserId)
                        ? (applicationDataCompositeValueCharacters[character.UserId] as
                            ApplicationDataCompositeValue)?[KeyIsEnabled]
                        : true,
                    [KeyLevel] = character.Level,
                    [KeyNickname] = character.Nickname
                };

            applicationDataCompositeValueAccount[KeyCharacters] = applicationDataCompositeValueCharactersNew;
            applicationDataCompositeValueAccount[KeyIsEnabled] = true;
            _propertySetAccounts[keyAccount] = applicationDataCompositeValueAccount;
        } // end method StoreCharacters

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers