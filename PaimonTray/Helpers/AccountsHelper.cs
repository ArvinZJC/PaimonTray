using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The accounts helper.
    /// </summary>
    public class AccountsHelper : INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// The accounts container key.
        /// </summary>
        private const string ContainerKeyAccounts = "accounts";

        /// <summary>
        /// The characters container key.
        /// </summary>
        private const string ContainerKeyCharacters = "characters";

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
        /// The cookie header name.
        /// </summary>
        private const string HeaderNameCookie = "Cookie";

        /// <summary>
        /// The Accept header value.
        /// </summary>
        private const string HeaderValueAccept = "application/json";

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
        /// The avatar key.
        /// </summary>
        private const string KeyAvatar = "avatar";

        /// <summary>
        /// The cookies key.
        /// </summary>
        public const string KeyCookies = "cookies";

        /// <summary>
        /// The data key.
        /// </summary>
        private const string KeyData = "data";

        /// <summary>
        /// The key of the flag indicating if the subject is enabled.
        /// </summary>
        private const string KeyIsEnabled = "isEnabled";

        /// <summary>
        /// The level key.
        /// </summary>
        private const string KeyLevel = "level";

        /// <summary>
        /// The list key.
        /// </summary>
        private const string KeyList = "list";

        /// <summary>
        /// The message key.
        /// </summary>
        private const string KeyMessage = "message";

        /// <summary>
        /// The nickname key.
        /// </summary>
        private const string KeyNickname = "nickname";

        /// <summary>
        /// The region key.
        /// </summary>
        private const string KeyRegion = "region";

        /// <summary>
        /// The region key for the CN server's Bilibili game server.
        /// </summary>
        private const string KeyRegionCnBilibili = "cn_qd01";

        /// <summary>
        /// The region key for the CN server's official game server.
        /// </summary>
        private const string KeyRegionCnOfficial = "cn_gf01";

        /// <summary>
        /// The region key for the global server's game server for America.
        /// </summary>
        private const string KeyRegionGlobalAmerica = "os_usa";

        /// <summary>
        /// The region key for the global server's game server for Asia.
        /// </summary>
        private const string KeyRegionGlobalAsia = "os_asia";

        /// <summary>
        /// The region key for the global server's game server for Europe.
        /// </summary>
        private const string KeyRegionGlobalEurope = "os_euro";

        /// <summary>
        /// The region key for the global server's game server for the special administrative regions (SARs).
        /// </summary>
        private const string KeyRegionGlobalSars = "os_cht";

        /// <summary>
        /// The return code key.
        /// </summary>
        public const string KeyReturnCode = "retcode";

        /// <summary>
        /// The server key.
        /// </summary>
        public const string KeyServer = "server";

        /// <summary>
        /// The status key.
        /// </summary>
        public const string KeyStatus = "status";

        /// <summary>
        /// The UID key.
        /// </summary>
        public const string KeyUid = "uid";

        /// <summary>
        /// The user info key.
        /// </summary>
        private const string KeyUserInfo = "user_info";

        /// <summary>
        /// The level prefix.
        /// </summary>
        private const string PrefixLevel = "Lv.";

        /// <summary>
        /// The property name for the flag indicating if the program is checking the accounts.
        /// </summary>
        public const string PropertyNameIsChecking = nameof(IsChecking);

        /// <summary>
        /// The login fail return code.
        /// </summary>
        private const int ReturnCodeLoginFail = -100;

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
        /// The fail status tag.
        /// </summary>
        public const string TagStatusFail = "fail";

        /// <summary>
        /// The ready status tag.
        /// </summary>
        public const string TagStatusReady = "ready";

        /// <summary>
        /// The updating status tag.
        /// </summary>
        public const string TagStatusUpdating = "updating";

        /// <summary>
        /// The URL for the CN server to get an account.
        /// </summary>
        private const string UrlAccountServerCn = "https://bbs-api.mihoyo.com/user/wapi/getUserFullInfo?gids=2";

        /// <summary>
        /// The URL for the global server to get an account.
        /// </summary>
        private const string UrlAccountServerGlobal = "https://bbs-api-os.hoyolab.com/community/painter/wapi/user/full";

        /// <summary>
        /// The base URL for the CN server to get an avatar.
        /// </summary>
        private const string UrlBaseAvatarServerCn = "https://img-static.mihoyo.com/avatar/avatar";

        /// <summary>
        /// The base URL for the global server to get an avatar.
        /// </summary>
        private const string UrlBaseAvatarServerGlobal = "https://img-os-static.hoyolab.com/avatar/avatar";

        /// <summary>
        /// The base URL for indicating the success of logging into miHoYo.
        /// </summary>
        public const string UrlBaseLoginEndMiHoYo = "https://bbs.mihoyo.com/ys/accountCenter/postList?";

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
        public const string UrlCookiesMiHoYo = "https://www.mihoyo.com";

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
        /// A flag indicating if the program is checking the accounts.
        /// </summary>
        private bool _isChecking;

        /// <summary>
        /// The HTTP client's lazy initialisation.
        /// </summary>
        private readonly Lazy<HttpClient>
            _lazyHttpClient; // System.Net.Http is used rather than the recommended Windows.Web.Http because of disabling automatic cookies handling, which the latter seems to perform badly.

        /// <summary>
        /// The regions dictionary.
        /// </summary>
        private readonly Dictionary<string, string> _regions;

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

        /// <summary>
        /// The grouped characters.
        /// </summary>
        public ObservableCollection<GroupInfoList> GroupedCharacters { get; }

        /// <summary>
        /// A flag indicating if the program is checking the accounts.
        /// </summary>
        public bool IsChecking
        {
            get => _isChecking;
            set
            {
                if (_isChecking == value) return;

                _isChecking = value;
                NotifyPropertyChanged();
            } // end set
        } // end property IsChecking

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the accounts helper.
        /// </summary>
        public AccountsHelper()
        {
            _app = Application.Current as App;
            _isChecking = false;
            _lazyHttpClient =
                new Lazy<HttpClient>(() => new HttpClient(new HttpClientHandler { UseCookies = false }));
            _resourceLoader = _app?.SettingsH.ResLoader;
            _regions = new Dictionary<string, string>
            {
                [KeyRegionCnBilibili] = _resourceLoader?.GetString("RegionCnBilibili"),
                [KeyRegionCnOfficial] = _resourceLoader?.GetString("RegionCnOfficial"),
                [KeyRegionGlobalAmerica] = _resourceLoader?.GetString("RegionGlobalAmerica"),
                [KeyRegionGlobalAsia] = _resourceLoader?.GetString("RegionGlobalAsia"),
                [KeyRegionGlobalEurope] = _resourceLoader?.GetString("RegionGlobalEurope"),
                [KeyRegionGlobalSars] = _resourceLoader?.GetString("RegionGlobalSars"),
            };
            ApplicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(ContainerKeyAccounts,
                    ApplicationDataCreateDisposition
                        .Always); // The container's containers are in a read-only dictionary, and should not be stored.
            GroupedCharacters = new ObservableCollection<GroupInfoList>();

            CheckAccountsAsync();
        } // end constructor AccountsHelper

        #endregion Constructors

        #region Events

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Methods

        /// <summary>
        /// TODO: Add the specific account's characters to the main window's navigation view, or update the specific navigation view items. ATTENTION: string null check
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
                    if (navigationViewItemCharacter is null)
                    {
                        navigationViewItemCharacter = new NavigationViewItem()
                        {
                            Icon = new SymbolIcon(Symbol.Contact),
                            Tag = new KeyValuePair<string, string>(containerKeyAccount, keyValuePairCharacter.Key)
                        };
                        navigationViewBodyMenuItems.Insert(navigationViewBodyMenuItems.Count - 1,
                            navigationViewItemCharacter);
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
        /// <param name="shouldForceCheck">A flag indicating if the check should be forced for all non-expired accounts.</param>
        private async void CheckAccountsAsync(bool shouldForceCheck = false)
        {
            IsChecking = true;

            // TODO: test purposes only.
            for (var i = 0; i < 3; i++)
            {
                var containerKeyAccount = i.ToString();
                var propertySetAccount = ApplicationDataContainerAccounts
                    .CreateContainer(containerKeyAccount, ApplicationDataCreateDisposition.Always).Values;
                var server = i % 2 == 0 ? TagServerCn : TagServerGlobal;

                propertySetAccount[KeyCookies] = $"test={containerKeyAccount}";
                propertySetAccount[KeyNickname] = "TEST_ACCOUNT";
                propertySetAccount[KeyServer] = server;
                propertySetAccount[KeyStatus] = TagStatusFail; // Should show as expired after checking
                propertySetAccount[KeyUid] = containerKeyAccount;

                var characters = new List<Character>();

                for (var j = 0; j < 3; j++)
                {
                    characters.Add(new Character()
                    {
                        Level = 55,
                        Nickname = "TEST_CHARACTER",
                        Region = server is TagServerCn ? KeyRegionCnBilibili : KeyRegionGlobalSars,
                        Uid = j.ToString()
                    });
                } // end for

                StoreCharacters(characters.ToImmutableList(), containerKeyAccount);
            } // end for

            if (CountAccounts() > 0)
            {
                foreach (var applicationDataContainerAccount in ApplicationDataContainerAccounts.Containers.Values)
                {
                    var propertySetAccount = applicationDataContainerAccount.Values;

                    if (propertySetAccount[KeyStatus] is not TagStatusAdding &&
                        propertySetAccount[KeyStatus] is not TagStatusExpired &&
                        propertySetAccount[KeyStatus] is not TagStatusFail &&
                        propertySetAccount[KeyStatus] is not TagStatusReady &&
                        propertySetAccount[KeyStatus] is not TagStatusUpdating)
                        propertySetAccount[KeyStatus] = TagStatusAdding;

                    switch (propertySetAccount[KeyStatus])
                    {
                        case TagStatusAdding:
                            if (propertySetAccount[KeyCookies] is null || propertySetAccount[KeyServer] is null ||
                                propertySetAccount[KeyUid] is null)
                            {
                                ApplicationDataContainerAccounts.DeleteContainer(applicationDataContainerAccount.Name);
                                continue;
                            } // end if

                            break;

                        case TagStatusExpired:
                            continue;

                        case TagStatusFail:
                            propertySetAccount[KeyStatus] = TagStatusUpdating;
                            break;

                        case TagStatusReady:
                            if (shouldForceCheck)
                            {
                                propertySetAccount[KeyStatus] = TagStatusUpdating;
                                break;
                            } // end if

                            continue;
                    } // end switch-case

                    StoreCharacters(await GetAccountCharactersFromApiAsync(applicationDataContainerAccount.Name),
                        applicationDataContainerAccount.Name);
                } // end foreach

                GetGroupedCharactersFromLocal();
            } // end if

            IsChecking = false;
        } // end method CheckAccountsAsync

        /// <summary>
        /// Count the accounts added.
        /// </summary>
        /// <returns>The number of the accounts added.</returns>
        public int CountAccounts()
        {
            return ApplicationDataContainerAccounts.Containers.Count;
        } // end method CountAccounts

        /// <summary>
        /// Get the account and its characters from the API.
        /// NOTE: Remember to change the account status to adding/updating.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Character>> GetAccountCharactersFromApiAsync(string containerKeyAccount)
        {
            if (await GetAccountFromApiAsync(containerKeyAccount))
                return await GetCharactersFromApiAsync(containerKeyAccount);

            return null;
        } // end method GetAccountCharactersFromApiAsync

        /// <summary>
        /// Get the account from the API. The method is usually used before getting the account's characters from the API.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A flag indicating if getting the account's characters from the API can be safe to execute.</returns>
        private async Task<bool> GetAccountFromApiAsync(string containerKeyAccount)
        {
            if (containerKeyAccount is null ||
                !ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return false;
            } // end if

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

            if (propertySetAccount[KeyStatus] is TagStatusExpired)
            {
                Log.Warning(
                    $"Cannot get the account from the API due to its expired status (account container key: {containerKeyAccount}).");
                return false;
            } // end if

            string headerValueUserAgent;
            string urlAccount;

            if (propertySetAccount[KeyServer] is TagServerCn)
            {
                headerValueUserAgent = HeaderValueUserAgentServerCn;
                urlAccount = UrlAccountServerCn;
            }
            else
            {
                headerValueUserAgent = HeaderValueUserAgentServerGlobal;
                urlAccount = UrlAccountServerGlobal;
            } // end if...else

            var httpClient = _lazyHttpClient.Value;
            var httpClientHeaders = httpClient.DefaultRequestHeaders;

            httpClientHeaders.Clear(); // Clear first.
            httpClientHeaders.Add(HeaderNameCookie, propertySetAccount[KeyCookies] as string);
            httpClientHeaders.Accept.TryParseAdd(HeaderValueAccept);
            httpClientHeaders.UserAgent.TryParseAdd(headerValueUserAgent);

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = await httpClient.GetAsync(new Uri(urlAccount));
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"The HTTP response to get an account was unsuccessful (account container key: {containerKeyAccount}).");
                Log.Error(exception.ToString());
                propertySetAccount[KeyStatus] = TagStatusFail;
                return true;
            } // end try...catch

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning($"Failed to parse the response's body (account container key: {containerKeyAccount}):");
                    Log.Information(httpResponseBody);
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return true;
                } // end if

                var returnCode = (int)jsonNodeResponse[KeyReturnCode];

                if (returnCode != 0)
                {
                    Log.Warning(
                        $"Failed to get an account from the specific API (account container key: {containerKeyAccount}, message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}).");
                    propertySetAccount[KeyStatus] =
                        returnCode is ReturnCodeLoginFail ? TagStatusExpired : TagStatusFail;
                    return true;
                } // end if

                var userInfo = jsonNodeResponse[KeyData]?[KeyUserInfo];

                if (userInfo is null)
                {
                    Log.Warning($"Failed to get the user info (account container key: {containerKeyAccount}).");
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return true;
                } // end if

                var uid = (string)userInfo[KeyUid];

                if (uid != propertySetAccount[KeyUid] as string)
                {
                    Log.Warning(
                        $"The account UID does not match (account container key: {containerKeyAccount}, UID got: {uid}).");
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return false;
                } // end if

                var avatar = (string)userInfo[KeyAvatar];

                if (avatar is null)
                {
                    Log.Warning(
                        $"Failed to get the avatar from the user info (account container key: {containerKeyAccount}).");
                    propertySetAccount[KeyStatus] = TagStatusFail;
                }
                else propertySetAccount[KeyAvatar] = avatar;

                var nickname = (string)userInfo[KeyNickname];

                if (nickname is null)
                {
                    Log.Warning(
                        $"Failed to get the nickname from the user info (account container key: {containerKeyAccount}).");
                    propertySetAccount[KeyStatus] = TagStatusFail;
                }
                else propertySetAccount[KeyNickname] = nickname;
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to parse the response's body (account container key: {containerKeyAccount}):");
                Log.Information(httpResponseBody);
                Log.Error(exception.ToString());
                propertySetAccount[KeyStatus] = TagStatusFail;
            } // end try...catch

            return true;
        } // end method GetAccountFromApiAsync

        /// <summary>
        /// Get the avatar URI.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>The avatar URI, or <c>null</c> if no such account container key.</returns>
        public Uri GetAvatarUri(string containerKeyAccount)
        {
            if (containerKeyAccount is null ||
                !ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return null;
            } // end if

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;
            var urlBaseAvatar = propertySetAccount[KeyServer] is TagServerCn
                ? UrlBaseAvatarServerCn
                : UrlBaseAvatarServerGlobal;

            return new Uri($"{urlBaseAvatar}{propertySetAccount[KeyAvatar]}.png");
        } // end method GetAvatarUri

        /// <summary>
        /// Get the specific account's characters from the API.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        private async Task<ImmutableList<Character>> GetCharactersFromApiAsync(string containerKeyAccount)
        {
            if (containerKeyAccount is null ||
                !ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return null;
            } // end if

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

            if (propertySetAccount[KeyStatus] is TagStatusExpired)
            {
                Log.Warning(
                    $"Cannot get characters from the API due to the specific account's expired status (account container key: {containerKeyAccount}).");
                return null;
            } // end if

            string headerValueUserAgent;
            string urlCharacters;

            if (propertySetAccount[KeyServer] is TagServerCn)
            {
                headerValueUserAgent = HeaderValueUserAgentServerCn;
                urlCharacters = UrlCharactersServerCn;
            }
            else
            {
                headerValueUserAgent = HeaderValueUserAgentServerGlobal;
                urlCharacters = UrlCharactersServerGlobal;
            } // end if...else

            var httpClient = _lazyHttpClient.Value;
            var httpClientHeaders = httpClient.DefaultRequestHeaders;

            httpClientHeaders.Clear(); // Clear first.
            httpClientHeaders.Add(HeaderNameCookie, propertySetAccount[KeyCookies] as string);
            httpClientHeaders.Accept.TryParseAdd(HeaderValueAccept);
            httpClientHeaders.UserAgent.TryParseAdd(headerValueUserAgent);

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = await httpClient.GetAsync(new Uri(urlCharacters));
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"The HTTP response to get characters was unsuccessful (account container key: {containerKeyAccount}).");
                Log.Error(exception.ToString());
                propertySetAccount[KeyStatus] = TagStatusFail;
                return null;
            } // end try...catch

            var httpResponseBody = await httpResponseMessage.Content.ReadAsStringAsync();

            try
            {
                var charactersRaw = JsonSerializer.Deserialize<CharactersResponse>(httpResponseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (charactersRaw is null)
                {
                    Log.Warning($"Failed to parse the response's body (account container key: {containerKeyAccount}):");
                    Log.Information(httpResponseBody);
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return null;
                } // end if

                if (charactersRaw.ReturnCode != 0)
                {
                    Log.Warning(
                        $"Failed to get characters from the specific API (account container key: {containerKeyAccount}, message: {charactersRaw.Message}, return code: {charactersRaw.ReturnCode}).");
                    propertySetAccount[KeyStatus] =
                        charactersRaw.ReturnCode is ReturnCodeLoginFail ? TagStatusExpired : TagStatusFail;
                    return null;
                } // end if

                if (charactersRaw.Data.TryGetValue(KeyList, out var characters)) return characters;

                Log.Warning($"Failed to get the character data list (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = TagStatusFail;
                return null;
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to parse the response's body (account container key: {containerKeyAccount}):");
                Log.Information(httpResponseBody);
                Log.Error(exception.ToString());
                propertySetAccount[KeyStatus] = TagStatusFail;
                return null;
            } // end try...catch
        } // end method GetCharactersFromApiAsync

        /// <summary>
        /// Get the characters grouped by account from local.
        /// </summary>
        private void GetGroupedCharactersFromLocal()
        {
            GroupedCharacters.Clear();

            if (CountAccounts() <= 0) return;

            var accountCharacters = new List<AccountCharacter>();

            foreach (var applicationDataContainerAccount in ApplicationDataContainerAccounts.Containers.Values)
            {
                var applicationDataContainersCharacter = applicationDataContainerAccount
                    .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers;
                var propertySetAccount = applicationDataContainerAccount.Values; // Get the account property set first.
                var aNickname = propertySetAccount[KeyNickname] as string;
                var aUid = propertySetAccount[KeyUid] as string;
                var server = propertySetAccount[KeyServer] switch
                {
                    TagServerCn => _resourceLoader.GetString("ServerCn"),
                    TagServerGlobal => _resourceLoader.GetString("ServerGlobal"),
                    _ => AppConstantsHelper.Unknown
                };
                var status = propertySetAccount[KeyStatus] as string;

                if (applicationDataContainersCharacter.Count == 0)
                {
                    accountCharacters.Add(new AccountCharacter
                    {
                        ANickname = aNickname,
                        AUid = aUid,
                        Key = applicationDataContainerAccount.Name,
                        Server = server,
                        Status = status
                    });
                    continue;
                } // end if

                accountCharacters.AddRange(from keyValuePairCharacter in applicationDataContainersCharacter
                    let propertySetCharacter = keyValuePairCharacter.Value.Values
                    select new AccountCharacter
                    {
                        ANickname = aNickname,
                        AUid = aUid,
                        CNickname = propertySetCharacter[KeyNickname] as string,
                        CUid = keyValuePairCharacter.Key,
                        IsEnabled = (bool)propertySetCharacter[KeyIsEnabled],
                        Key = applicationDataContainerAccount.Name,
                        Level = $"{PrefixLevel}{propertySetCharacter[KeyLevel]}",
                        Region = GetRegion(propertySetCharacter[KeyRegion] as string),
                        Server = server,
                        Status = status
                    });
            } // end foreach

            // TODO: Ordered by datetime? Need ToList to allow modification?
            (from accountCharacter in accountCharacters.ToImmutableList()
                    group accountCharacter by JsonSerializer.Serialize(accountCharacter)
                    into accountGroup
                    orderby accountGroup.Key
                    select new GroupInfoList(accountGroup) { Key = accountGroup.Key }).ToImmutableList()
                .ForEach(GroupedCharacters.Add);
        } // end method GetGroupedCharactersFromLocal

        /// <summary>
        /// Get the region.
        /// </summary>
        /// <param name="keyRegion">The region key.</param>
        /// <returns>The region.</returns>
        private string GetRegion(string keyRegion)
        {
            return keyRegion is null || !_regions.TryGetValue(keyRegion, out var region)
                ? AppConstantsHelper.Unknown
                : region;
        } // end method GetRegion

        /// <summary>
        /// Notify the property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property for the event.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // end method NotifyPropertyChanged

        /// <summary>
        /// TODO: Remove the specific account's characters from the main window's navigation view. ATTENTION: string null check
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
                           (containerKeysCharacter is null ||
                            containerKeysCharacter.Contains(navigationViewBodyMenuItemTag.Value))
                     select navigationViewBodyMenuItem)
                navigationViewBodyMenuItems.Remove(navigationViewBodyMenuItem);

            navigationViewBody.SelectedItem ??= navigationViewBodyMenuItems.FirstOrDefault();
        } // end method RemoveAccountNavigation

        /// <summary>
        /// TODO: Add or update the specific account's characters, and do necessary operations on the relevant UI elements.
        /// </summary>
        /// <param name="characters">A list of characters.</param>
        /// <param name="containerKeyAccount">The account container key.</param>
        public void StoreCharacters(ImmutableList<Character> characters, string containerKeyAccount)
        {
            if (containerKeyAccount is null ||
                !ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount))
            {
                Log.Warning($"No such account container key ({containerKeyAccount}).");
                return;
            } // end if

            var applicationDataContainerAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount];
            var applicationDataContainerCharacters =
                applicationDataContainerAccount.CreateContainer(ContainerKeyCharacters,
                    ApplicationDataCreateDisposition.Always);
            var propertySetAccount = applicationDataContainerAccount.Values;

            if (characters is null)
            {
                Log.Warning($"Cannot store null characters (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = propertySetAccount[KeyStatus] is TagStatusExpired
                    ? TagStatusExpired
                    : TagStatusFail;
                return;
            } // end if

            if (characters.Count == 0)
            {
                foreach (var containerKeyCharacter in applicationDataContainerCharacters.Containers.Keys)
                    applicationDataContainerCharacters.DeleteContainer(containerKeyCharacter);

                if (propertySetAccount[KeyStatus] is TagStatusAdding or TagStatusUpdating)
                    propertySetAccount[KeyStatus] = TagStatusReady;

                // RemoveCharactersNavigation(containerKeyAccount);
                return;
            } // end if

            // var containerKeysCharacter = new List<string>();

            foreach (var containerKeyCharacter in applicationDataContainerCharacters.Containers.Keys.ToImmutableList()
                         .Where(containerKeyCharacter => !characters.Select(character => character.Uid)
                             .ToImmutableList().Contains(containerKeyCharacter)))
            {
                applicationDataContainerCharacters.DeleteContainer(containerKeyCharacter);
                // containerKeysCharacter.Add(keyValuePairCharacter.Key);
            } // end foreach

            // RemoveCharactersNavigation(containerKeyAccount, containerKeysCharacter.ToImmutableList());
            // containerKeysCharacter.Clear();

            foreach (var character in characters)
            {
                var propertySetCharacter = applicationDataContainerCharacters
                    .CreateContainer(character.Uid, ApplicationDataCreateDisposition.Always).Values;

                if (!propertySetCharacter.ContainsKey(KeyIsEnabled)) propertySetCharacter[KeyIsEnabled] = true;

                propertySetCharacter[KeyLevel] = character.Level;
                propertySetCharacter[KeyNickname] = character.Nickname;
                propertySetCharacter[KeyRegion] = character.Region;
                // containerKeysCharacter.Add(character.Uid);
            } // end foreach

            if (propertySetAccount[KeyStatus] is TagStatusAdding or TagStatusUpdating)
                propertySetAccount[KeyStatus] = TagStatusReady;

            // AddOrUpdateCharactersNavigation(containerKeyAccount, containerKeysCharacter.ToImmutableList());
        } // end method StoreCharacters

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers