using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using PaimonTray.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
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
    public partial class AccountsHelper : INotifyPropertyChanged
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
        /// The MID/UID cookie key (Option 1).
        /// </summary>
        public const string CookieKeyIdOption1 = "ltuid";

        /// <summary>
        /// The MID/UID cookie key (Option 2).
        /// </summary>
        public const string CookieKeyIdOption2 = "account_id";

        /// <summary>
        /// The MID/UID cookie key (Option 3).
        /// </summary>
        public const string CookieKeyIdOption3 = "ltmid_v2";

        /// <summary>
        /// The MID/UID cookie key (Option 4).
        /// </summary>
        public const string CookieKeyIdOption4 = "account_mid_v2";

        /// <summary>
        /// The token cookie key (Option 1).
        /// </summary>
        public const string CookieKeyTokenOption1 = "ltoken";

        /// <summary>
        /// The token cookie key (Option 2).
        /// </summary>
        public const string CookieKeyTokenOption2 = "cookie_token";

        /// <summary>
        /// The token cookie key (Option 3).
        /// </summary>
        public const string CookieKeyTokenOption3 = $"{CookieKeyTokenOption1}_v2";

        /// <summary>
        /// The token cookie key (Option 4).
        /// </summary>
        public const string CookieKeyTokenOption4 = $"{CookieKeyTokenOption2}_v2";

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
        public const string KeyAvatar = "avatar";

        /// <summary>
        /// The avatar side icon key.
        /// </summary>
        private const string KeyAvatarSideIcon = "avatarSideIcon";

        /// <summary>
        /// The challenge key.
        /// </summary>
        private const string KeyChallenge = "challenge";

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
        /// The error key.
        /// </summary>
        private const string KeyError = "error";

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
        /// The GeeTest challenge key.
        /// </summary>
        public const string KeyGeeTestChallenge = "geetest_challenge";

        /// <summary>
        /// The key of the GeeTest certificate for the secondary verification.
        /// </summary>
        public const string KeyGeeTestSecCode = "geetest_seccode";

        /// <summary>
        /// The GeeTest validation key.
        /// </summary>
        public const string KeyGeeTestValidation = "geetest_validate";

        /// <summary>
        /// The GT key.
        /// </summary>
        private const string KeyGt = "gt";

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
        public const string KeyNickname = "nickname";

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
        public const string KeyTimeUpdateLast = "timeUpdateLast";

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
        /// The character UID key.
        /// </summary>
        public const string KeyUidCharacter = "game_uid";

        /// <summary>
        /// The selected character's UID key.
        /// </summary>
        public const string KeyUidCharacterSelected = "uidCharacterSelected";

        /// <summary>
        /// The user info key.
        /// </summary>
        private const string KeyUserInfo = "user_info";

        /// <summary>
        /// The validation key.
        /// </summary>
        private const string KeyValidation = "validate";

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
        /// The property name for the UID of the character updating the real-time notes.
        /// </summary>
        public const string PropertyNameUidCharacterRealTimeNotesUpdated = nameof(UidCharacterRealTimeNotesUpdated);

        /// <summary>
        /// The return code for requiring a validated GeeTest challenge.
        /// </summary>
        private const int ReturnCodeChallengeValidated = 1034;

        /// <summary>
        /// The disabled return code.
        /// </summary>
        private const int ReturnCodeDisabled = 10102;

        /// <summary>
        /// The login fail return code.
        /// </summary>
        public const int ReturnCodeLoginFail = -100;

        /// <summary>
        /// The success return code.
        /// </summary>
        public const int ReturnCodeSuccess = 0;

        /// <summary>
        /// The success status.
        /// </summary>
        private const string StatusSuccess = "success";

        /// <summary>
        /// The tag indicating that all enabled characters have updated the real-time notes.
        /// </summary>
        public const string TagRealTimeNotesUpdatedCharactersAllEnabled = "realTimeNotesUpdatedCharactersAllEnabled";

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
        /// The base URL for the CN server to create a GeeTest challenge.
        /// </summary>
        private const string UrlBaseGeeTestChallengeServerCn =
            "https://api-takumi-record.mihoyo.com/game_record/app/card/wapi/createVerification?";

        /// <summary>
        /// The base URL for the CN server to solve a GeeTest challenge.
        /// </summary>
        private const string UrlBaseGeeTestChallengeResultServerCn =
            "https://apiv6.geetest.com/ajax.php?pt=3&client_type=web_mobile&lang=zh-cn&";

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
        public const string UrlCookiesMiHoYo = "https://www.miyoushe.com";

        /// <summary>
        /// The URL for the CN server to validate the GeeTest challenge result.
        /// </summary>
        private const string UrlGeeTestChallengeResultValidationServerCn =
            "https://api-takumi-record.mihoyo.com/game_record/app/card/wapi/verifyVerification";

        /// <summary>
        /// The URL for logging in to HoYoLAB.
        /// </summary>
        public const string UrlLoginHoYoLab = $"{UrlCookiesHoYoLab}/home";

        /// <summary>
        /// The URL for logging in to miHoYo.
        /// </summary>
        public const string UrlLoginMiHoYo = $"{UrlCookiesMiHoYo}/ys";

        #endregion Constants

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
            _uidCharacterRealTimeNotesUpdated = string.Empty;
            AccountGroupInfoLists = new ObservableCollection<GroupInfoList>();
            ApplicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(ContainerKeyAccounts,
                    ApplicationDataCreateDisposition
                        .Always); // The container's containers are in a read-only dictionary, and should not be stored.
            _ = CheckAccountsAsync(
                _app?.SettingsH.PropertySetSettings[SettingsHelper.KeyAccountGroupsCheckRefreshWhenAppStarts] is true);
            SetRealTimeNotesDispatcherQueueTimerInterval();
        } // end constructor AccountsHelper

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~AccountsHelper()
        {
            _app = null;
            _dispatcherQueueTimerRealTimeNotes.Stop();
            _dispatcherQueueTimerRealTimeNotes.Tick -= DispatcherQueueTimerRealTimeNotes_OnTick;
        } // end destructor

        #endregion Destructor

        #region Events

        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Event Handlers

        // Handle the real-time notes dispatcher queue timer's tick event.
        private void DispatcherQueueTimerRealTimeNotes_OnTick(object sender, object e)
        {
            _ = GetRealTimeNotesFromApiForAllEnabledAsync();
        } // end method DispatcherQueueTimerRealTimeNotes_OnTick

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The real-time notes dispatcher queue timer.
        /// </summary>
        private DispatcherQueueTimer _dispatcherQueueTimerRealTimeNotes;

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

        /// <summary>
        /// The UID of the character updating the real-time notes.
        /// NOTE: Expected values include an empty string, a string indicating all enabled characters, and the character UID.
        /// </summary>
        private string _uidCharacterRealTimeNotesUpdated;

        #endregion Fields

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
            var server = propertySetAccount[KeyServer] as string;
            var serverName = server switch
            {
                TagServerCn => resourceLoader.GetString("ServerCn"),
                TagServerGlobal => resourceLoader.GetString("ServerGlobal"),
                _ => AppFieldsHelper.Unknown
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
                        Level = level is null ? $"{PrefixLevel}{AppFieldsHelper.Unknown}" : $"{PrefixLevel}{level}",
                        NicknameAccount = nicknameAccount,
                        NicknameCharacter = propertySetCharacter[KeyNickname] as string,
                        Region = GetRegion(propertySetCharacter[KeyRegion] as string),
                        Server = serverName,
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
                    Server = serverName,
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
        /// <returns>Void.</returns>
        public async Task AddUpdateCharactersAsync(ImmutableList<Character> characters,
            string containerKeyAccount)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            var applicationDataContainerAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount];
            var applicationDataContainerCharacters =
                applicationDataContainerAccount.CreateContainer(ContainerKeyCharacters,
                    ApplicationDataCreateDisposition.Always);
            var propertySetAccount = applicationDataContainerAccount.Values;
            var status = propertySetAccount[KeyStatus] as string;

            if (characters is null)
            {
                Log.Warning($"Failed to store null characters (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = status is TagStatusExpired ? TagStatusExpired : TagStatusFail;
            }
            else if (characters.Count is 0)
            {
                foreach (var containerKeyCharacter in applicationDataContainerCharacters.Containers.Keys)
                    applicationDataContainerCharacters.DeleteContainer(containerKeyCharacter);

                if (status is TagStatusAdding or TagStatusUpdating)
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

                    await GetRealTimeNotesFromApiAsync(null, containerKeyAccount, character.Uid);
                } // end foreach

                if (status is TagStatusAdding or TagStatusUpdating)
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
        /// <param name="isStandalone">A flag indicating if the operation is standalone. Default: <c>false</c>.</param>
        /// <returns>Void.</returns>
        public async Task CheckAccountAsync(string containerKeyAccount, bool isStandalone = false)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return;

            if (isStandalone) IsManaging = true;

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;
            var status = propertySetAccount[KeyStatus] as string;

            if (status is not TagStatusAdding and not TagStatusExpired and not TagStatusFail and not TagStatusReady
                and not TagStatusUpdating) propertySetAccount[KeyStatus] = TagStatusAdding;

            var shouldAddUpdateCharacters = true;

            switch (status)
            {
                case TagStatusAdding:
                {
                    if (propertySetAccount[KeyCookies] is null || propertySetAccount[KeyServer] is null ||
                        propertySetAccount[KeyUid] is null)
                    {
                        ApplicationDataContainerAccounts.DeleteContainer(containerKeyAccount);
                        shouldAddUpdateCharacters = false;
                    } // end if

                    break;
                }
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
        /// <param name="shouldCheckAccount">A flag indicating if an account should be checked. Default: <c>true</c>.</param>
        /// <returns>Void.</returns>
        public async Task CheckAccountsAsync(bool shouldCheckAccount = true)
        {
            IsManaging = true;

            foreach (var containerKeyAccount in ApplicationDataContainerAccounts.Containers.Keys)
                if (shouldCheckAccount) await CheckAccountAsync(containerKeyAccount);
                else AddUpdateAccountGroup(containerKeyAccount);

            if (!shouldCheckAccount) _ = GetRealTimeNotesFromApiForAllEnabledAsync();

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
        /// Create a GeeTest challenge.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <returns>A tuple. 1st item: the challenge; 2nd item: the GT.</returns>
        private async Task<(string, string)> CreateGeeTestChallenge(string cookies)
        {
            const string query = "is_high=false";
            var httpResponseBody = await _app.HttpClientH.GetAsync(cookies, true,
                $"{UrlBaseGeeTestChallengeServerCn}{query}", null, true, query);

            if (httpResponseBody is null)
            {
                Log.Warning("Failed to create a GeeTest challenge due to null HTTP response body.");
                return (null, null);
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning("Failed to parse the GeeTest challenge creation response body.");
                    Log.Information(httpResponseBody);
                    return (null, null);
                } // end if

                var returnCode = (int?)jsonNodeResponse[KeyReturnCode];

                if (returnCode is not ReturnCodeSuccess)
                {
                    Log.Warning(
                        $"Failed to create a GeeTest challenge (message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}).");
                    return (null, null);
                } // end if

                var data = jsonNodeResponse[KeyData];

                if (data is not null) return ((string)data[KeyChallenge], (string)data[KeyGt]);

                Log.Warning("Failed to get the GeeTest challenge creation data.");
                return (null, null);
            }
            catch (Exception exception)
            {
                Log.Error("Failed to parse the GeeTest challenge creation response body.");
                LogException(exception, httpResponseBody);
                return (null, null);
            } // end try...catch
        } // end method CreateGeeTestChallenge

        /// <summary>
        /// Get the account and its characters from the API.
        /// NOTE: Remember to change the account status to adding/updating.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="shouldGetAccount">A flag indicating if the specific account should be got from the API before getting the account's characters.</param>
        /// <returns>A list of characters, or <c>null</c> if the operation fails.</returns>
        public async Task<ImmutableList<Character>> GetAccountCharactersFromApiAsync(string containerKeyAccount,
            bool shouldGetAccount = true)
        {
            if (shouldGetAccount && !await GetAccountFromApiAsync(containerKeyAccount)) return null;

            return await GetCharactersFromApiAsync(containerKeyAccount);
        } // end method GetAccountCharactersFromApiAsync

        /// <summary>
        /// Get the specific account from the API. The method is used when the user adds/updates an account.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <returns>A tuple. 1st item: the account UID; 2nd item: the avatar; 3rd item: the nickname; 4th item: the return code.</returns>
        public async Task<(string, string, string, int?)> GetAccountFromApiAsync(string cookies, bool isServerCn)
        {
            var httpResponseBody = await _app.HttpClientH.GetAsync(cookies, isServerCn,
                isServerCn ? UrlAccountServerCn : UrlAccountServerGlobal);

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get the account from the API due to null HTTP response body (CN server: {isServerCn}).");
                return (string.Empty, string.Empty, string.Empty, null);
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the account response body (CN server: {isServerCn}):");
                    Log.Information(httpResponseBody);
                    return (string.Empty, string.Empty, string.Empty, null);
                } // end if

                var returnCode = (int?)jsonNodeResponse[KeyReturnCode];

                if (returnCode is not ReturnCodeSuccess)
                {
                    Log.Warning(
                        $"Failed to get the account from the specific API (CN server: {isServerCn}, message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}).");
                    return (string.Empty, string.Empty, string.Empty, returnCode);
                } // end if

                var userInfo = jsonNodeResponse[KeyData]?[KeyUserInfo];

                if (userInfo is null)
                {
                    Log.Warning($"Failed to get the user info (CN server: {isServerCn}).");
                    return (string.Empty, string.Empty, string.Empty, returnCode);
                } // end if

                var aUid = (string)userInfo[KeyUid];

                if (string.IsNullOrWhiteSpace(aUid))
                {
                    Log.Warning(
                        $"Failed to get the account UID from the user info (CN server: {isServerCn}).");
                    return (null, string.Empty, string.Empty, returnCode);
                } // end if

                var avatar = (string)userInfo[KeyAvatar];

                if (string.IsNullOrWhiteSpace(avatar))
                    Log.Warning(
                        $"Failed to get the avatar from the user info (CN server: {isServerCn}).");

                var nickname = (string)userInfo[KeyNickname];

                if (string.IsNullOrWhiteSpace(nickname))
                    Log.Warning(
                        $"Failed to get the nickname from the user info (CN server: {isServerCn}).");

                return (aUid, avatar, nickname, returnCode);
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"Failed to parse the account response body (CN server: {isServerCn}):");
                LogException(exception, httpResponseBody);
                return (string.Empty, string.Empty, string.Empty, null);
            } // end try...catch
        } // end method GetAccountFromApiAsync(string, string)

        /// <summary>
        /// Get the specific account from the API. The method is usually used before getting the account's characters from the API.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <returns>A flag indicating if getting the account's characters from the API can be safe to execute.</returns>
        private async Task<bool> GetAccountFromApiAsync(string containerKeyAccount)
        {
            if (!ValidateAccountContainerKey(containerKeyAccount)) return false;

            var propertySetAccount = ApplicationDataContainerAccounts.Containers[containerKeyAccount].Values;

            if (propertySetAccount[KeyStatus] as string is TagStatusExpired)
            {
                Log.Warning(
                    $"Failed to get the account from the API due to its expired status (account container key: {containerKeyAccount}).");
                return false;
            } // end if

            propertySetAccount[KeyTimeUpdateLast] = DateTimeOffset.UtcNow;

            var isServerCn = propertySetAccount[KeyServer] is TagServerCn;
            var httpResponseBody = await _app.HttpClientH.GetAsync(propertySetAccount[KeyCookies] as string,
                isServerCn,
                isServerCn ? UrlAccountServerCn : UrlAccountServerGlobal); // Send an HTTP GET request when ready.

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get the account from the API due to null HTTP response body (account container key: {containerKeyAccount}).");
                propertySetAccount[KeyStatus] = TagStatusFail;
                return true;
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the account response body (account container key: {containerKeyAccount}):");
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

                var aUid = (string)userInfo[KeyUid];

                if (aUid != propertySetAccount[KeyUid] as string)
                {
                    Log.Warning(
                        $"The account UID does not match (account container key: {containerKeyAccount}, UID got: {aUid}).");
                    propertySetAccount[KeyStatus] = TagStatusFail;
                    return false;
                } // end if

                var avatar = (string)userInfo[KeyAvatar];

                if (string.IsNullOrWhiteSpace(avatar))
                {
                    Log.Warning(
                        $"Failed to get the avatar from the user info (account container key: {containerKeyAccount}).");
                    propertySetAccount[KeyStatus] = TagStatusFail;
                }
                else propertySetAccount[KeyAvatar] = avatar;

                var nickname = (string)userInfo[KeyNickname];

                if (string.IsNullOrWhiteSpace(nickname))
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
                    $"Failed to parse the account response body (account container key: {containerKeyAccount}):");
                LogException(exception, httpResponseBody);
                propertySetAccount[KeyStatus] = TagStatusFail;
            } // end try...catch

            return true;
        } // end method GetAccountFromApiAsync(string)

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
            var httpResponseBody = await _app.HttpClientH.GetAsync(propertySetAccount[KeyCookies] as string,
                isServerCn,
                isServerCn ? UrlCharactersServerCn : UrlCharactersServerGlobal); // Send an HTTP GET request when ready.

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get characters from the API due to null HTTP response body (account container key: {containerKeyAccount}).");
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
                        $"Failed to parse the characters response body (account container key: {containerKeyAccount}):");
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
                    $"Failed to parse the characters response body (account container key: {containerKeyAccount}):");
                LogException(exception, httpResponseBody);
                propertySetAccount[KeyStatus] = TagStatusFail;
                return null;
            } // end try...catch
        } // end method GetCharactersFromApiAsync

        /// <summary>
        /// Get a local date and time string.
        /// </summary>
        /// <param name="dateTimeOffset">The struct containing the date, time, and offset.</param>
        /// <returns>The local date and time string.</returns>
        public string GetLocalDateTimeString(DateTimeOffset? dateTimeOffset)
        {
            if (dateTimeOffset is null) return AppFieldsHelper.Unknown;

            var cultureApplied = _app.SettingsH.CultureApplied;
            var dateTimeOffsetLocal = ((DateTimeOffset)dateTimeOffset).ToLocalTime();
            var resourceLoader = _app.SettingsH.ResLoader;
            var dateSimplified = dateTimeOffsetLocal.Date.Subtract(DateTimeOffset.Now.Date).Days switch
            {
                -1 => resourceLoader.GetString("Yesterday"),
                0 => resourceLoader.GetString("Today"),
                1 => resourceLoader.GetString("Tomorrow"),
                _ => null
            }; // Get the simplified date string when ready.

            return dateSimplified is null
                ? dateTimeOffsetLocal.ToString("g", cultureApplied)
                : $"{dateSimplified} {dateTimeOffsetLocal.ToString("t", cultureApplied)}";
        } // end method GetLocalDateTimeString

        /// <summary>
        /// Get the real-time notes.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <returns>A tuple. 1st item: the expeditions header; 2nd item: the expedition notes; 3rd item: the general notes; 4th item: the status; 5th item: the last update local time.</returns>
        public (RealTimeNote, ImmutableList<RealTimeNote>, ImmutableList<RealTimeNote>, string, string)
            GetRealTimeNotes(string containerKeyAccount,
                string containerKeyCharacter)
        {
            var resourceLoader = _app.SettingsH.ResLoader; // Get the resource loader first.
            var colonAndEstimated =
                $"{resourceLoader.GetString("Colon")}{resourceLoader.GetString("Estimated")} "; // The resource string for ": est. ".
            var commissionsDailyExplanation = AppFieldsHelper.Unknown;
            int? commissionsDailyFinished = null;
            int? commissionsDailyMax = null;
            int? currencyRealmCurrent = null;
            var currencyRealmExplanation = AppFieldsHelper.Unknown;
            int? currencyRealmMax = null;
            int? domainsTrounceDiscountsMax = null;
            int? domainsTrounceDiscountsRemaining = null;
            int? expeditionsCurrent = null;
            int? expeditionsMax = null;
            var realTimeNotesExpeditions = new List<RealTimeNote>(); // 2. Real-time notes' expeditions section.
            var realTimeNotesGeneral = new List<RealTimeNote>(); // 1. Real-time notes' general section.
            string realTimeNotesStatus = null;
            var realTimeNotesTimeLocalUpdateLast = AppFieldsHelper.Unknown;
            int? resinOriginalCurrent = null;
            var resinOriginalExplanation = AppFieldsHelper.Unknown;
            int? resinOriginalMax = null;
            var transformerParametricExplanation = AppFieldsHelper.Unknown;
            var transformerParametricStatus = AppFieldsHelper.Unknown;

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
                        realTimeNotesTimeLocalUpdateLast =
                            GetLocalDateTimeString(propertySetRealTimeNotes[KeyTimeUpdateLast] as DateTimeOffset?);

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
                                    $"{colonAndEstimated}{GetLocalDateTimeString(propertySetRealTimeNotes[KeyCurrencyRealmTimeRecovery] as DateTimeOffset?)}";

                            if (expeditionsCurrent is null || expeditionsMax is null)
                                realTimeNotesExpeditions.Add(new RealTimeNote
                                {
                                    Explanation = AppFieldsHelper.Unknown,
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
                                        .CreateContainer(i.ToString(), ApplicationDataCreateDisposition.Always)
                                        .Values; // Get the expedition property set first.
                                    var expeditionExplanation = resourceLoader.GetString("ExpeditionComplete");
                                    var expeditionStatus = propertySetExpedition[KeyStatus] as string;
                                    var urlBaseAvatarSideIcon =
                                        (string)applicationDataContainerAccount.Values[KeyServer] switch
                                        {
                                            TagServerCn => UrlBaseAvatarSideIconServerCn,
                                            TagServerGlobal => UrlBaseAvatarSideIconServerGlobal,
                                            _ => null
                                        };

                                    if (expeditionStatus is not ExpeditionStatusFinished)
                                        expeditionExplanation +=
                                            $"{colonAndEstimated}{GetLocalDateTimeString(propertySetExpedition[KeyTimeRemaining] as DateTimeOffset?)}";

                                    realTimeNotesExpeditions.Add(new RealTimeNote
                                    {
                                        Explanation = expeditionExplanation,
                                        Status = expeditionStatus,
                                        UriImage =
                                            propertySetExpedition[KeyAvatarSideIcon] is not string
                                                expeditionAvatarSideIcon || urlBaseAvatarSideIcon is null
                                                ? null
                                                : new Uri(
                                                    $"{urlBaseAvatarSideIcon}{expeditionAvatarSideIcon}{FileExtensionPng}")
                                    });
                                } // end for
                            } // end nested if...else

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
                                                $"{colonAndEstimated}{AppFieldsHelper.Unknown}";
                                            break;

                                        case true:
                                            transformerParametricStatus =
                                                resourceLoader.GetString("TransformerParametricReady");
                                            break;

                                        default:
                                            transformerParametricExplanation +=
                                                $"{colonAndEstimated}{GetLocalDateTimeString(propertySetRealTimeNotes[KeyTransformerParametricTimeRecovery] as DateTimeOffset?)}";
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
                                    $"{colonAndEstimated}{GetLocalDateTimeString(propertySetRealTimeNotes[KeyResinOriginalTimeRecovery] as DateTimeOffset?)}";
                        } // end if...else

                        if (realTimeNotesExpeditions.Count is 0)
                            realTimeNotesExpeditions.Add(new RealTimeNote
                            {
                                Explanation = resourceLoader.GetString("ExpeditionsNone"),
                                UriImage = null
                            });
                    } // end if...else
                } // end if...else
            } // end if

            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = resinOriginalExplanation,
                Status = GetRealTimeNoteStatus(resinOriginalCurrent, resinOriginalMax),
                Title = resourceLoader.GetString("ResinOriginal"),
                UriImage = new Uri(AppFieldsHelper.UriImageResinOriginal)
            }); // 1.1. Original Resin.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = currencyRealmExplanation,
                Status = GetRealTimeNoteStatus(currencyRealmCurrent, currencyRealmMax),
                Title = resourceLoader.GetString("CurrencyRealm"),
                UriImage = new Uri(AppFieldsHelper.UriImageCurrencyRealm)
            }); // 1.2. Realm Currency.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = commissionsDailyExplanation,
                Status = GetRealTimeNoteStatus(commissionsDailyFinished, commissionsDailyMax),
                Title = resourceLoader.GetString("CommissionsDaily"),
                UriImage = new Uri(AppFieldsHelper.UriImageCommissionsDaily)
            }); // 1.3. Daily commissions.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = resourceLoader.GetString("DomainsTrounceDiscountsExplanation"),
                Status = GetRealTimeNoteStatus(domainsTrounceDiscountsRemaining, domainsTrounceDiscountsMax),
                Title = resourceLoader.GetString("DomainsTrounceDiscounts"),
                UriImage = new Uri(AppFieldsHelper.UriImageDomainsTrounce)
            }); // 1.4. Trounce Domains discounts.
            realTimeNotesGeneral.Add(new RealTimeNote
            {
                Explanation = transformerParametricExplanation,
                Status = transformerParametricStatus,
                Title = resourceLoader.GetString("TransformerParametric"),
                UriImage = new Uri(AppFieldsHelper.UriImageTransformerParametric)
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
                realTimeNotesTimeLocalUpdateLast);
        } // end method GetRealTimeNotes

        /// <summary>
        /// Get the real-time notes from the API.
        /// </summary>
        /// <param name="challengeValidated">The validated GeeTest challenge.</param>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <param name="isStandalone">A flag indicating if the operation is standalone.</param>
        /// <returns>Void.</returns>
        private async Task GetRealTimeNotesFromApiAsync(string challengeValidated, string containerKeyAccount,
            string containerKeyCharacter,
            bool isStandalone = true)
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

            if (isStandalone) UidCharacterRealTimeNotesUpdated = string.Empty;

            var applicationDataContainerRealTimeNotes =
                applicationDataContainerCharacter.CreateContainer(ContainerKeyRealTimeNotes,
                    ApplicationDataCreateDisposition.Always);
            var propertySetAccount = applicationDataContainerAccount.Values;
            var propertySetRealTimeNotes = applicationDataContainerRealTimeNotes.Values;
            var uidCharacter = applicationDataContainerCharacter.Name;

            propertySetRealTimeNotes[KeyStatus] = TagStatusUpdating;
            propertySetRealTimeNotes[KeyTimeUpdateLast] = DateTimeOffset.UtcNow;

            if (propertySetAccount[KeyStatus] is TagStatusExpired)
            {
                Log.Warning(
                    $"Failed to get real-time notes from the API due to the specific account's expired status (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;

                if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;

                return;
            } // end if

            var region = applicationDataContainerCharacter.Values[KeyRegion] as string;

            if (region is null || !_regions.ContainsKey(region))
            {
                Log.Warning(
                    $"Invalid region (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}, region: {region}).");
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;

                if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;

                return;
            } // end if

            var cookies = propertySetAccount[KeyCookies] as string;
            var isServerCn = propertySetAccount[KeyServer] is TagServerCn;
            var query = $"role_id={uidCharacter}&server={region}";
            var urlBaseRealTimeNotes = isServerCn ? UrlBaseRealTimeNotesServerCn : UrlBaseRealTimeNotesServerGlobal;
            var httpResponseBody = await _app.HttpClientH.GetAsync(cookies, isServerCn,
                $"{urlBaseRealTimeNotes}{query}", challengeValidated, true,
                query); // Send an HTTP GET request when ready.

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to get real-time notes from the API due to null HTTP response body (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;

                if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;

                return;
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the real-time notes response body (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}):");
                    Log.Information(httpResponseBody);
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;

                    if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;

                    return;
                } // end if

                var returnCode = (int?)jsonNodeResponse[KeyReturnCode];

                // Get real-time notes from the specific API with a validated GeeTest challenge for a CN server account's character only if the validated challenge is not provided to avoid an endless iterated function.
                if (returnCode is ReturnCodeChallengeValidated && string.IsNullOrWhiteSpace(challengeValidated) &&
                    isServerCn)
                {
                    Log.Warning(
                        $"Failed to get real-time notes from the specific API because a validated GeeTest challenge is required (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");

                    foreach (var time in new[] { "1st", "2nd", "3rd" })
                    {
                        var (challenge, gt) = await CreateGeeTestChallenge(cookies);

                        if (string.IsNullOrWhiteSpace(challenge) || string.IsNullOrWhiteSpace(gt))
                        {
                            Log.Warning($"Invalid GeeTest challenge (challenge: {challenge}, GT: {gt}).");
                            continue;
                        } // end if

                        var validation = await TrySolveGeeTestChallenge(challenge, cookies, gt);

                        if (string.IsNullOrWhiteSpace(validation))
                        {
                            Log.Warning($"Invalid GeeTest challenge result (challenge: {challenge}, GT: {gt}).");
                            continue;
                        } // end if

                        challengeValidated = await ValidateGeeTestChallengeResult(challenge, cookies, validation);

                        if (string.IsNullOrWhiteSpace(challengeValidated))
                        {
                            Log.Warning(
                                $"Failed to get a validated GeeTest challenge for the {time} time (challenge: {challenge}, GT: {gt}, validation: {validation}).");
                            continue;
                        } // end if

                        await GetRealTimeNotesFromApiAsync(challengeValidated, containerKeyAccount,
                            containerKeyCharacter, isStandalone);
                        return;
                    } // end foreach
                } // end if

                if (returnCode is not ReturnCodeSuccess)
                {
                    Log.Warning(
                        $"Failed to get real-time notes from the specific API (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}, message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}).");
                    propertySetRealTimeNotes[KeyStatus] =
                        returnCode is ReturnCodeDisabled ? TagStatusDisabled : TagStatusFail;

                    if (returnCode is ReturnCodeLoginFail) propertySetAccount[KeyStatus] = TagStatusExpired;

                    if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;

                    return;
                } // end if

                var data = jsonNodeResponse[KeyData];

                if (data is null)
                {
                    Log.Warning(
                        $"Failed to get real-time notes data (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}).");
                    propertySetRealTimeNotes[KeyStatus] = TagStatusFail;

                    if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;

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
                            foreach (var match in GenerateAvatarSideIconRegex().Matches(expeditionAvatarSideIconRaw)
                                         .Cast<Match>())
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
                    $"Failed to parse the real-time notes response body (account container key: {containerKeyAccount}, character container key: {containerKeyCharacter}):");
                LogException(exception, httpResponseBody);
                propertySetRealTimeNotes[KeyStatus] = TagStatusFail;
            } // end try...catch

            if (isStandalone) UidCharacterRealTimeNotesUpdated = uidCharacter;
        } // end method GetRealTimeNotesFromApiAsync

        /// <summary>
        /// Generate the regular expression for the avatar side icon.
        /// </summary>
        /// <returns>The regular expression for the avatar side icon.</returns>
        [GeneratedRegex("(?<=(UI_AvatarIcon_Side_)).*(?=.png)")]
        private static partial Regex GenerateAvatarSideIconRegex(); // end method GenerateAvatarSideIconRegex

        /// <summary>
        /// Get the real-time notes from the API for all enabled characters.
        /// </summary>
        /// <returns>Void.</returns>
        private async Task GetRealTimeNotesFromApiForAllEnabledAsync()
        {
            UidCharacterRealTimeNotesUpdated = string.Empty;

            foreach (var keyValuePairAccount in ApplicationDataContainerAccounts.Containers)
            foreach (var keyValuePairCharacter in from keyValuePairCharacter in keyValuePairAccount.Value
                         .CreateContainer(ContainerKeyCharacters, ApplicationDataCreateDisposition.Always).Containers
                         .ToImmutableList()
                     let propertySetCharacter = keyValuePairCharacter.Value.Values
                     where propertySetCharacter[KeyIsEnabled] is true
                     select keyValuePairCharacter)
                await GetRealTimeNotesFromApiAsync(null, keyValuePairAccount.Key, keyValuePairCharacter.Key, false);

            UidCharacterRealTimeNotesUpdated = TagRealTimeNotesUpdatedCharactersAllEnabled;
        } // end method GetRealTimeNotesFromApiForAllEnabledAsync

        /// <summary>
        /// Get the real-time note's status with the current and max values.
        /// </summary>
        /// <param name="valueCurrent">The current value.</param>
        /// <param name="valueMax">The max value.</param>
        /// <returns>The real-time note's status.</returns>
        private string GetRealTimeNoteStatus(object valueCurrent, object valueMax)
        {
            var current = valueCurrent is null or not int ? AppFieldsHelper.Unknown : valueCurrent;
            var max = valueMax is null or not int ? AppFieldsHelper.Unknown : valueMax;

            return current is 0 && max is 0
                ? _app.SettingsH.ResLoader.GetString("RealTimeNotesStatusLocked")
                : current is AppFieldsHelper.Unknown && max is AppFieldsHelper.Unknown
                    ? AppFieldsHelper.Unknown
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
                ? AppFieldsHelper.Unknown
                : region;
        } // end method GetRegion

        /// <summary>
        /// Log the exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="httpResponseBody">The HTTP response body.</param>
        private static void LogException(Exception exception, string httpResponseBody)
        {
            App.LogException(exception);
            Log.Error($"  - Extra info: {httpResponseBody}");
        } // end method LogException

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
        /// Set the real-time notes dispatcher queue timer's interval.
        /// NOTE: The method will first initialise the timer if the timer is null.
        /// </summary>
        public void SetRealTimeNotesDispatcherQueueTimerInterval()
        {
            if (_dispatcherQueueTimerRealTimeNotes is null)
            {
                _dispatcherQueueTimerRealTimeNotes = DispatcherQueue.GetForCurrentThread().CreateTimer();
                _dispatcherQueueTimerRealTimeNotes.Tick +=
                    DispatcherQueueTimerRealTimeNotes_OnTick; // Add the tick event handler first.
            } // end if

            _dispatcherQueueTimerRealTimeNotes.Interval = TimeSpan.FromMinutes(
                _app.SettingsH.PropertySetSettings[SettingsHelper.KeyRealTimeNotesIntervalRefresh] as int? ??
                SettingsHelper.TagRealTimeNotesIntervalRefreshResinOriginal);

            if (!_dispatcherQueueTimerRealTimeNotes.IsRunning)
                _dispatcherQueueTimerRealTimeNotes.Start(); // The 1st tick occurs when the timer interval has elapsed.
        } // end method SetRealTimeNotesDispatcherTimerInterval

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

            if (shouldEnableCharacter)
                _ = GetRealTimeNotesFromApiAsync(null, containerKeyAccount, containerKeyCharacter);

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
        /// Try solving the GeeTest challenge.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="gt">The GT.</param>
        /// <returns>The GeeTest challenge result, or <c>null</c> if the operation fails.</returns>
        private async Task<string> TrySolveGeeTestChallenge(string challenge, string cookies, string gt)
        {
            var httpResponseBody =
                await _app.HttpClientH.GetAsync(cookies, true,
                    $"{UrlBaseGeeTestChallengeResultServerCn}challenge={challenge}&gt={gt}");

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to try solving the GeeTest challenge due to null HTTP response body (challenge: {challenge}, gt: {gt}).");
                return null;
            } // end if

            // The valid response body may not be a valid JSON string (e.g., ({"data": "XXX"})).
            var indexCurlyBracketLeft = httpResponseBody.IndexOf('{');
            var indexCurlyBracketRight = httpResponseBody.LastIndexOf('}');

            if (indexCurlyBracketLeft >= indexCurlyBracketRight)
            {
                Log.Warning($"Invalid GeeTest challenge result response body (challenge: {challenge}, gt: {gt}).");
                Log.Information(httpResponseBody);
                return null;
            } // end if

            var jsonResponse = httpResponseBody.Substring(indexCurlyBracketLeft,
                indexCurlyBracketRight - indexCurlyBracketLeft + 1);

            try
            {
                var jsonNodeResponse = JsonNode.Parse(jsonResponse);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the GeeTest challenge result response body (challenge: {challenge}, gt: {gt}).");
                    Log.Information(httpResponseBody);
                    return null;
                } // end if

                var status = (string)jsonNodeResponse[KeyStatus];

                if (status is not StatusSuccess)
                {
                    Log.Warning(
                        $"Failed to try solving the GeeTest challenge (challenge: {challenge}, error: {jsonNodeResponse[KeyError]}, gt: {gt}, status: {status}).");
                    return null;
                } // end if

                var data = jsonNodeResponse[KeyData];

                if (data is not null) return (string)data[KeyValidation];

                Log.Warning($"Failed to get the GeeTest challenge result data (challenge: {challenge}, gt: {gt}).");
                return null;
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"Failed to parse the GeeTest challenge result response body (challenge: {challenge}, gt: {gt}).");
                LogException(exception, httpResponseBody);
                return null;
            } // end try...catch
        } // end method TrySolveGeeTestChallenge

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

        /// <summary>
        /// Validate the GeeTest challenge result.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="validation">The validation.</param>
        /// <returns>The validated challenge.</returns>
        private async Task<string> ValidateGeeTestChallengeResult(string challenge, string cookies, string validation)
        {
            var httpResponseBody = await _app.HttpClientH.PostAsync(cookies, true,
                UrlGeeTestChallengeResultValidationServerCn,
                JsonContent.Create(new GeeTestChallengeResult(challenge, validation)), true);

            if (httpResponseBody is null)
            {
                Log.Warning(
                    $"Failed to validate the GeeTest challenge result due to null HTTP response body (challenge: {challenge}, validation: {validation}).");
                return null;
            } // end if

            try
            {
                var jsonNodeResponse = JsonNode.Parse(httpResponseBody);

                if (jsonNodeResponse is null)
                {
                    Log.Warning(
                        $"Failed to parse the GeeTest challenge result validation response body (challenge: {challenge}, validation: {validation}).");
                    Log.Information(httpResponseBody);
                    return null;
                } // end if

                var returnCode = (int?)jsonNodeResponse[KeyReturnCode];

                if (returnCode is not ReturnCodeSuccess)
                {
                    Log.Warning(
                        $"Failed to validate the GeeTest challenge result (challenge: {challenge}, message: {(string)jsonNodeResponse[KeyMessage]}, return code: {returnCode}, validation: {validation}).");
                    return null;
                } // end if

                var data = jsonNodeResponse[KeyData];

                if (data is not null) return (string)data[KeyChallenge];

                Log.Warning(
                    $"Failed to get the GeeTest challenge result validation data (challenge: {challenge}, validation: {validation}).");
                return null;
            }
            catch (Exception exception)
            {
                Log.Error(
                    $"Failed to parse the GeeTest challenge result validation response body (challenge: {challenge}, validation: {validation}).");
                LogException(exception, httpResponseBody);
                return null;
            } // end try...catch
        } // end method ValidateGeeTestChallengeResult

        #endregion Methods

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

        /// <summary>
        /// The UID of the character updating the real-time notes.
        /// NOTE: Expected values include an empty string, a string indicating all enabled characters, and the character UID.
        /// </summary>
        public string UidCharacterRealTimeNotesUpdated
        {
            get => _uidCharacterRealTimeNotesUpdated;
            set
            {
                if (_uidCharacterRealTimeNotesUpdated == value) return;

                _uidCharacterRealTimeNotesUpdated = value;
                OnPropertyChanged();
            } // end set
        } // end property UidCharacterRealTimeNotesUpdated

        #endregion Properties
    } // end class AccountsHelper
} // end namespace PaimonTray.Helpers