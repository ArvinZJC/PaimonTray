using Microsoft.UI.Xaml;
using PaimonTray.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        /// The expeditions container key.
        /// </summary>
        private const string ContainerKeyExpeditions = "expeditions";

        /// <summary>
        /// The real-time notes container key.
        /// </summary>
        private const string ContainerKeyRealTimeNotes = "realTimeNotes";

        /// <summary>
        /// The token cookie key.
        /// </summary>
        public const string CookieKeyToken = "ltoken";

        /// <summary>
        /// The UID cookie key.
        /// </summary>
        public const string CookieKeyUid = "ltuid";

        /// <summary>
        /// The max number of accounts.
        /// </summary>
        public const int CountAccountsMax = 5;

        /// <summary>
        /// The finished expedition status in JSON data.
        /// </summary>
        public const string ExpeditionStatusFinished = "Finished";

        /// <summary>
        /// The ongoing expedition status in JSON data.
        /// </summary>
        public const string ExpeditionStatusOngoing = "Ongoing";

        /// <summary>
        /// The PNG file extension.
        /// </summary>
        private const string FileExtensionPng = ".png";

        /// <summary>
        /// The key of the flag indicating if the daily commissions' bonus rewards are claimed.
        /// </summary>
        private const string KeyAreDailyCommissionsBonusRewardsClaimed = "areDailyCommissionsBonusRewardsClaimed";

        /// <summary>
        /// The key of the flag indicating if the daily commissions' bonus rewards are claimed, for processing JSON data.
        /// </summary>
        private const string KeyAreDailyCommissionsBonusRewardsClaimedRaw = "is_extra_task_reward_received";

        /// <summary>
        /// The avatar key.
        /// </summary>
        private const string KeyAvatar = "avatar";

        /// <summary>
        /// The avatar side icon key.
        /// </summary>
        private const string KeyAvatarSideIcon = "avatarSideIcon";

        /// <summary>
        /// The finished daily commissions key.
        /// </summary>
        private const string KeyCommissionsDailyFinished = "commissionsDailyFinished";

        /// <summary>
        /// The finished daily commissions key for processing JSON data.
        /// </summary>
        private const string KeyCommissionsDailyFinishedRaw = "finished_task_num";

        /// <summary>
        /// The max daily commissions key.
        /// </summary>
        private const string KeyCommissionsDailyMax = "commissionsDailyMax";

        /// <summary>
        /// The max daily commissions key for processing JSON data.
        /// </summary>
        private const string KeyCommissionsDailyMaxRaw = "total_task_num";

        /// <summary>
        /// The cookies key.
        /// </summary>
        public const string KeyCookies = "cookies";

        /// <summary>
        /// The current Realm Currency key.
        /// </summary>
        private const string KeyCurrencyRealmCurrent = "currencyRealmCurrent";

        /// <summary>
        /// The current Realm Currency key for processing JSON data.
        /// </summary>
        private const string KeyCurrencyRealmCurrentRaw = "current_home_coin";

        /// <summary>
        /// The max Realm Currency key.
        /// </summary>
        private const string KeyCurrencyRealmMax = "currencyRealmMax";

        /// <summary>
        /// The max Realm Currency key for processing JSON data.
        /// </summary>
        private const string KeyCurrencyRealmMaxRaw = "max_home_coin";

        /// <summary>
        /// The Realm Currency recovery time key.
        /// </summary>
        private const string KeyCurrencyRealmTimeRecovery = "currencyRealmTimeRecovery";

        /// <summary>
        /// The Realm Currency recovery time key for processing JSON data.
        /// </summary>
        private const string KeyCurrencyRealmTimeRecoveryRaw = "home_coin_recovery_time";

        /// <summary>
        /// The data key.
        /// </summary>
        private const string KeyData = "data";

        /// <summary>
        /// The day key for processing JSON data.
        /// </summary>
        private const string KeyDayRaw = "Day";

        /// <summary>
        /// The max Trounce Domains discount key.
        /// </summary>
        private const string KeyDomainsTrounceDiscountsMax = "domainsTrounceDiscountsMax";

        /// <summary>
        /// The max Trounce Domains discount key for processing JSON data.
        /// </summary>
        private const string KeyDomainsTrounceDiscountsMaxRaw = "resin_discount_num_limit";

        /// <summary>
        /// The remaining Trounce Domains discount key.
        /// </summary>
        private const string KeyDomainsTrounceDiscountsRemaining = "domainsTrounceDiscountsRemaining";

        /// <summary>
        /// The remaining Trounce Domains discount key for processing JSON data.
        /// </summary>
        private const string KeyDomainsTrounceDiscountsRemainingRaw = "remain_resin_discount_num";

        /// <summary>
        /// The expedition's avatar side icon key for processing JSON data.
        /// </summary>
        private const string KeyExpeditionAvatarSideIconRaw = "avatar_side_icon";

        /// <summary>
        /// The current expeditions key.
        /// </summary>
        private const string KeyExpeditionsCurrent = "expeditionsCurrent";

        /// <summary>
        /// The current expeditions key for processing JSON data.
        /// </summary>
        private const string KeyExpeditionsCurrentRaw = "current_expedition_num";

        /// <summary>
        /// The max expeditions key.
        /// </summary>
        private const string KeyExpeditionsMax = "expeditionsMax";

        /// <summary>
        /// The max expeditions key for processing JSON data.
        /// </summary>
        private const string KeyExpeditionsMaxRaw = "max_expedition_num";

        /// <summary>
        /// The expeditions key for processing JSON data.
        /// </summary>
        private const string KeyExpeditionsRaw = "expeditions";

        /// <summary>
        /// The expedition status key for processing JSON data.
        /// </summary>
        private const string KeyExpeditionStatusRaw = "status";

        /// <summary>
        /// The remaining expedition time key for processing JSON data.
        /// </summary>
        private const string KeyExpeditionTimeRemainingRaw = "remained_time";

        /// <summary>
        /// The hour key for processing JSON data.
        /// </summary>
        private const string KeyHourRaw = "Hour";

        /// <summary>
        /// The key of the flag indicating if the subject is enabled.
        /// </summary>
        private const string KeyIsEnabled = "isEnabled";

        /// <summary>
        /// The key of the flag indicating if the Parametric Transformer is obtained.
        /// </summary>
        private const string KeyIsParametricTransformerObtained = "isParametricTransformerObtained";

        /// <summary>
        /// The key of the flag indicating if the Parametric Transformer is obtained, for processing JSON data.
        /// </summary>
        private const string KeyIsParametricTransformerObtainedRaw = "obtained";

        /// <summary>
        /// The key of the flag indicating if the Parametric Transformer recovery time is reached.
        /// </summary>
        private const string KeyIsParametricTransformerRecoveryTimeReached =
            "isParametricTransformerRecoveryTimeReached";

        /// <summary>
        /// The key of the flag indicating if the Parametric Transformer recovery time is reached, for processing JSON data.
        /// </summary>
        private const string KeyIsParametricTransformerRecoveryTimeReachedRaw = "reached";

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
        /// The minute key for processing JSON data.
        /// </summary>
        private const string KeyMinuteRaw = "Minute";

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
        /// The current Original Resin key.
        /// </summary>
        private const string KeyResinOriginalCurrent = "resinOriginalCurrent";

        /// <summary>
        /// The current Original Resin key for processing JSON data.
        /// </summary>
        private const string KeyResinOriginalCurrentRaw = "current_resin";

        /// <summary>
        /// The max Original Resin key.
        /// </summary>
        private const string KeyResinOriginalMax = "resinOriginalMax";

        /// <summary>
        /// The max Original Resin key for processing JSON data.
        /// </summary>
        private const string KeyResinOriginalMaxRaw = "max_resin";

        /// <summary>
        /// The Original Resin recovery time key.
        /// </summary>
        private const string KeyResinOriginalTimeRecovery = "resinOriginalTimeRecovery";

        /// <summary>
        /// The Original Resin recovery time key for processing JSON data.
        /// </summary>
        private const string KeyResinOriginalTimeRecoveryRaw = "resin_recovery_time";

        /// <summary>
        /// The return code key.
        /// </summary>
        public const string KeyReturnCode = "retcode";

        /// <summary>
        /// The second key for processing JSON data.
        /// </summary>
        private const string KeySecondRaw = "Second";

        /// <summary>
        /// The server key.
        /// </summary>
        public const string KeyServer = "server";

        /// <summary>
        /// The status key.
        /// </summary>
        public const string KeyStatus = "status";

        /// <summary>
        /// The remaining time key.
        /// </summary>
        private const string KeyTimeRemaining = "timeRemaining";

        /// <summary>
        /// The last update time key.
        /// </summary>
        private const string KeyTimeUpdateLast = "timeUpdateLast";

        /// <summary>
        /// The Parametric Transformer key for processing JSON data.
        /// </summary>
        private const string KeyTransformerParametricRaw = "transformer";

        /// <summary>
        /// The Parametric Transformer recovery time key.
        /// </summary>
        private const string KeyTransformerParametricTimeRecovery = "transformerParametricTimeRecovery";

        /// <summary>
        /// The Parametric Transformer recovery time key for processing JSON data.
        /// </summary>
        private const string KeyTransformerParametricTimeRecoveryRaw = "recovery_time";

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
        /// The property name for the flag indicating if an account's character is updated.
        /// </summary>
        public const string PropertyNameIsAccountCharacterUpdated = nameof(IsAccountCharacterUpdated);

        /// <summary>
        /// The property name for the flag indicating if an account group is updated.
        /// </summary>
        public const string PropertyNameIsAccountGroupUpdated = nameof(IsAccountGroupUpdated);

        /// <summary>
        /// The property name for the flag indicating if the program is adding/updating an account.
        /// </summary>
        public const string PropertyNameIsAddingUpdating = nameof(IsAddingUpdating);

        /// <summary>
        /// The property name for the flag indicating if the program is managing the accounts.
        /// </summary>
        public const string PropertyNameIsManaging = nameof(IsManaging);

        /// <summary>
        /// The disabled return code.
        /// </summary>
        private const int ReturnCodeDisabled = 10102;

        /// <summary>
        /// The login fail return code.
        /// </summary>
        private const int ReturnCodeLoginFail = -100;

        /// <summary>
        /// The success return code.
        /// </summary>
        private const int ReturnCodeSuccess = 0;

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
        /// The disabled status tag.
        /// </summary>
        public const string TagStatusDisabled = "disabled";

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
        /// The base URL for the CN server to get an avatar side icon.
        /// </summary>
        private const string UrlBaseAvatarSideIconServerCn =
            "https://upload-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_";

        /// <summary>
        /// The base URL for the global server to get an avatar side icon.
        /// </summary>
        private const string UrlBaseAvatarSideIconServerGlobal =
            "https://upload-os-bbs.mihoyo.com/game_record/genshin/character_side_icon/UI_AvatarIcon_Side_";

        /// <summary>
        /// The base URL for indicating the success of logging into miHoYo.
        /// </summary>
        public const string UrlBaseLoginEndMiHoYo = "https://bbs.mihoyo.com/ys/accountCenter/postList?";

        /// <summary>
        /// The base URL for the CN server to get real-time notes.
        /// </summary>
        private const string UrlBaseRealTimeNotesServerCn =
            "https://api-takumi-record.mihoyo.com/game_record/app/genshin/api/dailyNote?";

        /// <summary>
        /// The base URL for the global server to get real-time notes.
        /// </summary>
        private const string UrlBaseRealTimeNotesServerGlobal =
            "https://bbs-api-os.hoyolab.com/game_record/app/genshin/api/dailyNote?";

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
        private App _app;

        /// <summary>
        /// A flag indicating if an account's character is updated.
        /// </summary>
        private bool _isAccountCharacterUpdated;

        /// <summary>
        /// A flag indicating if an account group is updated.
        /// </summary>
        private bool _isAccountGroupUpdated;

        /// <summary>
        /// A flag indicating if the program is adding/updating an account.
        /// </summary>
        private bool _isAddingUpdating;

        /// <summary>
        /// A flag indicating if the program is managing the accounts.
        /// </summary>
        private bool _isManaging;

        /// <summary>
        /// The regions dictionary.
        /// </summary>
        private readonly Dictionary<string, string> _regions;

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
        /// A flag indicating if an account's character is updated.
        /// </summary>
        public bool IsAccountCharacterUpdated
        {
            get => _isAccountCharacterUpdated;
            set
            {
                if (_isAccountCharacterUpdated == value) return;

                _isAccountCharacterUpdated = value;
                OnPropertyChanged();
            } // end set
        } // end property IsAccountCharacterUpdated

        /// <summary>
        /// A flag indicating if an account group is updated.
        /// </summary>
        public bool IsAccountGroupUpdated
        {
            get => _isAccountGroupUpdated;
            set
            {
                if (_isAccountGroupUpdated == value) return;

                _isAccountGroupUpdated = value;
                OnPropertyChanged();
            } // end set
        } // end property IsAccountGroupUpdated

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
            _app = Application.Current as App;
            _isAccountCharacterUpdated = false;
            _isAccountGroupUpdated = false;
            _isAddingUpdating = false;
            _isManaging = false;

            var resourceLoader = _app?.SettingsH.ResLoader;

            _regions = new Dictionary<string, string>
            {
                [KeyRegionCnBilibili] = resourceLoader?.GetString("RegionCnBilibili"),
                [KeyRegionCnOfficial] = resourceLoader?.GetString("RegionCnOfficial"),
                [KeyRegionGlobalAmerica] = resourceLoader?.GetString("RegionGlobalAmerica"),
                [KeyRegionGlobalAsia] = resourceLoader?.GetString("RegionGlobalAsia"),
                [KeyRegionGlobalEurope] = resourceLoader?.GetString("RegionGlobalEurope"),
                [KeyRegionGlobalSars] = resourceLoader?.GetString("RegionGlobalSars"),
            };
            AccountGroupInfoLists = new ObservableCollection<GroupInfoList>();
            ApplicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(ContainerKeyAccounts,
                    ApplicationDataCreateDisposition
                        .Always); // The container's containers are in a read-only dictionary, and should not be stored.
            CheckAccountsAsync(
                _app?.SettingsH.PropertySetSettings[SettingsHelper.KeyAccountGroupsCheckRefreshWhenAppStarts] is true);
        } // end constructor AccountsHelper

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~AccountsHelper()
        {
            _app = null;
        } // end destructor AccountsHelper

        #endregion Destructor

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
            var cookies = propertySetAccount[KeyCookies] as string;
            var nicknameAccount = propertySetAccount[KeyNickname] as string;
            var resourceLoader = _app.SettingsH.ResLoader;
            var server = propertySetAccount[KeyServer] switch
            {
                TagServerCn => resourceLoader.GetString("ServerCn"),
                TagServerGlobal => resourceLoader.GetString("ServerGlobal"),
                _ => AppConstantsHelper.Unknown
            };
            var status = propertySetAccount[KeyStatus] as string;
            var timeUpdateLast = propertySetAccount[KeyTimeUpdateLast] as DateTimeOffset?;
            var uidAccount = propertySetAccount[KeyUid] as string;

            if (applicationDataContainersCharacter.Count > 0)
                accountCharacters.AddRange(from keyValuePairCharacter in applicationDataContainersCharacter
                    orderby keyValuePairCharacter.Key // Order by the character's UID.
                    let propertySetCharacter = keyValuePairCharacter.Value.Values
                    let level = propertySetCharacter[KeyLevel]
                    select new AccountCharacter
                    {
                        Cookies = cookies,
                        IsEnabled = propertySetCharacter[KeyIsEnabled] as bool? ?? true,
                        Key = containerKeyAccount,
                        Level = level is null ? $"{PrefixLevel}{AppConstantsHelper.Unknown}" : $"{PrefixLevel}{level}",
                        NicknameAccount = nicknameAccount,
                        NicknameCharacter = propertySetCharacter[KeyNickname] as string,
                        Region = GetRegion(propertySetCharacter[KeyRegion] as string),
                        Server = server,
                        Status = status,
                        TimeUpdateLast = timeUpdateLast,
                        UidAccount = uidAccount,
                        UidCharacter = keyValuePairCharacter.Key
                    });
            else
                accountCharacters.Add(new AccountCharacter
                {
                    Cookies = cookies,
                    Key = containerKeyAccount,
                    NicknameAccount = nicknameAccount,
                    Server = server,
                    Status = status,
                    TimeUpdateLast = timeUpdateLast,
                    UidAccount = uidAccount
                }); // Add an account's character containing account info only when no character for UI rendering.

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
                IsAccountGroupUpdated = false;
                accountGroupInfoListTarget.Clear();
                accountGroupInfoListTarget.AddRange(accountCharacters);
                IsAccountGroupUpdated = true;
            } // end if...else
        } // end method AddUpdateAccountGroup

        /// <summary>
        /// Add/Update the specific account's characters.
        /// NOTE: If the account container key is valid and the characters list is not <c>null</c>, the relevant account group will also be added/updated.
        /// </summary>
        /// <param name="characters">A list of characters.</param>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A task just to indicate that any later operation needs to wait.</returns>
        public async Task AddUpdateCharactersAsync(ImmutableList<Character> characters,
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
                Log.Warning($"Failed to store null characters (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = propertySetAccount[KeyStatus] is TagStatusExpired
                    ? TagStatusExpired
                    : TagStatusFail;
                return;
            } // end if

            if (characters.Count is 0)
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

                    await GetRealTimeNotesFromApiAsync(containerKeyAccount, character.Uid);
                } // end foreach

                if (propertySetAccount[KeyStatus] is TagStatusAdding or TagStatusUpdating)
                    propertySetAccount[KeyStatus] = TagStatusReady;
            } // end if...else

            AddUpdateAccountGroup(containerKeyAccount);
        } // end method AddUpdateCharactersAsync

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

            IsAccountCharacterUpdated = false;
            accountCharacterTarget.IsEnabled = shouldEnableCharacter;
            accountGroupInfoListTarget.Insert(accountCharacterTargetIndex, accountCharacterTarget);
            accountGroupInfoListTarget.RemoveAt(accountCharacterTargetIndex + 1);
            CheckSelectedCharacterUid();
            IsAccountCharacterUpdated = true;
        } // end method ApplyCharacterStatus

        /// <summary>
        /// Check the account.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="isStandalone">A flag indicating if the operation is standalone.</param>
        /// <returns>A task just to indicate that any later operation needs to wait.</returns>
        public async Task CheckAccountAsync(string containerKeyAccount, bool isStandalone = false)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            if (isStandalone) IsManaging = true;

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

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
                case TagStatusReady:
                    propertySetAccount[KeyStatus] = TagStatusUpdating;
                    break;
            } // end switch-case

            if (shouldAddUpdateCharacters)
                await AddUpdateCharactersAsync(await GetAccountCharactersFromApiAsync(containerKeyAccount),
                    containerKeyAccount);

            if (!isStandalone) return;

            CheckSelectedCharacterUid();
            IsManaging = false;
        } // end method CheckAccountAsync

        /// <summary>
        /// Check the accounts.
        /// </summary>
        /// <param name="shouldCheckAccount">A flag indicating if an account should be checked.</param>
        public async void CheckAccountsAsync(bool shouldCheckAccount = true)
        {
            IsManaging = true;

            foreach (var containerKeyAccount in ApplicationDataContainerAccounts.Containers.Keys)
                if (shouldCheckAccount) await CheckAccountAsync(containerKeyAccount);
                else AddUpdateAccountGroup(containerKeyAccount);

            if (!shouldCheckAccount) GetRealTimeNotesFromApiForAllEnabledAsync();

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

                var isSelectedCharacterValid = false;
                var uidCharacterSelected = propertySetAccounts[KeyUidCharacterSelected] as string;

                foreach (var accountCharacters in AccountGroupInfoLists.Select(accountGroupInfoList =>
                             accountGroupInfoList.Cast<AccountCharacter>()))
                {
                    isSelectedCharacterValid = accountCharacters.Any(accountCharacter =>
                        accountCharacter.UidCharacter == uidCharacterSelected && accountCharacter.IsEnabled);

                    if (isSelectedCharacterValid) break;
                } // end foreach

                if (!isSelectedCharacterValid) propertySetAccounts[KeyUidCharacterSelected] = null;
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
        /// Get the specific account from the API. The method is usually used before getting the account's characters from the API.
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
                    $"Failed to get the account from the API due to its expired status (account container key: {containerKeyAccount}).");
                return false;
            } // end if

            propertySetAccount[KeyTimeUpdateLast] = DateTimeOffset.UtcNow;

            var isServerCn =
                propertySetAccount[KeyServer] is TagServerCn;
            var httpResponseBody = await _app.HttpClientH.SendGetRequestAsync(propertySetAccount[KeyCookies] as string,
                isServerCn,
                isServerCn ? UrlAccountServerCn : UrlAccountServerGlobal); // Send an HTTP GET request when ready.

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get the account from the API due to null HTTP response message content (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = TagStatusFail;
                return true;
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the account response's body (account container key: {containerKeyAccount}):");
                    Log.Information(httpResponseBody);
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return true;
                } // end if

                var returnCode = (int?)jsonNodeResponse[KeyReturnCode];

                if (returnCode is not ReturnCodeSuccess)
                {
                    Log.Warning(
                        $"Failed to get the account from the specific API (account container key: {containerKeyAccount}, message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}).");
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
                Log.Error(
                    $"Failed to parse the account response's body (account container key: {containerKeyAccount}):");
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

            return new Uri($"{urlBaseAvatar}{propertySetAccount[KeyAvatar]}{FileExtensionPng}");
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
                    $"Failed to get characters from the API due to the specific account's expired status (account container key: {containerKeyAccount}).");
                return null;
            } // end if

            var isServerCn = propertySetAccount[KeyServer] is TagServerCn;
            var httpResponseBody = await _app.HttpClientH.SendGetRequestAsync(propertySetAccount[KeyCookies] as string,
                isServerCn,
                isServerCn ? UrlCharactersServerCn : UrlCharactersServerGlobal); // Send an HTTP GET request when ready.

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get characters from the API due to null HTTP response message content (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = TagStatusFail;
                return null;
            } // end if

            try
            {
                var charactersRaw = JsonSerializer.Deserialize<CharactersResponse>(httpResponseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (charactersRaw is null)
                {
                    Log.Warning(
                        $"Failed to parse the characters response's body (account container key: {containerKeyAccount}):");
                    Log.Information(httpResponseBody);
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return null;
                } // end if

                if (charactersRaw.ReturnCode is not ReturnCodeSuccess)
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
                Log.Error(
                    $"Failed to parse the characters response's body (account container key: {containerKeyAccount}):");
                Log.Information(httpResponseBody);
                Log.Error(exception.ToString());
                propertySetAccount[KeyStatus] = TagStatusFail;
                return null;
            } // end try...catch
        } // end method GetCharactersFromApiAsync

        /// <summary>
        /// Get a date and time string.
        /// </summary>
        /// <param name="dateTime">The struct containing the date and time.</param>
        /// <returns>The date and time string.</returns>
        public string GetDateTimeString(DateTimeOffset? dateTime)
        {
            if (dateTime is null) return AppConstantsHelper.Unknown;

            var cultureApplied = _app.SettingsH.CultureApplied;
            var dateTimeLocal = ((DateTimeOffset)dateTime).ToLocalTime();
            var dateTimeNowDay = DateTimeOffset.Now.Day;
            var resourceLoader = _app.SettingsH.ResLoader;

            if (dateTimeLocal.Day == dateTimeNowDay - 1)
                return $"{resourceLoader.GetString("Yesterday")} {dateTimeLocal.ToString("t", cultureApplied)}";

            if (dateTimeLocal.Day == dateTimeNowDay)
                return $"{resourceLoader.GetString("Today")} {dateTimeLocal.ToString("t", cultureApplied)}";

            return dateTimeLocal.Day == dateTimeNowDay + 1
                ? $"{resourceLoader.GetString("Tomorrow")} {dateTimeLocal.ToString("t", cultureApplied)}"
                : dateTimeLocal.ToString("g", cultureApplied);
        } // end method GetDateTimeString

        /// <summary>
        /// Get the real-time notes.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <returns>A tuple. 1st item: the expeditions header; 2nd item: the expedition notes; 3rd item: the general notes; 4th item: the status; 5th item: the last update time.</returns>
        public (RealTimeNote, ImmutableList<RealTimeNote>, ImmutableList<RealTimeNote>, string, string)
            GetRealTimeNotes(string containerKeyAccount,
                string containerKeyCharacter)
        {
            var resourceLoader = _app.SettingsH.ResLoader; // Get the resource loader first.
            var colonAndEstimated =
                $"{resourceLoader.GetString("Colon")}{resourceLoader.GetString("Estimated")} "; // The resource string for ": est. ".
            var commissionsDailyExplanation = AppConstantsHelper.Unknown;
            int? commissionsDailyFinished = null;
            int? commissionsDailyMax = null;
            int? currencyRealmCurrent = null;
            var currencyRealmExplanation = AppConstantsHelper.Unknown;
            int? currencyRealmMax = null;
            int? domainsTrounceDiscountsMax = null;
            int? domainsTrounceDiscountsRemaining = null;
            int? expeditionsCurrent = null;
            int? expeditionsMax = null;
            var realTimeNotesExpeditions = new List<RealTimeNote>(); // 2. Real-time notes' expeditions section.
            var realTimeNotesGeneral = new List<RealTimeNote>(); // 1. Real-time notes' general section.
            string realTimeNotesStatus = null;
            var realTimeNotesTimeUpdateLast = AppConstantsHelper.Unknown;
            int? resinOriginalCurrent = null;
            var resinOriginalExplanation = AppConstantsHelper.Unknown;
            int? resinOriginalMax = null;
            var transformerParametricExplanation = AppConstantsHelper.Unknown;
            var transformerParametricStatus = AppConstantsHelper.Unknown;

            if (ValidateAccountContainerKey(containerKeyAccount))
            {
                if (containerKeyCharacter is null)
                    Log.Warning($"Null character container key (account container key: {containerKeyAccount}).");
                else
                {
                    var applicationDataContainerAccount =
                        ApplicationDataContainerAccounts.Containers[containerKeyAccount];
                    var (_, applicationDataContainerCharacter) = applicationDataContainerAccount
                        .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers
                        .FirstOrDefault(keyValuePairCharacter => keyValuePairCharacter.Key == containerKeyCharacter,
                            new KeyValuePair<string, ApplicationDataContainer>(containerKeyCharacter, null));

                    if (applicationDataContainerCharacter is null)
                        Log.Warning(
                            $"Failed to find the specific account's character (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    else
                    {
                        var applicationDataContainerRealTimeNotes =
                            applicationDataContainerCharacter.CreateContainer(ContainerKeyRealTimeNotes,
                                ApplicationDataCreateDisposition.Always);
                        var propertySetRealTimeNotes = applicationDataContainerRealTimeNotes.Values;

                        realTimeNotesStatus = propertySetRealTimeNotes[KeyStatus] as string;
                        realTimeNotesTimeUpdateLast =
                            GetDateTimeString(propertySetRealTimeNotes[KeyTimeUpdateLast] as DateTimeOffset?);

                        if (realTimeNotesStatus is null or TagStatusDisabled)
                        {
                            realTimeNotesStatus ??= TagStatusFail; // Ensure the local variable first.
                            propertySetRealTimeNotes[KeyStatus] = realTimeNotesStatus;
                        }
                        else
                        {
                            var areDailyCommissionsBonusRewardsClaimed =
                                propertySetRealTimeNotes[KeyAreDailyCommissionsBonusRewardsClaimed] as bool?;
                            var isParametricTransformerObtained =
                                propertySetRealTimeNotes[KeyIsParametricTransformerObtained] as bool?;
                            var isParametricTransformerRecoveryTimeReached =
                                propertySetRealTimeNotes[KeyIsParametricTransformerRecoveryTimeReached] as bool?;

                            commissionsDailyFinished = propertySetRealTimeNotes[KeyCommissionsDailyFinished] as int?;
                            commissionsDailyMax = propertySetRealTimeNotes[KeyCommissionsDailyMax] as int?;
                            currencyRealmCurrent = propertySetRealTimeNotes[KeyCurrencyRealmCurrent] as int?;
                            currencyRealmExplanation = resourceLoader.GetString("CurrencyRealmLimitReached");
                            currencyRealmMax = propertySetRealTimeNotes[KeyCurrencyRealmMax] as int?;
                            domainsTrounceDiscountsMax =
                                propertySetRealTimeNotes[KeyDomainsTrounceDiscountsMax] as int?;
                            domainsTrounceDiscountsRemaining =
                                propertySetRealTimeNotes[KeyDomainsTrounceDiscountsRemaining] as int?;
                            expeditionsCurrent = propertySetRealTimeNotes[KeyExpeditionsCurrent] as int?;
                            expeditionsMax = propertySetRealTimeNotes[KeyExpeditionsMax] as int?;
                            resinOriginalCurrent = propertySetRealTimeNotes[KeyResinOriginalCurrent] as int?;
                            resinOriginalExplanation = resourceLoader.GetString("ResinOriginalReplenishedFully");
                            resinOriginalMax = propertySetRealTimeNotes[KeyResinOriginalMax] as int?;

                            if (commissionsDailyFinished is not null && commissionsDailyMax is not null)
                            {
                                if (commissionsDailyFinished == commissionsDailyMax)
                                    commissionsDailyExplanation = commissionsDailyMax is 0
                                        ? resourceLoader.GetString("CommissionsDailyLocked")
                                        : resourceLoader.GetString(areDailyCommissionsBonusRewardsClaimed switch
                                        {
                                            null => "CommissionsDailyRewardsBonusUnknown",
                                            true => "CommissionsDailyRewardsBonusClaimed",
                                            _ => "CommissionsDailyRewardsBonusUnclaimed"
                                        });
                                else
                                    commissionsDailyExplanation =
                                        resourceLoader.GetString("CommissionsDailyIncomplete");
                            } // end if

                            if (currencyRealmCurrent is not null && currencyRealmMax is not null &&
                                currencyRealmCurrent == currencyRealmMax)
                            {
                                if (currencyRealmMax is 0)
                                    currencyRealmExplanation = resourceLoader.GetString("CurrencyRealmLocked");
                            }
                            else
                                currencyRealmExplanation +=
                                    $"{colonAndEstimated}{GetDateTimeString(propertySetRealTimeNotes[KeyCurrencyRealmTimeRecovery] as DateTimeOffset?)}";

                            if (expeditionsCurrent is null || expeditionsMax is null)
                                realTimeNotesExpeditions.Add(new RealTimeNote
                                {
                                    Explanation = AppConstantsHelper.Unknown,
                                    UriImage = null
                                });
                            else if (expeditionsCurrent > 0)
                            {
                                var applicationDataContainerExpeditions =
                                    applicationDataContainerRealTimeNotes.CreateContainer(ContainerKeyExpeditions,
                                        ApplicationDataCreateDisposition.Always);

                                // 2.X. Each expedition.
                                for (var i = 0; i < expeditionsCurrent; i++)
                                {
                                    var propertySetExpedition = applicationDataContainerExpeditions
                                        .CreateContainer(i.ToString(), ApplicationDataCreateDisposition.Always).Values;
                                    var expeditionAvatarSideIcon = propertySetExpedition[KeyAvatarSideIcon] as string;
                                    var expeditionExplanation = resourceLoader.GetString("ExpeditionComplete");
                                    var expeditionStatus = propertySetExpedition[KeyStatus] as string;
                                    var urlBaseAvatarSideIcon = applicationDataContainerAccount.Values[KeyServer] switch
                                    {
                                        TagServerCn => UrlBaseAvatarSideIconServerCn,
                                        TagServerGlobal => UrlBaseAvatarSideIconServerGlobal,
                                        _ => null
                                    };

                                    if (expeditionStatus is not ExpeditionStatusFinished)
                                        expeditionExplanation +=
                                            $"{colonAndEstimated}{GetDateTimeString(propertySetExpedition[KeyTimeRemaining] as DateTimeOffset?)}";

                                    realTimeNotesExpeditions.Add(new RealTimeNote
                                    {
                                        Explanation = expeditionExplanation,
                                        Status = expeditionStatus,
                                        UriImage = expeditionAvatarSideIcon is null || urlBaseAvatarSideIcon is null
                                            ? null
                                            : new Uri(
                                                $"{urlBaseAvatarSideIcon}{expeditionAvatarSideIcon}{FileExtensionPng}")
                                    });
                                } // end for
                            }
                            else
                                realTimeNotesExpeditions.Add(new RealTimeNote
                                {
                                    Explanation = resourceLoader.GetString("ExpeditionsNone"),
                                    UriImage = null
                                });

                            switch (isParametricTransformerObtained)
                            {
                                case null:
                                    break;

                                case true:
                                    transformerParametricExplanation =
                                        resourceLoader.GetString("TransformerParametricUsable");

                                    switch (isParametricTransformerRecoveryTimeReached)
                                    {
                                        case null:
                                            transformerParametricExplanation +=
                                                $"{colonAndEstimated}{AppConstantsHelper.Unknown}";
                                            break;

                                        case true:
                                            transformerParametricStatus =
                                                resourceLoader.GetString("TransformerParametricReady");
                                            break;

                                        default:
                                            transformerParametricExplanation +=
                                                $"{colonAndEstimated}{GetDateTimeString(propertySetRealTimeNotes[KeyTransformerParametricTimeRecovery] as DateTimeOffset?)}";
                                            transformerParametricStatus =
                                                resourceLoader.GetString("TransformerParametricCooldown");
                                            break;
                                    } // end switch-case

                                    break;

                                default:
                                    transformerParametricExplanation =
                                        resourceLoader.GetString("TransformerParametricLocked");
                                    transformerParametricStatus = resourceLoader.GetString("RealTimeNotesStatusLocked");
                                    break;
                            } // end switch-case

                            if (!(resinOriginalCurrent is not null && resinOriginalMax is not null &&
                                  resinOriginalCurrent == resinOriginalMax))
                                resinOriginalExplanation +=
                                    $"{colonAndEstimated}{GetDateTimeString(propertySetRealTimeNotes[KeyResinOriginalTimeRecovery] as DateTimeOffset?)}";
                        } // end if...else
                    } // end if...else
                } // end if...else
            } // end if

            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = resinOriginalExplanation,
                Status = GetRealTimeNoteStatus(resinOriginalCurrent, resinOriginalMax),
                Title = resourceLoader.GetString("ResinOriginal"),
                UriImage = new Uri(AppConstantsHelper.UriImageResinOriginal)
            }); // 1.1. Original Resin.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = currencyRealmExplanation,
                Status = GetRealTimeNoteStatus(currencyRealmCurrent, currencyRealmMax),
                Title = resourceLoader.GetString("CurrencyRealm"),
                UriImage = new Uri(AppConstantsHelper.UriImageCurrencyRealm)
            }); // 1.2. Realm Currency.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = commissionsDailyExplanation,
                Status = GetRealTimeNoteStatus(commissionsDailyFinished, commissionsDailyMax),
                Title = resourceLoader.GetString("CommissionsDaily"),
                UriImage = new Uri(AppConstantsHelper.UriImageCommissionsDaily)
            }); // 1.3. Daily commissions.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = resourceLoader.GetString("DomainsTrounceDiscountsExplanation"),
                Status = GetRealTimeNoteStatus(domainsTrounceDiscountsRemaining, domainsTrounceDiscountsMax),
                Title = resourceLoader.GetString("DomainsTrounceDiscounts"),
                UriImage = new Uri(AppConstantsHelper.UriImageDomainsTrounce)
            }); // 1.4. Trounce Domains discounts.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = transformerParametricExplanation,
                Status = transformerParametricStatus,
                Title = resourceLoader.GetString("TransformerParametric"),
                UriImage = new Uri(AppConstantsHelper.UriImageTransformerParametric)
            }); // 1.5. Parametric Transformer.

            return (
                new RealTimeNote
                {
                    Explanation = resourceLoader.GetString("ExpeditionsExplanation"),
                    Status = GetRealTimeNoteStatus(expeditionsCurrent, expeditionsMax),
                    Title = resourceLoader.GetString("Expeditions")
                },
                realTimeNotesExpeditions.ToImmutableList(),
                realTimeNotesGeneral.ToImmutableList(),
                realTimeNotesStatus,
                realTimeNotesTimeUpdateLast);
        } // end method GetRealTimeNotes

        /// <summary>
        /// Get the real-time notes from the API.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <returns>A task just to indicate that any later operation needs to wait.</returns>
        private async Task GetRealTimeNotesFromApiAsync(string containerKeyAccount, string containerKeyCharacter)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            if (containerKeyCharacter is null)
            {
                Log.Warning($"Null character container key (account container key: {containerKeyAccount}).");
                return;
            } // end if

            var applicationDataContainerAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount];
            var (_, applicationDataContainerCharacter) = applicationDataContainerAccount
                .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers
                .FirstOrDefault(keyValuePairCharacter => keyValuePairCharacter.Key == containerKeyCharacter,
                    new KeyValuePair<string, ApplicationDataContainer>(containerKeyCharacter, null));

            if (applicationDataContainerCharacter is null)
            {
                Log.Warning(
                    $"Failed to find the specific account's character (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                return;
            } // end if

            var applicationDataContainerRealTimeNotes =
                applicationDataContainerCharacter.CreateContainer(ContainerKeyRealTimeNotes,
                    ApplicationDataCreateDisposition.Always);
            var propertySetAccount = applicationDataContainerAccount.Values;
            var propertySetRealTimeNotes = applicationDataContainerRealTimeNotes.Values;

            propertySetRealTimeNotes[KeyStatus] = TagStatusUpdating;
            propertySetRealTimeNotes[KeyTimeUpdateLast] = DateTimeOffset.UtcNow;

            if (propertySetAccount[KeyStatus] is TagStatusExpired)
            {
                Log.Warning(
                    $"Failed to get real-time notes from the API due to the specific account's expired status (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                return;
            } // end if

            var region = applicationDataContainerCharacter.Values[KeyRegion] as string;

            if (region is null || !_regions.ContainsKey(region))
            {
                Log.Warning(
                    $"Invalid region (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}, region: {region}).");
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                return;
            } // end if

            var isServerCn = propertySetAccount[KeyServer] is TagServerCn;
            var query = $"role_id={applicationDataContainerCharacter.Name}&server={region}";
            var urlBaseRealTimeNotes = isServerCn ? UrlBaseRealTimeNotesServerCn : UrlBaseRealTimeNotesServerGlobal;
            var httpResponseBody = await _app.HttpClientH.SendGetRequestAsync(propertySetAccount[KeyCookies] as string,
                isServerCn, $"{urlBaseRealTimeNotes}{query}", true, query); // Send an HTTP GET request when ready.

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get real-time notes from the API due to null HTTP response message content (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                return;
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the real-time notes response's body (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}):");
                    Log.Information(httpResponseBody);
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                    return;
                } // end if

                var returnCode = (int?)jsonNodeResponse[KeyReturnCode];

                if (returnCode is not ReturnCodeSuccess)
                {
                    Log.Warning(
                        $"Failed to get real-time notes from the specific API (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}, message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}).");
                    propertySetAccount[KeyStatus] =
                        returnCode is ReturnCodeDisabled ? TagStatusDisabled : TagStatusFail;
                    return;
                } // end if

                var data = jsonNodeResponse[KeyData];

                if (data is null)
                {
                    Log.Warning(
                        $"Failed to get real-time notes data (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                    return;
                } // end if

                var areDailyCommissionsBonusRewardsClaimed = (bool?)data[KeyAreDailyCommissionsBonusRewardsClaimedRaw];

                if (areDailyCommissionsBonusRewardsClaimed is null)
                {
                    Log.Warning(
                        $"Failed to get the flag indicating if the daily commissions' bonus rewards are claimed from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else
                    propertySetRealTimeNotes[KeyAreDailyCommissionsBonusRewardsClaimed] =
                        areDailyCommissionsBonusRewardsClaimed;

                var commissionsDailyFinished = (int?)data[KeyCommissionsDailyFinishedRaw];

                if (commissionsDailyFinished is null)
                {
                    Log.Warning(
                        $"Failed to get the finished daily commissions from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyCommissionsDailyFinished] = commissionsDailyFinished;

                var commissionsDailyMax = (int?)data[KeyCommissionsDailyMaxRaw];

                if (commissionsDailyMax is null)
                {
                    Log.Warning(
                        $"Failed to get the max daily commissions from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyCommissionsDailyMax] = commissionsDailyMax;

                var currencyRealmCurrent = (int?)data[KeyCurrencyRealmCurrentRaw];

                if (currencyRealmCurrent is null)
                {
                    Log.Warning(
                        $"Failed to get the current Realm Currency from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyCurrencyRealmCurrent] = currencyRealmCurrent;

                var currencyRealmMax = (int?)data[KeyCurrencyRealmMaxRaw];

                if (currencyRealmMax is null)
                {
                    Log.Warning(
                        $"Failed to get the max Realm Currency from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyCurrencyRealmMax] = currencyRealmMax;

                if (int.TryParse((string)data[KeyCurrencyRealmTimeRecoveryRaw], out var currencyRealmTimeRecovery))
                    propertySetRealTimeNotes[KeyCurrencyRealmTimeRecovery] =
                        DateTimeOffset.UtcNow.AddSeconds(currencyRealmTimeRecovery);
                else
                {
                    Log.Warning(
                        $"Failed to get the Realm Currency recovery time from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                } // end if...else

                var domainsTrounceDiscountsMax = (int?)data[KeyDomainsTrounceDiscountsMaxRaw];

                if (domainsTrounceDiscountsMax is null)
                {
                    Log.Warning(
                        $"Failed to get the max Trounce Domains discounts from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyDomainsTrounceDiscountsMax] = domainsTrounceDiscountsMax;

                var domainsTrounceDiscountsRemaining = (int?)data[KeyDomainsTrounceDiscountsRemainingRaw];

                if (domainsTrounceDiscountsRemaining is null)
                {
                    Log.Warning(
                        $"Failed to get the remaining Trounce Domains discounts from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyDomainsTrounceDiscountsRemaining] = domainsTrounceDiscountsRemaining;

                var expeditions = data[KeyExpeditionsRaw];

                if (expeditions is null or not JsonArray)
                {
                    Log.Warning(
                        $"Failed to get the expeditions from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else
                {
                    applicationDataContainerRealTimeNotes.DeleteContainer(ContainerKeyExpeditions);

                    var applicationDataContainerExpeditions =
                        applicationDataContainerRealTimeNotes.CreateContainer(ContainerKeyExpeditions,
                            ApplicationDataCreateDisposition.Always);
                    var expeditionIndex = 0;

                    foreach (var expedition in expeditions as JsonArray)
                    {
                        var propertySetExpedition = applicationDataContainerExpeditions
                            .CreateContainer(expeditionIndex.ToString(), ApplicationDataCreateDisposition.Always)
                            .Values;

                        expeditionIndex++;

                        var expeditionAvatarSideIconRaw = (string)expedition[KeyExpeditionAvatarSideIconRaw];

                        if (expeditionAvatarSideIconRaw is null || expeditionAvatarSideIconRaw.Length is 0)
                        {
                            Log.Warning(
                                $"Failed to get the expedition's avatar side icon from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                            propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                        }
                        else
                        {
                            foreach (Match match in new Regex(@"(?<=(UI_AvatarIcon_Side_)).*(?=.png)").Matches(
                                         expeditionAvatarSideIconRaw))
                            {
                                propertySetExpedition[KeyAvatarSideIcon] = match.Value;
                                break;
                            } // end foreach
                        } // end if...else

                        var expeditionStatus = (string)expedition[KeyExpeditionStatusRaw];

                        if (expeditionStatus is null)
                        {
                            Log.Warning(
                                $"Failed to get the expedition status from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                            propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                        }
                        else propertySetExpedition[KeyStatus] = expeditionStatus;

                        if (int.TryParse((string)expedition[KeyExpeditionTimeRemainingRaw],
                                out var expeditionTimeRemaining))
                            propertySetExpedition[KeyTimeRemaining] =
                                DateTimeOffset.UtcNow.AddSeconds(expeditionTimeRemaining);
                        else
                        {
                            Log.Warning(
                                $"Failed to get the remaining expedition time from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                            propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                        } // end if...else
                    } // end foreach
                } // end if...else

                var expeditionsCurrent = (int?)data[KeyExpeditionsCurrentRaw];

                if (expeditionsCurrent is null)
                {
                    Log.Warning(
                        $"Failed to get the current expeditions from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyExpeditionsCurrent] = expeditionsCurrent;

                var expeditionsMax = (int?)data[KeyExpeditionsMaxRaw];

                if (expeditionsMax is null)
                {
                    Log.Warning(
                        $"Failed to get the max expeditions from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyExpeditionsMax] = expeditionsMax;

                var resinOriginalCurrent = (int?)data[KeyResinOriginalCurrentRaw];

                if (resinOriginalCurrent is null)
                {
                    Log.Warning(
                        $"Failed to get the current Original Resin from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyResinOriginalCurrent] = resinOriginalCurrent;

                var resinOriginalMax = (int?)data[KeyResinOriginalMaxRaw];

                if (resinOriginalMax is null)
                {
                    Log.Warning(
                        $"Failed to get the max Original Resin from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else propertySetRealTimeNotes[KeyResinOriginalMax] = resinOriginalMax;

                if (int.TryParse((string)data[KeyResinOriginalTimeRecoveryRaw], out var resinOriginalTimeRecovery))
                    propertySetRealTimeNotes[KeyResinOriginalTimeRecovery] =
                        DateTimeOffset.UtcNow.AddSeconds(resinOriginalTimeRecovery);
                else
                {
                    Log.Warning(
                        $"Failed to get the Original Resin recovery time from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                } // end if...else

                var transformerParametric = data[KeyTransformerParametricRaw];

                if (transformerParametric is null)
                {
                    Log.Warning(
                        $"Failed to get the Parametric Transformer data from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                }
                else
                {
                    var isParametricTransformerObtained =
                        (bool?)transformerParametric[KeyIsParametricTransformerObtainedRaw];

                    if (isParametricTransformerObtained is null)
                    {
                        Log.Warning(
                            $"Failed to get the flag indicating if the Parametric Transformer is obtained from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                        propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                    }
                    else
                    {
                        propertySetRealTimeNotes[KeyIsParametricTransformerObtained] = isParametricTransformerObtained;

                        if (isParametricTransformerObtained is true)
                        {
                            var transformerParametricRecoveryTime =
                                transformerParametric[KeyTransformerParametricTimeRecoveryRaw];

                            if (transformerParametricRecoveryTime is null)
                            {
                                Log.Warning(
                                    $"Failed to get the Parametric Transformer recovery time from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                            }
                            else
                            {
                                var isParametricTransformerRecoveryTimeReached =
                                    (bool?)transformerParametricRecoveryTime[
                                        KeyIsParametricTransformerRecoveryTimeReachedRaw];

                                if (isParametricTransformerRecoveryTimeReached is null)
                                {
                                    Log.Warning(
                                        $"Failed to get the flag indicating if the Parametric Transformer recovery time is reached from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                                }
                                else
                                {
                                    propertySetRealTimeNotes[KeyIsParametricTransformerRecoveryTimeReached] =
                                        isParametricTransformerRecoveryTimeReached;

                                    if (isParametricTransformerRecoveryTimeReached is false)
                                    {
                                        var transformerParametricRecoveryDay =
                                            (int?)transformerParametricRecoveryTime[KeyDayRaw];
                                        var transformerParametricRecoveryHour =
                                            (int?)transformerParametricRecoveryTime[KeyHourRaw];
                                        var transformerParametricRecoveryMinute =
                                            (int?)transformerParametricRecoveryTime[KeyMinuteRaw];
                                        var transformerParametricRecoverySecond =
                                            (int?)transformerParametricRecoveryTime[KeySecondRaw];

                                        if (transformerParametricRecoveryDay is null ||
                                            transformerParametricRecoveryHour is null ||
                                            transformerParametricRecoveryMinute is null ||
                                            transformerParametricRecoverySecond is null)
                                        {
                                            Log.Warning(
                                                $"Failed to get the Parametric Transformer recovery time details from real-time notes (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                                            propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
                                        }
                                        else
                                            propertySetRealTimeNotes[KeyTransformerParametricTimeRecovery] =
                                                DateTimeOffset.UtcNow.Add(new TimeSpan(
                                                    (int)transformerParametricRecoveryDay,
                                                    (int)transformerParametricRecoveryHour,
                                                    (int)transformerParametricRecoveryMinute,
                                                    (int)transformerParametricRecoverySecond));
                                    } // end if
                                } // end if...else
                            } // end if...else
                        } // end if
                    } // end if...else
                } // end if...else

                if (propertySetRealTimeNotes[KeyStatus] is TagStatusUpdating)
                    propertySetRealTimeNotes[KeyStatus] = TagStatusReady;
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"Failed to parse the real-time notes response's body (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}):");
                Log.Information(httpResponseBody);
                Log.Error(exception.ToString());
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
            } // end try...catch
        } // end method GetRealTimeNotesFromApiAsync

        /// <summary>
        /// Get the real-time notes from the API for all enabled characters.
        /// </summary>
        private async void GetRealTimeNotesFromApiForAllEnabledAsync()
        {
            foreach (var keyValuePairAccount in ApplicationDataContainerAccounts.Containers)
            foreach (var keyValuePairCharacter in from keyValuePairCharacter in keyValuePairAccount.Value
                         .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers
                         .ToImmutableList()
                     let propertySetCharacter = keyValuePairCharacter.Value.Values
                     where propertySetCharacter[KeyIsEnabled] is true
                     select keyValuePairCharacter)
                await GetRealTimeNotesFromApiAsync(keyValuePairAccount.Key, keyValuePairCharacter.Key);
        } // end method GetRealTimeNotesFromApiForAllEnabledAsync

        /// <summary>
        /// Get the real-time note's status with the current and max values.
        /// </summary>
        /// <param name="valueCurrent">The current value.</param>
        /// <param name="valueMax">The max value.</param>
        /// <returns>The real-time note's status.</returns>
        private string GetRealTimeNoteStatus(object valueCurrent, object valueMax)
        {
            var current = valueCurrent is null or not int ? AppConstantsHelper.Unknown : valueCurrent;
            var max = valueMax is null or not int ? AppConstantsHelper.Unknown : valueMax;

            return current is 0 && max is 0
                ? _app.SettingsH.ResLoader.GetString("RealTimeNotesStatusLocked")
                : current is AppConstantsHelper.Unknown && max is AppConstantsHelper.Unknown
                    ? AppConstantsHelper.Unknown
                    : $"{current}/{max}";
        } // end method GetRealTimeNoteStatus

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

            if (containerKeyCharacter is null)
            {
                Log.Warning($"Null character container key (account container key: {containerKeyAccount}).");
                return false;
            } // end if

            var (_, applicationDataContainerCharacter) = ApplicationDataContainerAccounts
                .Containers[containerKeyAccount]
                .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers
                .FirstOrDefault(keyValuePairCharacter => keyValuePairCharacter.Key == containerKeyCharacter,
                    new KeyValuePair<string, ApplicationDataContainer>(containerKeyCharacter, null));

            if (applicationDataContainerCharacter is null)
            {
                Log.Warning(
                    $"No such character container key (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                return false;
            } // end if

            var propertySetCharacter = applicationDataContainerCharacter.Values;

            if (propertySetCharacter[KeyIsEnabled] as bool? == shouldEnableCharacter) return false;

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