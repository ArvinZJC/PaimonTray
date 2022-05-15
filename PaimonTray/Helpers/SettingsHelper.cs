using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Serilog;
using System.Linq;
using Windows.Storage;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The settings helper.
    /// </summary>
    internal class SettingsHelper
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

        #region Methods

        /// <summary>
        /// Apply the selection of the main window's top navigation pane.
        /// </summary>
        public static void ApplyMainWindowTopNavigationPaneSelection()
        {
            WindowsHelper.ShowMainWindow().MainWinViewModel.NavViewPaneDisplayMode =
                DecideMainWindowNavigationViewPaneDisplayMode();
        } // end method ApplyMainWindowTopNavigationPaneSelection

        /// <summary>
        /// Apply the theme selection.
        /// </summary>
        public static void ApplyThemeSelection()
        {
            foreach (var existingWindow in WindowsHelper.ExistingWindowList)
                ((FrameworkElement)existingWindow.Content).RequestedTheme = GetTheme();
        } // end method ApplyThemeSelection

        /// <summary>
        /// Decide the main window's navigation view's pane display mode.
        /// </summary>
        /// <returns>The main window's navigation view's pane display mode.</returns>
        public static NavigationViewPaneDisplayMode DecideMainWindowNavigationViewPaneDisplayMode()
        {
            return (bool)ApplicationData.Current.LocalSettings.Containers[ContainerKeySettings]
                .Values[KeyMainWindowTopNavigationPane]
                ? NavigationViewPaneDisplayMode.Top
                : NavigationViewPaneDisplayMode.LeftCompact;
        } // end method DecideMainWindowNavigationViewPaneDisplayMode

        /// <summary>
        /// Get the theme.
        /// </summary>
        /// <returns>The theme.</returns>
        public static ElementTheme GetTheme()
        {
            return ApplicationData.Current.LocalSettings.Containers[ContainerKeySettings].Values[KeyTheme] switch
            {
                TagThemeDark => ElementTheme.Dark,
                TagThemeLight => ElementTheme.Light,
                _ => ElementTheme.Default
            };
        } // end method GetTheme

        /// <summary>
        /// Initialise the settings when initialising the app. It should be invoked earlier than any other operations on the settings.
        /// </summary>
        public static void InitialiseSettings()
        {
            var propertySetSettings = ApplicationData.Current.LocalSettings
                .CreateContainer(ContainerKeySettings, ApplicationDataCreateDisposition.Always).Values;

            if (!propertySetSettings.ContainsKey(KeyLanguage) ||
                !new[] { TagLanguageEn, TagLanguageZhCn, TagSystem }.Contains(propertySetSettings[KeyLanguage]))
                InitialiseSetting(KeyLanguage, "Language setting", TagSystem);

            if (!propertySetSettings.ContainsKey(KeyLoginAlternativeAlways) ||
                propertySetSettings[KeyLoginAlternativeAlways] is not bool)
                InitialiseSetting(KeyLoginAlternativeAlways,
                    "The setting for always using the alternative login method", false);

            if (!propertySetSettings.ContainsKey(KeyMainWindowTopNavigationPane) ||
                propertySetSettings[KeyMainWindowTopNavigationPane] is not bool)
                InitialiseSetting(KeyMainWindowTopNavigationPane,
                    "The setting for the main window's top navigation pane", false);

            if (!propertySetSettings.ContainsKey(KeyNotificationClear) ||
                propertySetSettings[KeyNotificationClear] is not bool)
                InitialiseSetting(KeyNotificationClear, "The setting for clearing notifications when exiting the app",
                    true);

            if (!propertySetSettings.ContainsKey(KeyNotificationGreeting) ||
                propertySetSettings[KeyNotificationGreeting] is not bool)
                InitialiseSetting(KeyNotificationGreeting, "Greeting notification setting", true);

            if (!propertySetSettings.ContainsKey(KeyServerDefault) ||
                !new[] { AccountsHelper.TagServerCn, AccountsHelper.TagServerGlobal }.Contains(
                    propertySetSettings[KeyServerDefault]))
                InitialiseSetting(KeyServerDefault, "The setting for the default server", AccountsHelper.TagServerCn);

            // ReSharper disable once InvertIf
            if (!propertySetSettings.ContainsKey(KeyTheme) ||
                !new[] { TagSystem, TagThemeDark, TagThemeLight }.Contains(propertySetSettings[KeyTheme]))
                InitialiseSetting(KeyTheme, "Theme setting", TagSystem);
        } // end method InitialiseSettings

        /// <summary>
        /// Initialise a setting.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="name">The setting name with the 1st letter capitalised.</param>
        /// <param name="value">The setting value.</param>
        private static void InitialiseSetting(string key, string name, object value)
        {
            ApplicationData.Current.LocalSettings.Containers[ContainerKeySettings].Values[key] = value;
            Log.Information($"{name} initialised to default.");
        } // end method InitialiseSetting

        #endregion Methods
    } // end class SettingsHelper
} // end namespace PaimonTray.Helpers