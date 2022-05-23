using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System.Linq;
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
        public const string ContainerKeySettings = "settings";

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
        /// The key for the default server.
        /// </summary>
        public const string KeyServerDefault = "serverDefault";

        /// <summary>
        /// The theme key.
        /// </summary>
        public const string KeyTheme = "theme";

        /// <summary>
        /// The English language tag.
        /// </summary>
        public const string TagLanguageEn = "en";

        /// <summary>
        /// The simplified Chinese language tag.
        /// </summary>
        public const string TagLanguageZhCn = "zh-CN";

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

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The language selection applied in this app lifecycle (the changes will be applied after app restart).
        /// </summary>
        public string LanguageSelectionApplied { get; private set; }

        /// <summary>
        /// The settings property set.
        /// </summary>
        public IPropertySet PropertySetSettings { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the settings helper.
        /// </summary>
        public SettingsHelper()
        {
            _app = Application.Current as App;
            PropertySetSettings = ApplicationData.Current.LocalSettings
                .CreateContainer(ContainerKeySettings, ApplicationDataCreateDisposition.Always).Values;

            InitialiseSettings();
        } // end constructor SettingsHelper

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Apply the selection of the main window's top navigation pane.
        /// </summary>
        public void ApplyMainWindowTopNavigationPaneSelection()
        {
            _app.WindowsH.GetMainWindow().MainWinViewModel.NavViewPaneDisplayMode =
                DecideMainWindowNavigationViewPaneDisplayMode();
        } // end method ApplyMainWindowTopNavigationPaneSelection

        /// <summary>
        /// Apply the theme selection.
        /// </summary>
        public void ApplyThemeSelection()
        {
            foreach (var existingWindow in _app.WindowsH.ExistingWindowList)
                ((FrameworkElement)existingWindow.Content).RequestedTheme = GetTheme();
        } // end method ApplyThemeSelection

        /// <summary>
        /// Decide the main window's navigation view's pane display mode.
        /// </summary>
        /// <returns>The main window's navigation view's pane display mode.</returns>
        public NavigationViewPaneDisplayMode DecideMainWindowNavigationViewPaneDisplayMode()
        {
            return (bool)PropertySetSettings[KeyMainWindowTopNavigationPane]
                ? NavigationViewPaneDisplayMode.Top
                : NavigationViewPaneDisplayMode.LeftCompact;
        } // end method DecideMainWindowNavigationViewPaneDisplayMode

        /// <summary>
        /// Get the theme.
        /// </summary>
        /// <returns>The theme.</returns>
        public ElementTheme GetTheme()
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
            PropertySetSettings[key] = value;
            Log.Information($"{name} initialised to default.");
        } // end method InitialiseSetting

        /// <summary>
        /// Initialise the settings when initialising the app. It should be invoked earlier than any other operations on the settings.
        /// </summary>
        public void InitialiseSettings()
        {
            if (!PropertySetSettings.ContainsKey(KeyLanguage) ||
                !new[] { TagLanguageEn, TagLanguageZhCn, TagSystem }.Contains(PropertySetSettings[KeyLanguage]))
                InitialiseSetting(KeyLanguage, "Language setting", TagSystem);

            if (!PropertySetSettings.ContainsKey(KeyLoginAlternativeAlways) ||
                PropertySetSettings[KeyLoginAlternativeAlways] is not bool)
                InitialiseSetting(KeyLoginAlternativeAlways,
                    "The setting for always using the alternative login method", false);

            if (!PropertySetSettings.ContainsKey(KeyMainWindowShowWhenAppStarts) ||
                PropertySetSettings[KeyMainWindowShowWhenAppStarts] is not bool)
                InitialiseSetting(KeyMainWindowShowWhenAppStarts,
                    "The setting for showing the main window when the app starts", false);

            if (!PropertySetSettings.ContainsKey(KeyMainWindowTopNavigationPane) ||
                PropertySetSettings[KeyMainWindowTopNavigationPane] is not bool)
                InitialiseSetting(KeyMainWindowTopNavigationPane,
                    "The setting for the main window's top navigation pane", false);

            if (!PropertySetSettings.ContainsKey(KeyNotificationClear) ||
                PropertySetSettings[KeyNotificationClear] is not bool)
                InitialiseSetting(KeyNotificationClear, "The setting for clearing notifications when exiting the app",
                    true);

            if (!PropertySetSettings.ContainsKey(KeyNotificationGreeting) ||
                PropertySetSettings[KeyNotificationGreeting] is not bool)
                InitialiseSetting(KeyNotificationGreeting, "Greeting notification setting", true);

            if (!PropertySetSettings.ContainsKey(KeyServerDefault) ||
                !new[] { AccountsHelper.TagServerCn, AccountsHelper.TagServerGlobal }.Contains(
                    PropertySetSettings[KeyServerDefault]))
                InitialiseSetting(KeyServerDefault, "The setting for the default server", AccountsHelper.TagServerCn);

            if (!PropertySetSettings.ContainsKey(KeyTheme) ||
                !new[] { TagSystem, TagThemeDark, TagThemeLight }.Contains(PropertySetSettings[KeyTheme]))
                InitialiseSetting(KeyTheme, "Theme setting", TagSystem);

            // Apply the language selection.
            LanguageSelectionApplied = PropertySetSettings[KeyLanguage] as string;
            ApplicationLanguages.PrimaryLanguageOverride =
                LanguageSelectionApplied == TagSystem ? string.Empty : LanguageSelectionApplied;
        } // end method InitialiseSettings

        #endregion Methods
    } // end class SettingsHelper
} // end namespace PaimonTray.Helpers