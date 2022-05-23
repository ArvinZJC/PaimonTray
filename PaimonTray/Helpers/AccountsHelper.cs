using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
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
        /// The max number of accounts.
        /// </summary>
        public const int CountAccountsMax = 5;

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
        /// The status key.
        /// </summary>
        public const string KeyStatus = "status";

        /// <summary>
        /// The CN server tag.
        /// </summary>
        public const string TagServerCn = "cn";

        /// <summary>
        /// The global server tag.
        /// </summary>
        public const string TagServerGlobal = "global";

        /// <summary>
        /// The adding status tag.
        /// </summary>
        public const string TagStatusAdding = "adding";

        /// <summary>
        /// The expired status tag.
        /// </summary>
        public const string TagStatusExpired = "expired";

        /// <summary>
        /// The ready status tag.
        /// </summary>
        public const string TagStatusReady = "ready";

        /// <summary>
        /// The updating status tag.
        /// </summary>
        public const string TagStatusUpdating = "updating";

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

        /// <summary>
        /// The HoYoLAB cookies URL.
        /// </summary>
        public const string UrlCookiesHoYoLab = "https://www.hoyolab.com";

        /// <summary>
        /// The miHoYo cookies URL.
        /// </summary>
        public const string UrlCookiesMiHoYo = "https://bbs.mihoyo.com";

        /// <summary>
        /// The URL for indicating the success of logging into miHoYo.
        /// </summary>
        public const string UrlLoginEndMiHoYo = "https://bbs.mihoyo.com/ys/accountCenter/postList?";

        /// <summary>
        /// The URL for logging into HoYoLAB.
        /// </summary>
        public const string UrlLoginHoYoLab = "https://www.hoyolab.com/home";

        /// <summary>
        /// The URL for logging into miHoYo.
        /// </summary>
        public const string UrlLoginMiHoYo = "https://bbs.mihoyo.com/ys/accountCenter";

        /// <summary>
        /// The URL for indicating the success of redirecting to the actual URL for logging into miHoYo.
        /// </summary>
        public const string UrlLoginRedirectMiHoYo =
            "https://user.mihoyo.com/?cb_url=//bbs.mihoyo.com/ys/accountCenter&week=1#/login";

        #endregion Constants

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// The HTTP client.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The resource loader.
        /// </summary>
        private readonly ResourceLoader _resourceLoader;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The accounts application data container.
        /// </summary>
        public ApplicationDataContainer ApplicationDataContainerAccounts { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the accounts helper.
        /// </summary>
        public AccountsHelper()
        {
            _app = Application.Current as App;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept
                .ParseAdd(HeaderValueAccept); // The specific Accept header should be sent with each request.
            _resourceLoader = ResourceLoader.GetForViewIndependentUse();
            ApplicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(ContainerKeyAccounts,
                    ApplicationDataCreateDisposition
                        .Always); // The container's containers are in a read-only dictionary, and should not be stored.

            CheckAccounts();
        } // end constructor AccountsHelper

        #endregion Constructors

        #region Methods

        /// <summary>
        /// TODO: Add the specific account's characters to the main window's navigation view, or update the specific navigation view items.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeysCharacter">A list of characters to add or update for the navigation if possible. Do for all characters if <c>null</c>.</param>
        /// <param name="shouldSelectFirst">A flag indicating if the account's 1st character added in the navigation view should be selected.</param>
        /// <returns>A flag indicating if the operations are successful.</returns>
        public bool AddOrUpdateCharactersNavigation(string containerKeyAccount,
            ImmutableList<string> containerKeysCharacter = null, bool shouldSelectFirst = true)
        {
            if (!ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return false;
            } // end if

            var applicationDataContainerAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount];

            if (!applicationDataContainerAccount.Containers.ContainsKey(ContainerKeyCharacters))
            {
                Log.Warning(
                    $"No character for adding the main window's navigation view items (account container key: {containerKeyAccount}).");
                return false;
            } // end if

            var navigationViewBody = _app.WindowsH.GetMainWindow().NavigationViewBody;
            var navigationViewBodyMenuItems = navigationViewBody.MenuItems;

            foreach (var keyValuePairCharacter in applicationDataContainerAccount.Containers[ContainerKeyCharacters]
                         .Containers)
            {
                if (containerKeysCharacter != null &&
                    !containerKeysCharacter.Contains(keyValuePairCharacter.Key)) continue;

                var navigationViewItemCharacter =
                    (from NavigationViewItem navigationViewBodyMenuItem in
                            navigationViewBodyMenuItems.Take(navigationViewBodyMenuItems.Count - 1).ToImmutableList()
                        let navigationViewBodyMenuItemTag = (KeyValuePair<string, string>)navigationViewBodyMenuItem.Tag
                        where navigationViewBodyMenuItemTag.Key == containerKeyAccount &&
                              navigationViewBodyMenuItemTag.Value == keyValuePairCharacter.Key
                        select navigationViewBodyMenuItem).FirstOrDefault();
                var propertySetCharacter = keyValuePairCharacter.Value.Values;

                if ((bool)propertySetCharacter[KeyIsEnabled])
                {
                    if (navigationViewItemCharacter == null)
                    {
                        navigationViewItemCharacter = new NavigationViewItem()
                        {
                            Icon = new SymbolIcon(Symbol.Contact),
                            Tag = new KeyValuePair<string, string>(containerKeyAccount, keyValuePairCharacter.Key)
                        };
                        navigationViewBodyMenuItems.Insert(navigationViewBodyMenuItems.Count - 1,
                            navigationViewItemCharacter); // TODO: respect order?
                    } // end if

                    ToolTipService.SetToolTip(navigationViewItemCharacter,
                        $"{propertySetCharacter[KeyNickname]} ({keyValuePairCharacter.Key})");
                }
                else if (navigationViewItemCharacter != null)
                    navigationViewBodyMenuItems.Remove(navigationViewItemCharacter);
            } // end foreach

            if (shouldSelectFirst)
                navigationViewBody.SelectedItem =
                    (from NavigationViewItem navigationViewBodyMenuItem in navigationViewBodyMenuItems
                            .Take(navigationViewBodyMenuItems.Count - 1).ToImmutableList()
                        where ((KeyValuePair<string, string>)navigationViewBodyMenuItem.Tag).Key == containerKeyAccount
                        select navigationViewBodyMenuItem).FirstOrDefault();

            return true;
        } // end method AddOrUpdateCharactersNavigation

        /// <summary>
        /// Check the accounts.
        /// </summary>
        private void CheckAccounts()
        {
            // TODO: Status adding/updating consider continue storing characters; expired needs user attention
            if (CountAccounts() <= 0) return;

            foreach (var propertySetAccount in ApplicationDataContainerAccounts.Containers.Values.ToImmutableList()
                         .Select(applicationDataContainerAccount => applicationDataContainerAccount.Values)
                         .Where(propertySetAccount =>
                             !new[] { TagStatusAdding, TagStatusExpired, TagStatusReady, TagStatusUpdating }.Contains(
                                 propertySetAccount[KeyStatus])))
                propertySetAccount[KeyStatus] = TagStatusExpired; // TODO: check cookies to change to expired or ready.
        } // end method CheckAccounts

        /// <summary>
        /// Count the accounts added.
        /// </summary>
        /// <returns>The number of the accounts added.</returns>
        public int CountAccounts()
        {
            return ApplicationDataContainerAccounts.Containers.Count;
        } // end method CountAccounts

        /// <summary>
        /// Get the specific account's characters from the API.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Character>> GetCharactersFromApiAsync(string containerKeyAccount)
        {
            if (!ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return null;
            } // end if

            string headerValueUserAgent;
            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;
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
            var account = JsonSerializer.Deserialize<Characters>(httpResponseBody,
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
        /// Get the characters grouped by account from local.
        /// </summary>
        /// <returns>The characters grouped by account.</returns>
        public ObservableCollection<GroupInfoList> GetGroupedCharactersFromLocal()
        {
            if (CountAccounts() <= 0) return null;

            return new ObservableCollection<GroupInfoList>(
                from character in (from applicationDataContainerAccount in ApplicationDataContainerAccounts.Containers
                        .Values
                    where applicationDataContainerAccount.Containers.ContainsKey(ContainerKeyCharacters)
                    let applicationDataContainerCharacters =
                        applicationDataContainerAccount.Containers[ContainerKeyCharacters]
                    where applicationDataContainerCharacters.Containers.Count > 0
                    let propertySetAccount = applicationDataContainerAccount.Values
                    from keyValuePairCharacter in applicationDataContainerCharacters.Containers
                    let propertySetCharacter = keyValuePairCharacter.Value.Values
                    select new AccountCharacter()
                    {
                        AccountId = propertySetAccount[KeyId] as string,
                        Level = (int)propertySetCharacter[KeyLevel],
                        Nickname = propertySetCharacter[KeyNickname] as string,
                        Region = propertySetCharacter[KeyRegion] as string,
                        Server = propertySetAccount[KeyServer] switch
                        {
                            TagServerCn => _resourceLoader.GetString("ServerCn"),
                            TagServerGlobal => _resourceLoader.GetString("ServerGlobal"),
                            _ => _resourceLoader.GetString("Unknown")
                        },
                        UserId = keyValuePairCharacter.Key
                    }).ToList()
                group character by $"{character.Server} | {character.AccountId}"
                into accountGroup
                orderby accountGroup.Key
                select new GroupInfoList(accountGroup) { Key = accountGroup.Key });
        } // end method GetGroupedCharactersFromLocal

        /// <summary>
        /// TODO: Remove the specific account's characters from the main window's navigation view.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeysCharacter">A list of characters to remove for the navigation if possible. Do for all characters if <c>null</c>.</param>
        private void RemoveCharactersNavigation(string containerKeyAccount,
            ImmutableList<string> containerKeysCharacter = null)
        {
            var navigationViewBody = _app.WindowsH.GetMainWindow().NavigationViewBody;
            var navigationViewBodyMenuItems = navigationViewBody.MenuItems;

            foreach (var navigationViewBodyMenuItem in from NavigationViewItem navigationViewBodyMenuItem in
                         navigationViewBodyMenuItems
                             .Take(navigationViewBodyMenuItems.Count - 1).ToImmutableList()
                     let navigationViewBodyMenuItemTag = (KeyValuePair<string, string>)navigationViewBodyMenuItem.Tag
                     where navigationViewBodyMenuItemTag.Key == containerKeyAccount &&
                           (containerKeysCharacter == null ||
                            containerKeysCharacter.Contains(navigationViewBodyMenuItemTag.Value))
                     select navigationViewBodyMenuItem)
                navigationViewBodyMenuItems.Remove(navigationViewBodyMenuItem);

            navigationViewBody.SelectedItem ??= navigationViewBodyMenuItems.FirstOrDefault();
        } // end method RemoveAccountNavigation

        /// <summary>
        /// Add or update the specific account's characters, and do necessary operations on the relevant UI elements.
        /// </summary>
        /// <param name="characters">A list of characters.</param>
        /// <param name="containerKeyAccount">The account container key.</param>
        public void StoreCharacters(ImmutableList<Character> characters, string containerKeyAccount)
        {
            if (!ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return;
            } // end if

            var applicationDataContainerAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount];
            var propertySetAccount = applicationDataContainerAccount.Values;

            if (characters.Count == 0)
            {
                applicationDataContainerAccount.DeleteContainer(ContainerKeyCharacters);
                propertySetAccount[KeyStatus] = TagStatusReady;
                RemoveCharactersNavigation(containerKeyAccount);
                return;
            } // end if

            var applicationDataContainerCharacters =
                applicationDataContainerAccount.CreateContainer(ContainerKeyCharacters,
                    ApplicationDataCreateDisposition.Always);
            var containerKeysCharacter = new List<string>();

            foreach (var keyValuePairCharacter in applicationDataContainerCharacters.Containers.Where(
                         keyValuePairCharacter => !characters.Select(character => character.UserId).ToImmutableList()
                             .Contains(keyValuePairCharacter.Key)))
            {
                applicationDataContainerCharacters.DeleteContainer(keyValuePairCharacter.Key);
                containerKeysCharacter.Add(keyValuePairCharacter.Key);
            } // end foreach

            RemoveCharactersNavigation(containerKeyAccount, containerKeysCharacter.ToImmutableList());
            containerKeysCharacter.Clear();

            foreach (var character in characters)
            {
                var propertySetCharacter = applicationDataContainerCharacters
                    .CreateContainer(character.UserId, ApplicationDataCreateDisposition.Always).Values;

                if (!propertySetCharacter.ContainsKey(KeyIsEnabled)) propertySetCharacter[KeyIsEnabled] = true;

                propertySetCharacter[KeyLevel] = character.Level;
                propertySetCharacter[KeyNickname] = character.Nickname;
                propertySetCharacter[KeyRegion] = character.Region;
                containerKeysCharacter.Add(character.UserId);
            } // end foreach

            propertySetAccount[KeyStatus] = TagStatusReady;
            AddOrUpdateCharactersNavigation(containerKeyAccount, containerKeysCharacter.ToImmutableList());
        } // end method StoreCharacters

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers