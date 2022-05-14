using Microsoft.UI.Xaml.Controls;
using PaimonTray.Models;
using PaimonTray.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
        /// The characters container key.
        /// </summary>
        public const string ContainerKeyCharacters = "characters";

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
        private const string HeaderValueAppVersionServerCn = "2.28.1";

        /// <summary>
        /// The app version RPC header value for the global server.
        /// </summary>
        private const string HeaderValueAppVersionServerGlobal = "2.10.1";

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
        /// The tag for adding an account.
        /// </summary>
        public const string TagAddAccount = "addAccount";

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

        private readonly ApplicationDataContainer _applicationDataContainerAccounts;
        private readonly HttpClient _httpClient;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the accounts helper.
        /// </summary>
        public AccountsHelper()
        {
            _applicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(ContainerKeyAccounts,
                    ApplicationDataCreateDisposition
                        .Always); // The container's containers are in a read-only dictionary, and should not be stored.
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept
                .ParseAdd(HeaderValueAccept); // The specific Accept header should be sent with each request.
        } // end constructor AccountsHelper

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Add the main window's navigation view items for a specific account's characters.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="shouldSelectFirst">A flag indicating if the 1st character should be selected in the navigation view.</param>
        public static void AddAccountNavigation(string containerKeyAccount, bool shouldSelectFirst = true)
        {
            var applicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.Containers[ContainerKeyAccounts];

            if (!applicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return;
            } // end if

            foreach (var existingWindow in WindowsHelper.ExistingWindowList.Where(existingWindow =>
                         existingWindow is MainWindow))
            {
                var applicationDataContainerAccount = applicationDataContainerAccounts.Containers[containerKeyAccount];

                if (!applicationDataContainerAccount.Containers.ContainsKey(ContainerKeyCharacters))
                {
                    Log.Warning(
                        $"No character for adding the main window's navigation view items (account container key: {containerKeyAccount}).");
                    return;
                } // end if

                var navigationViewBody = ((MainWindow)existingWindow).NavigationViewBody;

                foreach (var keyValuePairCharacter in applicationDataContainerAccount.Containers[ContainerKeyCharacters]
                             .Containers)
                {
                    var propertySetCharacter = keyValuePairCharacter.Value.Values;

                    if (!(bool)propertySetCharacter[KeyIsEnabled]) continue;

                    var navigationViewItemCharacter = new NavigationViewItem()
                    {
                        Icon = new SymbolIcon(Symbol.Contact),
                        Tag = new KeyValuePair<string, string>(containerKeyAccount, keyValuePairCharacter.Key)
                    };

                    ToolTipService.SetToolTip(navigationViewItemCharacter,
                        $"{propertySetCharacter[KeyNickname]} ({keyValuePairCharacter.Key})");
                    navigationViewBody.MenuItems.Insert(navigationViewBody.MenuItems.Count - 1,
                        navigationViewItemCharacter);
                } // end foreach

                if (shouldSelectFirst) navigationViewBody.SelectedItem = navigationViewBody.MenuItems[0];
            } // end foreach
        } // end method AddAccountNavigation

        /// <summary>
        /// Get an account's characters from the API.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Character>> GetCharactersFromApiAsync(string containerKeyAccount)
        {
            if (!_applicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return null;
            } // end if

            string headerValueUserAgent;
            var propertySetAccount = _applicationDataContainerAccounts.Containers[containerKeyAccount].Values;
            string urlCharacters;

            if (propertySetAccount[KeyServer] as string == TagServerCn)
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

            headers.Cookie.ParseAdd(propertySetAccount[KeyCookies] as string);
            headers.UserAgent.ParseAdd(headerValueUserAgent);

            var httpResponseMessage = await _httpClient.SendRequestAsync(httpRequestMessage);

            try
            {
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"The HTTP response to get characters was unsuccessful (account container key: {containerKeyAccount}).");
                Log.Error(exception.ToString());
                return null;
            } // end try...catch

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();
            var account = JsonSerializer.Deserialize<Account>(httpResponseBody,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (account == null)
            {
                Log.Warning($"Failed to parse the response's body (account container key: {containerKeyAccount}):");
                Log.Information(httpResponseBody);
                return null;
            } // end if

            if (account.ReturnCode != 0)
            {
                Log.Warning(
                    $"Failed to get characters from the specific API (account container key: {containerKeyAccount}, message: {account.Message}, return code: {account.ReturnCode}).");
                return null;
            } // end if

            if (account.Data.TryGetValue(KeyList, out var characters)) return characters;

            Log.Warning($"Failed to get the character data list (account container key: {containerKeyAccount}).");
            return null;
        } // end method GetCharactersFromApiAsync

        /// <summary>
        /// Remove the main window's navigation view items for a specific account's characters.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        private static void RemoveAccountNavigation(string containerKeyAccount)
        {
            foreach (var existingWindow in WindowsHelper.ExistingWindowList.Where(existingWindow =>
                         existingWindow is MainWindow))
            {
                var navigationViewBodyMenuItems = ((MainWindow)existingWindow).NavigationViewBody.MenuItems;

                foreach (NavigationViewItem navigationViewBodyMenuItem in navigationViewBodyMenuItems.ToImmutableList())
                {
                    if (navigationViewBodyMenuItem.Tag is not KeyValuePair<string, string> keyValuePairTag) continue;

                    if (keyValuePairTag.Key == containerKeyAccount)
                        navigationViewBodyMenuItems.Remove(navigationViewBodyMenuItem);
                } // end foreach
            } // end foreach
        } // end method RemoveAccountNavigation

        /// <summary>
        /// Add or update the characters, and change the main window's navigation view.
        /// </summary>
        /// <param name="characters">A list of characters.</param>
        /// <param name="containerKeyAccount">The account container key.</param>
        public void StoreCharacters(ImmutableList<Character> characters, string containerKeyAccount)
        {
            if (!_applicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return;
            } // end if

            var applicationDataContainerAccount = _applicationDataContainerAccounts.Containers[containerKeyAccount];

            if (characters.Count == 0)
            {
                applicationDataContainerAccount.DeleteContainer(ContainerKeyCharacters);
                applicationDataContainerAccount.Values[KeyIsEnabled] = true;
                RemoveAccountNavigation(containerKeyAccount);
                return;
            } // end if

            var applicationDataContainerCharacters =
                applicationDataContainerAccount.CreateContainer(ContainerKeyCharacters,
                    ApplicationDataCreateDisposition.Always);

            foreach (var keyValuePairCharacter in applicationDataContainerCharacters.Containers.Where(
                         keyValuePairCharacter => !characters.Select(character => character.UserId).ToImmutableList()
                             .Contains(keyValuePairCharacter.Key)))
                applicationDataContainerCharacters.DeleteContainer(keyValuePairCharacter.Key);

            foreach (var character in characters)
            {
                var propertySetCharacter = applicationDataContainerCharacters
                    .CreateContainer(character.UserId, ApplicationDataCreateDisposition.Always).Values;

                if (!propertySetCharacter.ContainsKey(KeyIsEnabled))
                    propertySetCharacter[KeyIsEnabled] = true;

                propertySetCharacter[KeyLevel] = character.Level;
                propertySetCharacter[KeyNickname] = character.Nickname;
                propertySetCharacter[KeyRegion] = character.Region;
            } // end foreach

            applicationDataContainerAccount.Values[KeyIsEnabled] = true;
            RemoveAccountNavigation(containerKeyAccount);
            AddAccountNavigation(containerKeyAccount);
        } // end method StoreCharacters

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers