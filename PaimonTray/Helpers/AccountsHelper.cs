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
        /// The Origin header key.
        /// </summary>
        private const string HeaderKeyOrigin = "Origin";

        /// <summary>
        /// The X-Requested-With header key.
        /// </summary>
        private const string HeaderKeyXRequestedWith = "X-Requested-With";

        /// <summary>
        /// The Accept header value.
        /// </summary>
        private const string HeaderValueAccept = "application/json, text/plain, */*";

        /// <summary>
        /// The Origin header value for the CN server.
        /// </summary>
        private const string HeaderValueOriginServerCn = "https://webstatic.mihoyo.com";

        /// <summary>
        /// The Origin header value for the global server.
        /// </summary>
        private const string HeaderValueOriginServerGlobal = "https://webstatic-sea.hoyolab.com";

        /// <summary>
        /// The Referer header value for the CN server.
        /// </summary>
        private const string HeaderValueRefererServerCn = "https://webstatic.mihoyo.com";

        /// <summary>
        /// The Referer header value for the global server.
        /// </summary>
        private const string HeaderValueRefererServerGlobal = "https://webstatic-sea.hoyolab.com";

        /// <summary>
        /// The base User-Agent header value.
        /// </summary>
        private const string HeaderValueUserAgentBase =
            "Mozilla/5.0 (Linux; Android 12; Mi 10 Pro Build/SKQ1.211006.001; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/95.0.4638.74 Mobile Safari/537.36 ";

        /// <summary>
        /// The User-Agent header value for the CN server.
        /// </summary>
        private const string HeaderValueUserAgentServerCn = HeaderValueUserAgentBase + "miHoYoBBS/2.23.1";

        /// <summary>
        /// The User-Agent header value for the global server.
        /// </summary>
        private const string HeaderValueUserAgentServerGlobal = HeaderValueUserAgentBase + "miHoYoBBSOversea/2.9.0";

        /// <summary>
        /// The X-Requested-With header value for the CN server.
        /// </summary>
        private const string HeaderValueXRequestedWithServerCn = "com.mihoyo.hyperion";

        /// <summary>
        /// The X-Requested-With header value for the global server.
        /// </summary>
        private const string HeaderValueXRequestedWithServerGlobal = "com.mihoyo.hoyolab";

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
        /// The URL for the CN server to get roles.
        /// </summary>
        private const string UrlRolesServerCn =
            "https://api-takumi.mihoyo.com/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";

        /// <summary>
        /// The URL for the global server to get roles.
        /// </summary>
        private const string UrlRolesServerGlobal =
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
        /// Get the account's roles.
        /// TODO: reduce the method complexity.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="server">The server. Should be the CN/global server tag.</param>
        /// <returns>A list of roles, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Role>> GetRolesAsync(string accountId, string server)
        {
            var keyAccount = $"{server}{accountId}";

            Log.Information($"Start to get roles (account key: {keyAccount}).");

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

            string headerValueOrigin;
            string headerValueReferer;
            string headerValueUserAgent;
            string headerValueXRequestedWith;
            string urlRoles;

            if (server == TagServerCn)
            {
                headerValueOrigin = HeaderValueOriginServerCn;
                headerValueReferer = HeaderValueRefererServerCn;
                headerValueUserAgent = HeaderValueUserAgentServerCn;
                headerValueXRequestedWith = HeaderValueXRequestedWithServerCn;
                urlRoles = UrlRolesServerCn;
            }
            else
            {
                headerValueOrigin = HeaderValueOriginServerGlobal;
                headerValueReferer = HeaderValueRefererServerGlobal;
                headerValueUserAgent = HeaderValueUserAgentServerGlobal;
                headerValueXRequestedWith = HeaderValueXRequestedWithServerGlobal;
                urlRoles = UrlRolesServerGlobal;
            } // end if...else

            var applicationDataCompositeValueAccount = (ApplicationDataCompositeValue)_propertySetAccounts[keyAccount];
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(urlRoles));
            var headers = httpRequestMessage.Headers;

            headers.Add(HeaderKeyOrigin, headerValueOrigin);
            headers.Add(HeaderKeyXRequestedWith, headerValueXRequestedWith);
            headers.Cookie.ParseAdd(applicationDataCompositeValueAccount[KeyCookies] as string);
            headers.Referer = new Uri(headerValueReferer);
            headers.UserAgent.ParseAdd(headerValueUserAgent);

            var httpResponseMessage = await _httpClient.SendRequestAsync(httpRequestMessage);

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Log.Error("The HTTP response to get roles was unsuccessful.");
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
                    $"Failed to get roles from the server API (message: {account.Message}, return code: {account.ReturnCode}).");
                return null;
            } // end if

            if (account.Data.TryGetValue(KeyList, out var roles)) return roles;

            Log.Warning("Failed to get the role data list.");
            return null;
        } // end method GetRolesAsync

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers