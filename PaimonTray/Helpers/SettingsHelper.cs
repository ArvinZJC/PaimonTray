using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Views;
using Serilog;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The settings helper.
    /// </summary>
    public class SettingsHelper
    {
        #region Constants

        /// <summary>
        /// The settings container key.
        /// </summary>
        private const string ContainerKeySettings = "settings";

        /// <summary>
        /// The default of the setting for checking and refreshing all accounts when the app starts.
        /// </summary>
        public const bool DefaultAccountGroupsCheckRefreshWhenAppStarts = true;

        /// <summary>
        /// The default of the setting for always using the alternative login method.
        /// </summary>
        public const bool DefaultLoginAlternativeAlways = false;

        /// <summary>
        /// The default of the main window's navigation view's pane display mode.
        /// </summary>
        public const NavigationViewPaneDisplayMode DefaultMainWindowNavigationViewPaneDisplayMode =
            NavigationViewPaneDisplayMode.LeftCompact;

        /// <summary>
        /// The default of the setting for showing the main window when the app starts.
        /// </summary>
        public const bool DefaultMainWindowShowWhenAppStarts = false;

        /// <summary>
        /// The default of the setting for the main window's top navigation pane.
        /// </summary>
        public const bool DefaultMainWindowTopNavigationPane = false;

        /// <summary>
        /// The default of the setting for clearing notifications when the app exits.
        /// </summary>
        public const bool DefaultNotificationClear = true;

        /// <summary>
        /// The default of the greeting notification setting.
        /// </summary>
        public const bool DefaultNotificationGreeting = true;

        /// <summary>
        /// The key for checking and refreshing all accounts when the app starts.
        /// </summary>
        public const string KeyAccountGroupsCheckRefreshWhenAppStarts = "accountGroupsCheckRefreshWhenAppStarts";

        /// <summary>
        /// The language key.
        /// </summary>
        public const string KeyLanguage = "language";

        /// <summary>
        /// The key for always using the alternative login method.
        /// </summary>
        public const string KeyLoginAlternativeAlways = "loginAlternativeAlways";

        /// <summary>
        /// The key for showing the main window when the app starts.
        /// </summary>
        public const string KeyMainWindowShowWhenAppStarts = "mainWindowShowWhenAppStarts";

        /// <summary>
        /// The key for the main window's top navigation pane.
        /// </summary>
        public const string KeyMainWindowTopNavigationPane = "mainWindowTopNavigationPane";

        /// <summary>
        /// The key for clearing notifications when the app exits.
        /// </summary>
        public const string KeyNotificationClear = "notificationClear";

        /// <summary>
        /// The greeting notification key.
        /// </summary>
        public const string KeyNotificationGreeting = "notificationGreeting";

        /// <summary>
        /// The real-time notes refresh interval key.
        /// </summary>
        public const string KeyRealTimeNotesIntervalRefresh = "realTimeNotesIntervalRefresh";

        /// <summary>
        /// The key for the default server.
        /// </summary>
        public const string KeyServerDefault = "serverDefault";

        /// <summary>
        /// The theme key.
        /// </summary>
        public const string KeyTheme = "theme";

        /// <summary>
        /// The English (United Kingdom) language tag.
        /// </summary>
        public const string TagLanguageEnGb = "en-GB";

        /// <summary>
        /// The English (United States) language tag.
        /// </summary>
        public const string TagLanguageEnUs = "en-US";

        /// <summary>
        /// The Indonesian (Indonesia) language tag.
        /// </summary>
        public const string TagLanguageIdId = "id-ID";

        /// <summary>
        /// The Chinese (Simplified, China) language tag.
        /// </summary>
        public const string TagLanguageZhHansCn = "zh-Hans-CN";

        /// <summary>
        /// The real-time notes refresh interval Other Option 1 tag.
        /// </summary>
        public const int TagRealTimeNotesIntervalRefreshOptionOther1 = 20; // 8 × 2.5

        /// <summary>
        /// The real-time notes refresh interval Other Option 2 tag.
        /// </summary>
        public const int TagRealTimeNotesIntervalRefreshOptionOther2 = 30; // 8 × 3.75

        /// <summary>
        /// The real-time notes refresh interval Other Option 3 tag.
        /// </summary>
        public const int TagRealTimeNotesIntervalRefreshOptionOther3 = 40; // 8 × 5

        /// <summary>
        /// The real-time notes refresh interval Other Option 4 tag.
        /// </summary>
        public const int TagRealTimeNotesIntervalRefreshOptionOther4 = 60; // 8 × 7.5

        /// <summary>
        /// The tag for the real-time notes refresh interval which is equal to the Original Resin replenishment time.
        /// </summary>
        public const int TagRealTimeNotesIntervalRefreshResinOriginal = 8;

        /// <summary>
        /// The system default tag.
        /// </summary>
        public const string TagSystem = "system";

        /// <summary>
        /// The dark theme tag.
        /// </summary>
        public const string TagThemeDark = "dark";

        /// <summary>
        /// The light theme tag.
        /// </summary>
        public const string TagThemeLight = "light";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initialise the settings helper.
        /// NOTE: Must be done as early as possible for the app.
        /// </summary>
        public SettingsHelper()
        {
            _app = Application.Current as App;
            PropertySetSettings = ApplicationData.Current.LocalSettings
                .CreateContainer(ContainerKeySettings, ApplicationDataCreateDisposition.Always).Values;

            InitialiseSettings(); // Initialise the settings first.
            ApplyLanguageSelection();
        } // end constructor SettingsHelper

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~SettingsHelper()
        {
            _app = null;
        } // end destructor

        #endregion Destructor

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Apply the language selection.
        /// </summary>
        private void ApplyLanguageSelection()
        {
            LanguageSelectionApplied = PropertySetSettings[KeyLanguage] as string;

            var languagePrimary = LanguageSelectionApplied is TagSystem ? string.Empty : LanguageSelectionApplied;

            ApplicationLanguages.PrimaryLanguageOverride = languagePrimary;
            ResLoader = ResourceLoader.GetForViewIndependentUse();

            var languageApplied = ResLoader.GetString("LanguageApplied");

            CultureApplied =
                new CultureInfo(
                    languageApplied is TagLanguageEnGb or TagLanguageEnUs or TagLanguageIdId or TagLanguageZhHansCn
                        ? languageApplied
                        : languagePrimary == string.Empty
                            ? TagLanguageEnUs
                            : languagePrimary ?? TagLanguageEnUs);
        } // end method ApplyLanguageSelection

        /// <summary>
        /// Apply the selection of the main window's top navigation pane.
        /// </summary>
        public void ApplyMainWindowTopNavigationPaneSelection()
        {
            if (_app.WindowsH.GetExistingMainWindow()?.Win is not MainWindow mainWindow) return;

            mainWindow.MainWinViewModel.NavViewPaneDisplayMode = DecideMainWindowNavigationViewPaneDisplayMode();
        } // end method ApplyMainWindowTopNavigationPaneSelection

        /// <summary>
        /// Apply the theme selection.
        /// </summary>
        public void ApplyThemeSelection()
        {
            foreach (var existingWindow in _app.WindowsH.ExistingWindows)
            {
                var theme = GetTheme();

                if (existingWindow.SystemBackdropConfig is not null)
                    existingWindow.SystemBackdropConfig.Theme = theme switch
                    {
                        ElementTheme.Dark => SystemBackdropTheme.Dark,
                        ElementTheme.Default => SystemBackdropTheme.Default,
                        ElementTheme.Light => SystemBackdropTheme.Light,
                        _ => SystemBackdropTheme.Default
                    };

                ((FrameworkElement)existingWindow.Win.Content).RequestedTheme = theme;
            } // end foreach
        } // end method ApplyThemeSelection

        /// <summary>
        /// Decide the main window's navigation view's pane display mode.
        /// </summary>
        /// <returns>The main window's navigation view's pane display mode.</returns>
        public NavigationViewPaneDisplayMode DecideMainWindowNavigationViewPaneDisplayMode()
        {
            return PropertySetSettings[KeyMainWindowTopNavigationPane] is true
                ? NavigationViewPaneDisplayMode.Top
                : DefaultMainWindowNavigationViewPaneDisplayMode;
        } // end method DecideMainWindowNavigationViewPaneDisplayMode

        /// <summary>
        /// Get the theme.
        /// </summary>
        /// <returns>The theme.</returns>
        private ElementTheme GetTheme()
        {
            var themeSelection = PropertySetSettings[KeyTheme] as string;

            switch (themeSelection)
            {
                case TagSystem:
                    return ElementTheme.Default;
                case TagThemeDark:
                    return ElementTheme.Dark;
                case TagThemeLight:
                    return ElementTheme.Light;
                default:
                    Log.Warning($"Invalid theme selection ({themeSelection}).");
                    return ElementTheme.Default;
            } // end switch-case
        } // end method GetTheme

        /// <summary>
        /// Initialise a setting.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="name">The setting name with the 1st letter capitalised.</param>
        /// <param name="value">The setting value.</param>
        private void InitialiseSetting(string key, string name, object value)
        {
            if (key is null)
            {
                Log.Warning($"Null setting key (setting name: {name}).");
                return;
            } // end if

            PropertySetSettings[key] = value;
            Log.Information($"{name} initialised to default.");
        } // end method InitialiseSetting

        /// <summary>
        /// Initialise the settings when initialising the app.
        /// NOTE: Must be invoked earlier than any other operations on the settings.
        /// </summary>
        private void InitialiseSettings()
        {
            if (!PropertySetSettings.ContainsKey(KeyAccountGroupsCheckRefreshWhenAppStarts) ||
                PropertySetSettings[KeyAccountGroupsCheckRefreshWhenAppStarts] is not bool)
                InitialiseSetting(KeyAccountGroupsCheckRefreshWhenAppStarts,
                    "The setting for checking and refreshing all accounts when the app starts",
                    DefaultAccountGroupsCheckRefreshWhenAppStarts);

            if (!PropertySetSettings.ContainsKey(KeyLanguage))
            {
                var language = PropertySetSettings[KeyLanguage] as string;

                if (language is not TagLanguageEnGb and not TagLanguageEnUs and not TagLanguageIdId
                    and not TagLanguageZhHansCn
                    and not TagSystem)
                    InitialiseSetting(KeyLanguage, "Language setting", TagSystem);
            } // end if

            if (!PropertySetSettings.ContainsKey(KeyLoginAlternativeAlways) ||
                PropertySetSettings[KeyLoginAlternativeAlways] is not bool)
                InitialiseSetting(KeyLoginAlternativeAlways,
                    "The setting for always using the alternative login method", DefaultLoginAlternativeAlways);

            if (!PropertySetSettings.ContainsKey(KeyMainWindowShowWhenAppStarts) ||
                PropertySetSettings[KeyMainWindowShowWhenAppStarts] is not bool)
                InitialiseSetting(KeyMainWindowShowWhenAppStarts,
                    "The setting for showing the main window when the app starts", DefaultMainWindowShowWhenAppStarts);

            if (!PropertySetSettings.ContainsKey(KeyMainWindowTopNavigationPane) ||
                PropertySetSettings[KeyMainWindowTopNavigationPane] is not bool)
                InitialiseSetting(KeyMainWindowTopNavigationPane,
                    "The setting for the main window's top navigation pane", DefaultMainWindowTopNavigationPane);

            if (!PropertySetSettings.ContainsKey(KeyNotificationClear) ||
                PropertySetSettings[KeyNotificationClear] is not bool)
                InitialiseSetting(KeyNotificationClear, "The setting for clearing notifications when the app exits",
                    DefaultNotificationClear);

            if (!PropertySetSettings.ContainsKey(KeyNotificationGreeting) ||
                PropertySetSettings[KeyNotificationGreeting] is not bool)
                InitialiseSetting(KeyNotificationGreeting, "Greeting notification setting",
                    DefaultNotificationGreeting);

            if (!PropertySetSettings.ContainsKey(KeyRealTimeNotesIntervalRefresh))
            {
                var realTimeNotesIntervalRefresh = PropertySetSettings[KeyRealTimeNotesIntervalRefresh] as int?;

                if (realTimeNotesIntervalRefresh is not TagRealTimeNotesIntervalRefreshOptionOther1
                    and not TagRealTimeNotesIntervalRefreshOptionOther2
                    and not TagRealTimeNotesIntervalRefreshOptionOther3
                    and not TagRealTimeNotesIntervalRefreshOptionOther4
                    and not TagRealTimeNotesIntervalRefreshResinOriginal)
                    InitialiseSetting(KeyRealTimeNotesIntervalRefresh, "Real-time notes refresh interval setting",
                        TagRealTimeNotesIntervalRefreshResinOriginal);
            } // end if

            if (!PropertySetSettings.ContainsKey(KeyServerDefault))
            {
                var serverDefault = PropertySetSettings[KeyServerDefault] as string;

                if (serverDefault is not AccountsHelper.TagServerCn and not AccountsHelper.TagServerGlobal)
                    InitialiseSetting(KeyServerDefault, "The setting for the default server",
                        AccountsHelper.TagServerCn);
            } // end if

            if (PropertySetSettings.ContainsKey(KeyTheme)) return;

            var theme = PropertySetSettings[KeyTheme] as string;

            if (theme is not TagSystem and not TagThemeDark and not TagThemeLight)
                InitialiseSetting(KeyTheme, "Theme setting", TagSystem);
        } // end method InitialiseSettings

        #endregion Methods

        #region Properties

        /// <summary>
        /// The culture applied in this app lifecycle due to the language selection applied.
        /// </summary>
        public CultureInfo CultureApplied { get; private set; }

        /// <summary>
        /// The language selection applied in this app lifecycle (the changes will be applied after app restart).
        /// </summary>
        public string LanguageSelectionApplied { get; private set; }

        /// <summary>
        /// The settings property set.
        /// </summary>
        public IPropertySet PropertySetSettings { get; }

        public ResourceLoader ResLoader { get; private set; }

        #endregion Properties
    } // end class SettingsHelper
} // end namespace PaimonTray.Helpers