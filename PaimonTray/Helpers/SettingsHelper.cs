using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Views;
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
        /// The language setting key.
        /// </summary>
        public const string KeyLanguage = "language";

        /// <summary>
        /// The setting key for the main window's top navigation pane.
        /// </summary>
        public const string KeyMainWindowTopNavigationPane = "mainWindowTopNavigationPane";

        /// <summary>
        /// The greeting notification setting key.
        /// </summary>
        public const string KeyNotificationGreeting = "notificationGreeting";

        /// <summary>
        /// The theme setting key.
        /// </summary>
        public const string KeyTheme = "theme";

        /// <summary>
        /// The English language option tag.
        /// </summary>
        public const string TagLanguageEn = "en";

        /// <summary>
        /// The simplified Chinese language option tag.
        /// </summary>
        public const string TagLanguageZhCn = "zh-CN";

        /// <summary>
        /// The system default option tag.
        /// </summary>
        public const string TagSystem = "system";

        /// <summary>
        /// The dark theme option tag.
        /// </summary>
        public const string TagThemeDark = "dark";

        /// <summary>
        /// The light theme option tag.
        /// </summary>
        public const string TagThemeLight = "light";

        #endregion Constants

        #region Methods

        /// <summary>
        /// Apply the selection of the main window's top navigation pane.
        /// </summary>
        public static void ApplyMainWindowTopNavigationPaneSelection()
        {
            foreach (var existingWindow in WindowsHelper.ExistingWindowList.Where(existingWindow =>
                         existingWindow is MainWindow))
                ((MainWindow)existingWindow).MainWinViewModel.NavViewPaneDisplayMode =
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
            var applicationDataContainerSettings = ApplicationData.Current.LocalSettings.CreateContainer(
                ContainerKeySettings,
                ApplicationDataCreateDisposition.Always);

            if (!applicationDataContainerSettings.Values.ContainsKey(KeyNotificationGreeting) ||
                applicationDataContainerSettings.Values[KeyNotificationGreeting] is not bool)
                InitialiseSetting(KeyNotificationGreeting, "Greeting notification setting", true);

            if (!applicationDataContainerSettings.Values.ContainsKey(KeyLanguage) ||
                !new[] { TagLanguageEn, TagLanguageZhCn, TagSystem }.Contains(
                    applicationDataContainerSettings.Values[KeyLanguage]))
                InitialiseSetting(KeyLanguage, "Language setting", TagSystem);

            if (!applicationDataContainerSettings.Values.ContainsKey(KeyMainWindowTopNavigationPane) ||
                applicationDataContainerSettings.Values[KeyMainWindowTopNavigationPane] is not bool)
                InitialiseSetting(KeyMainWindowTopNavigationPane,
                    "The setting for configuring the main window's top navigation pane", false);

            // ReSharper disable once InvertIf
            if (!applicationDataContainerSettings.Values.ContainsKey(KeyTheme) ||
                !new[] { TagSystem, TagThemeDark, TagThemeLight }.Contains(
                    applicationDataContainerSettings.Values[KeyTheme]))
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