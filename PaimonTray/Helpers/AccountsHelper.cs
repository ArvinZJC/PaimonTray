using PaimonTray.Models;
using Serilog;
using System;
using System.Collections.Immutable;
using System.Linq;
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
        /// The cookies key.
        /// </summary>
        public const string KeyCookies = "cookies";

        /// <summary>
        /// The list key.
        /// </summary>
        public const string KeyList = "list";

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
        /// <param name="accountId">The account ID.</param>
        /// <param name="server">The server. Should be the CN/global server tag.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Character>> GetCharactersAsync(string accountId, string server)
        {
            var keyAccount = $"{server}{accountId}";

            Log.Information($"Start to get characters (account key: {keyAccount}).");

            if (!new[] { TagServerCn, TagServerGlobal }.Contains(server))
            {
                Log.Warning("Invalid server.");
                return null;
            } // end if

            if (!_propertySetAccounts.ContainsKey(keyAccount))
            {
                Log.Warning("No such account key.");
                return null;
            } // end if

            string headerValueUserAgent;
            string urlCharacters;

            if (server == TagServerCn)
            {
                headerValueUserAgent = HeaderValueUserAgentServerCn;
                urlCharacters = UrlCharactersServerCn;
            }
            else
            {
                headerValueUserAgent = HeaderValueUserAgentServerGlobal;
                urlCharacters = UrlCharactersServerGlobal;
            } // end if...else

            var applicationDataCompositeValueAccount = (ApplicationDataCompositeValue)_propertySetAccounts[keyAccount];
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
                Log.Error("The HTTP response to get characters was unsuccessful.");
                Log.Error(exception.ToString());
                return null;
            } // end try...catch

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var account = JsonSerializer.Deserialize<Account>(httpResponseBody,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (account == null)
            {
                Log.Warning($"Failed to parse the response's body: {httpResponseBody}");
                return null;
            } // end if

            if (account.ReturnCode != 0)
            {
                Log.Warning(
                    $"Failed to get characters from the server API (message: {account.Message}, return code: {account.ReturnCode}).");
                return null;
            } // end if

            if (account.Data.TryGetValue(KeyList, out var characters)) return characters;

            Log.Warning("Failed to get the character data list.");
            return null;
        } // end method GetCharactersAsync

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers