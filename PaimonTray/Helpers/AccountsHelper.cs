using Microsoft.UI.Xaml;
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
        private const string HeaderValueAppVersionServerCn = "2.29.1";

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
        /// The selected character's UID key.
        /// </summary>
        public const string KeyUidCharacterSelected = "uidCharacterSelected";

        /// <summary>
        /// The user info key.
        /// </summary>
        private const string KeyUserInfo = "user_info";

        /// <summary>
        /// The level prefix.
        /// </summary>
        private const string PrefixLevel = "Lv.";

        /// <summary>
        /// The property name for the flag indicating if the program has updated an account's character.
        /// </summary>
        public const string PropertyNameHasUpdatedAccountCharacter = nameof(HasUpdatedAccountCharacter);

        /// <summary>
        /// The property name for the flag indicating if the program has updated an account group.
        /// </summary>
        public const string PropertyNameHasUpdatedAccountGroup = nameof(HasUpdatedAccountGroup);

        /// <summary>
        /// The property name for the flag indicating if the program is adding/updating an account.
        /// </summary>
        public const string PropertyNameIsAddingUpdating = nameof(IsAddingUpdating);

        /// <summary>
        /// The property name for the flag indicating if the program is managing the accounts.
        /// </summary>
        public const string PropertyNameIsManaging = nameof(IsManaging);

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
        /// A flag indicating if the program has updated an account's character.
        /// </summary>
        private bool _hasUpdatedAccountCharacter;

        /// <summary>
        /// A flag indicating if the program has updated an account group.
        /// </summary>
        private bool _hasUpdatedAccountGroup;

        /// <summary>
        /// A flag indicating if the program is adding/updating an account.
        /// </summary>
        private bool _isAddingUpdating;

        /// <summary>
        /// A flag indicating if the program is managing the accounts.
        /// </summary>
        private bool _isManaging;

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
        /// The account group info lists.
        /// </summary>
        public ObservableCollection<GroupInfoList> AccountGroupInfoLists { get; }

        /// <summary>
        /// A flag indicating if the program has updated an account's character.
        /// </summary>
        public bool HasUpdatedAccountCharacter
        {
            get => _hasUpdatedAccountCharacter;
            set
            {
                if (_hasUpdatedAccountCharacter == value) return;

                _hasUpdatedAccountCharacter = value;
                OnPropertyChanged();
            } // end set
        } // end property HasUpdatedAccountCharacter

        /// <summary>
        /// A flag indicating if the program has updated an account group.
        /// </summary>
        public bool HasUpdatedAccountGroup
        {
            get => _hasUpdatedAccountGroup;
            set
            {
                if (_hasUpdatedAccountGroup == value) return;

                _hasUpdatedAccountGroup = value;
                OnPropertyChanged();
            } // end set
        } // end property HasUpdatedAccountGroup

        /// <summary>
        /// A flag indicating if the program is adding/updating an account.
        /// </summary>
        public bool IsAddingUpdating
        {
            get => _isAddingUpdating;
            set
            {
                if (_isAddingUpdating == value) return;

                _isAddingUpdating = value;
                OnPropertyChanged();
            } // end set
        } // end property IsAddingUpdating

        /// <summary>
        /// A flag indicating if the program is managing the accounts.
        /// </summary>
        public bool IsManaging
        {
            get => _isManaging;
            set
            {
                if (_isManaging == value) return;

                _isManaging = value;
                OnPropertyChanged();
            } // end set
        } // end property IsManaging

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the accounts helper.
        /// </summary>
        public AccountsHelper()
        {
            var app = Application.Current as App;

            _hasUpdatedAccountCharacter = false;
            _hasUpdatedAccountGroup = false;
            _isAddingUpdating = false;
            _isManaging = false;
            _lazyHttpClient =
                new Lazy<HttpClient>(() => new HttpClient(new HttpClientHandler { UseCookies = false }));
            _resourceLoader = app?.SettingsH.ResLoader;
            _regions = new Dictionary<string, string>
            {
                [KeyRegionCnBilibili] = _resourceLoader?.GetString("RegionCnBilibili"),
                [KeyRegionCnOfficial] = _resourceLoader?.GetString("RegionCnOfficial"),
                [KeyRegionGlobalAmerica] = _resourceLoader?.GetString("RegionGlobalAmerica"),
                [KeyRegionGlobalAsia] = _resourceLoader?.GetString("RegionGlobalAsia"),
                [KeyRegionGlobalEurope] = _resourceLoader?.GetString("RegionGlobalEurope"),
                [KeyRegionGlobalSars] = _resourceLoader?.GetString("RegionGlobalSars"),
            };
            AccountGroupInfoLists = new ObservableCollection<GroupInfoList>();
            ApplicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(ContainerKeyAccounts,
                    ApplicationDataCreateDisposition
                        .Always); // The container's containers are in a read-only dictionary, and should not be stored.
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
        /// Add/Update an account group.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        private void AddUpdateAccountGroup(string containerKeyAccount)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            var accountCharacters = new List<AccountCharacter>();
            var applicationDataContainerAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount];
            var applicationDataContainersCharacter = applicationDataContainerAccount
                .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers;
            var propertySetAccount = applicationDataContainerAccount.Values; // Get the account property set first.
            var nicknameAccount = propertySetAccount[KeyNickname] as string;
            var server = propertySetAccount[KeyServer] switch
            {
                TagServerCn => _resourceLoader.GetString("ServerCn"),
                TagServerGlobal => _resourceLoader.GetString("ServerGlobal"),
                _ => AppConstantsHelper.Unknown
            };
            var status = propertySetAccount[KeyStatus] as string;
            var uidAccount = propertySetAccount[KeyUid] as string;

            if (applicationDataContainersCharacter.Count > 0)
                accountCharacters.AddRange(from keyValuePairCharacter in applicationDataContainersCharacter
                    orderby keyValuePairCharacter.Key // Order by the character's UID.
                    let propertySetCharacter = keyValuePairCharacter.Value.Values
                    select new AccountCharacter
                    {
                        IsEnabled = (bool)propertySetCharacter[KeyIsEnabled],
                        Key = containerKeyAccount,
                        Level = $"{PrefixLevel}{propertySetCharacter[KeyLevel]}",
                        NicknameAccount = nicknameAccount,
                        NicknameCharacter = propertySetCharacter[KeyNickname] as string,
                        Region = GetRegion(propertySetCharacter[KeyRegion] as string),
                        Server = server,
                        Status = status,
                        UidAccount = uidAccount,
                        UidCharacter = keyValuePairCharacter.Key
                    });
            else
                accountCharacters.Add(new AccountCharacter
                {
                    Key = containerKeyAccount,
                    NicknameAccount = nicknameAccount,
                    Server = server,
                    Status = status,
                    UidAccount = uidAccount
                });

            var accountGroupInfoListTarget = AccountGroupInfoLists.ToImmutableList()
                .FirstOrDefault(accountGroupInfoList => accountGroupInfoList.Key == containerKeyAccount, null);

            if (accountGroupInfoListTarget is null)
                (from accountCharacter in accountCharacters
                        group accountCharacter by accountCharacter.Key
                        into accountGroup
                        orderby accountGroup.Key
                        select new GroupInfoList(accountGroup) { Key = accountGroup.Key }).ToImmutableList()
                    .ForEach(AccountGroupInfoLists.Add);
            else
            {
                HasUpdatedAccountGroup = false;
                accountGroupInfoListTarget.Clear();
                accountGroupInfoListTarget.AddRange(accountCharacters);
                HasUpdatedAccountGroup = true;
            } // end if...else
        } // end method AddUpdateAccountGroup

        /// <summary>
        /// Add/Update the specific account's characters.
        /// NOTE: If the account container key is valid and the characters list is not <c>null</c>, the relevant account group will also be added/updated.
        /// </summary>
        /// <param name="characters">A list of characters.</param>
        /// <param name="containerKeyAccount">The account container key.</param>
        public void AddUpdateCharacters(ImmutableList<Character> characters,
            string containerKeyAccount)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

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
            }
            else
            {
                foreach (var containerKeyCharacter in applicationDataContainerCharacters.Containers.Keys
                             .ToImmutableList()
                             .Where(containerKeyCharacter => !characters.Select(character => character.Uid)
                                 .ToImmutableList().Contains(containerKeyCharacter)))
                    applicationDataContainerCharacters.DeleteContainer(containerKeyCharacter);

                foreach (var character in characters)
                {
                    var propertySetCharacter = applicationDataContainerCharacters
                        .CreateContainer(character.Uid, ApplicationDataCreateDisposition.Always).Values;

                    if (!propertySetCharacter.ContainsKey(KeyIsEnabled)) propertySetCharacter[KeyIsEnabled] = true;

                    propertySetCharacter[KeyLevel] = character.Level;
                    propertySetCharacter[KeyNickname] = character.Nickname;
                    propertySetCharacter[KeyRegion] = character.Region;
                } // end foreach

                if (propertySetAccount[KeyStatus] is TagStatusAdding or TagStatusUpdating)
                    propertySetAccount[KeyStatus] = TagStatusReady;
            } // end if...else

            AddUpdateAccountGroup(containerKeyAccount);
        } // end method AddUpdateCharacters

        /// <summary>
        /// Apply the specific account's character's status.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <param name="shouldEnableCharacter">A flag indicating if the specific account's character should be enabled.</param>
        public void ApplyCharacterStatus(string containerKeyAccount, string containerKeyCharacter,
            bool shouldEnableCharacter)
        {
            if (!TryChangeCharacterStatus(containerKeyAccount, containerKeyCharacter, shouldEnableCharacter)) return;

            var accountGroupInfoListTarget = AccountGroupInfoLists.ToImmutableList()
                .FirstOrDefault(accountGroupInfoList => accountGroupInfoList.Key == containerKeyAccount, null);

            var accountCharacterTarget = accountGroupInfoListTarget?.Cast<AccountCharacter>()
                .FirstOrDefault(accountCharacter => accountCharacter.UidCharacter == containerKeyCharacter, null);

            if (accountCharacterTarget is null) return;

            var accountCharacterTargetIndex = accountGroupInfoListTarget.IndexOf(accountCharacterTarget);

            HasUpdatedAccountCharacter = false;
            accountCharacterTarget.IsEnabled = shouldEnableCharacter;
            accountGroupInfoListTarget.Insert(accountCharacterTargetIndex, accountCharacterTarget);
            accountGroupInfoListTarget.RemoveAt(accountCharacterTargetIndex + 1);
            HasUpdatedAccountCharacter = true;

            CheckSelectedCharacterUid();
        } // end method ApplyCharacterStatus

        /// <summary>
        /// Check the account.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="isStandalone">A flag indicating if the operation is standalone.</param>
        /// <param name="shouldForceCheck">A flag indicating if the check should be forced for all non-expired accounts.</param>
        /// <returns>A task just to indicate that any later operation needs to wait.</returns>
        public async Task CheckAccountAsync(string containerKeyAccount, bool isStandalone = false,
            bool shouldForceCheck = false)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            if (isStandalone) IsManaging = true;

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

            // TODO: test purposes only.
            if (propertySetAccount[KeyUid] is "0" or "1" or "2")
            {
                if (!isStandalone) return;

                CheckSelectedCharacterUid();
                IsManaging = false;
                return;
            } // end if

            if (propertySetAccount[KeyStatus] is not TagStatusAdding &&
                propertySetAccount[KeyStatus] is not TagStatusExpired &&
                propertySetAccount[KeyStatus] is not TagStatusFail &&
                propertySetAccount[KeyStatus] is not TagStatusReady &&
                propertySetAccount[KeyStatus] is not TagStatusUpdating)
                propertySetAccount[KeyStatus] = TagStatusAdding;

            var shouldAddUpdateCharacters = true;

            switch (propertySetAccount[KeyStatus])
            {
                case TagStatusAdding:
                    if (propertySetAccount[KeyCookies] is null || propertySetAccount[KeyServer] is null ||
                        propertySetAccount[KeyUid] is null)
                    {
                        ApplicationDataContainerAccounts.DeleteContainer(containerKeyAccount);
                        shouldAddUpdateCharacters = false;
                    } // end if

                    break;

                case TagStatusExpired:
                    shouldAddUpdateCharacters = false;
                    break;

                case TagStatusFail:
                    propertySetAccount[KeyStatus] = TagStatusUpdating;
                    break;

                case TagStatusReady:
                    if (shouldForceCheck) propertySetAccount[KeyStatus] = TagStatusUpdating;
                    else
                    {
                        AddUpdateAccountGroup(containerKeyAccount);
                        shouldAddUpdateCharacters = false;
                    } // end if...else

                    break;
            } // end switch-case

            if (shouldAddUpdateCharacters)
                AddUpdateCharacters(await GetAccountCharactersFromApiAsync(containerKeyAccount), containerKeyAccount);

            if (!isStandalone) return;

            CheckSelectedCharacterUid();
            IsManaging = false;
        } // end method CheckAccountAsync

        /// <summary>
        /// Check the accounts.
        /// </summary>
        /// <param name="shouldForceCheck">A flag indicating if the check should be forced for all non-expired accounts.</param>
        public async void CheckAccountsAsync(bool shouldForceCheck = false)
        {
            IsManaging = true;

            // TODO: test purposes only.
            for (var i = 0; i < 3; i++)
            {
                var server = i % 2 == 0 ? TagServerCn : TagServerGlobal;
                var uidAccount = i.ToString();
                var containerKeyAccount = $"{server}{uidAccount}";
                var applicationDataContainerAccount = ApplicationDataContainerAccounts
                    .CreateContainer(containerKeyAccount, ApplicationDataCreateDisposition.Always);
                var propertySetAccount = applicationDataContainerAccount.Values;

                propertySetAccount[KeyCookies] = $"test={uidAccount}";
                propertySetAccount[KeyNickname] = "TEST_ACCOUNT";
                propertySetAccount[KeyServer] = server;
                propertySetAccount[KeyStatus] = TagStatusFail;
                propertySetAccount[KeyUid] = uidAccount;

                var characters = new List<Character>();

                for (var j = 0; j < 3; j++)
                {
                    characters.Add(new Character()
                    {
                        Level = 55,
                        Nickname = "TEST_CHARACTER",
                        Region = server is TagServerCn ? KeyRegionCnBilibili : KeyRegionGlobalSars,
                        Uid = (i * 3 + j).ToString()
                    });
                } // end for

                applicationDataContainerAccount.DeleteContainer(ContainerKeyCharacters);
                AddUpdateCharacters(characters.ToImmutableList(), containerKeyAccount);
            } // end for

            if (CountAccounts() > 0)
                foreach (var containerKeyAccount in ApplicationDataContainerAccounts.Containers.Keys)
                    await CheckAccountAsync(containerKeyAccount, shouldForceCheck: shouldForceCheck);

            CheckSelectedCharacterUid();
            IsManaging = false;
        } // end method CheckAccountsAsync

        /// <summary>
        /// Check if the selected character's UID can be found in the account group info lists.
        /// NOTE: Should be invoked after finishing modifying account groups.
        /// </summary>
        private void CheckSelectedCharacterUid()
        {
            var propertySetAccounts = ApplicationDataContainerAccounts.Values;

            if (AccountGroupInfoLists.Count > 0)
            {
                if (propertySetAccounts[KeyUidCharacterSelected] is null) return;

                var canFindCharacterSelected = false;
                var uidCharacterSelected = propertySetAccounts[KeyUidCharacterSelected] as string;

                foreach (var accountCharacters in AccountGroupInfoLists.Select(accountGroupInfoList =>
                             accountGroupInfoList.Cast<AccountCharacter>()))
                {
                    canFindCharacterSelected = accountCharacters.Any(accountCharacter =>
                        accountCharacter.UidCharacter == uidCharacterSelected);

                    if (canFindCharacterSelected) break;
                } // end foreach

                if (!canFindCharacterSelected) propertySetAccounts[KeyUidCharacterSelected] = null;
            }
            else propertySetAccounts[KeyUidCharacterSelected] = null;
        } // end method CheckSelectedCharacterUid

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
            if (!ValidateAccountContainerKey(containerKeyAccount)) return false;

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

            if (propertySetAccount[KeyStatus] is TagStatusExpired)
            {
                Log.Warning(
                    $"Cannot get the account from the API due to its expired status (account container key: {containerKeyAccount}).");
                return false;
            } // end if

            var httpClient = _lazyHttpClient.Value;
            var httpClientHeaders = httpClient.DefaultRequestHeaders;
            var isServerCn = propertySetAccount[KeyServer] is TagServerCn;

            httpClientHeaders.Clear(); // Clear first.
            httpClientHeaders.Add(HeaderNameCookie, propertySetAccount[KeyCookies] as string);
            httpClientHeaders.Accept.TryParseAdd(HeaderValueAccept);
            httpClientHeaders.UserAgent.TryParseAdd(isServerCn
                ? HeaderValueUserAgentServerCn
                : HeaderValueUserAgentServerGlobal);

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage =
                    await httpClient.GetAsync(new Uri(isServerCn ? UrlAccountServerCn : UrlAccountServerGlobal));
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
            if (!ValidateAccountContainerKey(containerKeyAccount)) return null;

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
            if (!ValidateAccountContainerKey(containerKeyAccount)) return null;

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

            if (propertySetAccount[KeyStatus] is TagStatusExpired)
            {
                Log.Warning(
                    $"Cannot get characters from the API due to the specific account's expired status (account container key: {containerKeyAccount}).");
                return null;
            } // end if

            var httpClient = _lazyHttpClient.Value;
            var httpClientHeaders = httpClient.DefaultRequestHeaders;
            var isServerCn = propertySetAccount[KeyServer] is TagServerCn;

            httpClientHeaders.Clear(); // Clear first.
            httpClientHeaders.Add(HeaderNameCookie, propertySetAccount[KeyCookies] as string);
            httpClientHeaders.Accept.TryParseAdd(HeaderValueAccept);
            httpClientHeaders.UserAgent.TryParseAdd(isServerCn
                ? HeaderValueUserAgentServerCn
                : HeaderValueUserAgentServerGlobal);

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage =
                    await httpClient.GetAsync(new Uri(isServerCn ? UrlCharactersServerCn : UrlCharactersServerGlobal));
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
        /// Occur when the specific property is changed.
        /// </summary>
        /// <param name="propertyName">The name of the property for the event.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } // end method OnPropertyChanged

        /// <summary>
        /// Remove an account and the relevant account group.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        public void RemoveAccount(string containerKeyAccount)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            IsManaging = true;
            ApplicationDataContainerAccounts
                .DeleteContainer(containerKeyAccount); // Delete the account container first.
            AccountGroupInfoLists.Remove(AccountGroupInfoLists.ToImmutableList()
                .FirstOrDefault(accountGroupInfoList => accountGroupInfoList.Key == containerKeyAccount, null));
            CheckSelectedCharacterUid();
            IsManaging = false;
        } // end method RemoveAccount

        /// <summary>
        /// Remove the accounts and the account groups.
        /// </summary>
        public void RemoveAccounts()
        {
            IsManaging = true;

            foreach (var containerKeyAccount in ApplicationDataContainerAccounts.Containers.Keys.ToImmutableList())
                ApplicationDataContainerAccounts.DeleteContainer(containerKeyAccount);

            AccountGroupInfoLists.Clear();
            CheckSelectedCharacterUid();
            IsManaging = false;
        } // end method RemoveAccounts

        /// <summary>
        /// Try changing the specific account's character's status.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <param name="shouldEnableCharacter">A flag indicating if the specific account's character should be enabled.</param>
        /// <returns>A flag indicating if the specific account's character's status is changed.</returns>
        private bool TryChangeCharacterStatus(string containerKeyAccount, string containerKeyCharacter,
            bool shouldEnableCharacter)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return false;

            if (containerKeyCharacter is null) return false;

            var characters = ApplicationDataContainerAccounts.Containers[containerKeyAccount]
                .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers;

            var (_, applicationDataContainerCharacter) = characters.FirstOrDefault(
                keyValuePairCharacter => keyValuePairCharacter.Key == containerKeyCharacter,
                new KeyValuePair<string, ApplicationDataContainer>(containerKeyCharacter, null));

            if (applicationDataContainerCharacter is null)
            {
                Log.Warning(
                    $"No such character container key (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                return false;
            } // end if

            var propertySetCharacter = applicationDataContainerCharacter.Values;

            if ((bool)propertySetCharacter[KeyIsEnabled] == shouldEnableCharacter) return false;

            propertySetCharacter[KeyIsEnabled] = shouldEnableCharacter;
            return true;
        } // end method TryChangeCharacterStatus

        /// <summary>
        /// Try selecting the specific account group's 1st enabled character.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A flag indicating if the specific account group's 1st enabled character exists.</returns>
        public bool TrySelectAccountGroupFirstEnabledCharacter(string containerKeyAccount)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return false;

            if (AccountGroupInfoLists.Count <= 0) return false;

            var accountGroup =
                AccountGroupInfoLists.FirstOrDefault(
                    accountGroupInfoList => accountGroupInfoList.Key == containerKeyAccount, null);

            if (accountGroup is null || accountGroup.Count <= 0) return false;

            var accountCharacterSelected = accountGroup.Cast<AccountCharacter>()
                .FirstOrDefault(
                    accountCharacter => accountCharacter.IsEnabled && accountCharacter.UidCharacter is not null, null);

            if (accountCharacterSelected is null) return false;

            ApplicationDataContainerAccounts.Values[KeyUidCharacterSelected] = accountCharacterSelected.UidCharacter;
            return true;
        } // end method TrySelectAccountGroupFirstEnabledCharacter

        /// <summary>
        /// Validate the account container key.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A flag indicating if the account container key is valid.</returns>
        private bool ValidateAccountContainerKey(string containerKeyAccount)
        {
            if (containerKeyAccount is not null &&
                ApplicationDataContainerAccounts.Containers.ContainsKey(containerKeyAccount)) return true;

            Log.Warning($"No such account container key ({containerKeyAccount}).");
            return false;
        } // end method ValidateAccountContainerKey

        #endregion Methods
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers