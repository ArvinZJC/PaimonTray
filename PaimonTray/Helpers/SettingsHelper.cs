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
        /// The language setting key.
        /// </summary>
        public const string KeyLanguage = "Language";

        /// <summary>
        /// The setting key for the main window's top navigation pane.
        /// </summary>
        public const string KeyMainWindowTopNavigationPane = "MainWindowTopNavigationPane";

        /// <summary>
        /// The greeting notification setting key.
        /// </summary>
        public const string KeyNotificationGreeting = "NotificationGreeting";

        /// <summary>
        /// The theme setting key.
        /// </summary>
        public const string KeyTheme = "Theme";

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
        public const string TagSystem = "System";

        /// <summary>
        /// The dark theme option tag.
        /// </summary>
        public const string TagThemeDark = "Dark";

        /// <summary>
        /// The light theme option tag.
        /// </summary>
        public const string TagThemeLight = "Light";

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
                ((FrameworkElement)existingWindow.Content).RequestedTheme =
                    ApplicationData.Current.LocalSettings.Values[KeyTheme] switch
                    {
                        TagThemeDark => ElementTheme.Dark,
                        TagThemeLight => ElementTheme.Light,
                        _ => ElementTheme.Default
                    };
        } // end method ApplyThemeSelection

        /// <summary>
        /// Decide the main window's navigation view's pane display mode.
        /// </summary>
        /// <returns>The main window's navigation view's pane display mode.</returns>
        public static NavigationViewPaneDisplayMode DecideMainWindowNavigationViewPaneDisplayMode()
        {
            return (bool)ApplicationData.Current.LocalSettings.Values[KeyMainWindowTopNavigationPane]
                ? NavigationViewPaneDisplayMode.Top
                : NavigationViewPaneDisplayMode.LeftCompact;
        } // end method DecideMainWindowNavigationViewPaneDisplayMode

        /// <summary>
        /// Initialise the settings when initialising the app.
        /// </summary>
        public static void InitialiseSettings()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyNotificationGreeting) ||
                ApplicationData.Current.LocalSettings.Values[KeyNotificationGreeting] is not bool)
                InitialiseSetting(KeyNotificationGreeting, "Greeting notification setting", true);

            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyLanguage) ||
                !new[] { TagLanguageEn, TagLanguageZhCn, TagSystem }.Contains(
                    ApplicationData.Current.LocalSettings.Values[KeyLanguage]))
                InitialiseSetting(KeyLanguage, "Language setting", TagSystem);

            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyMainWindowTopNavigationPane) ||
                ApplicationData.Current.LocalSettings.Values[KeyMainWindowTopNavigationPane] is not bool)
                InitialiseSetting(KeyMainWindowTopNavigationPane,
                    "The setting for configuring the main window's top navigation pane", false);

            // ReSharper disable once InvertIf
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyTheme) ||
                !new[] { TagSystem, TagThemeDark, TagThemeLight }.Contains(
                    ApplicationData.Current.LocalSettings.Values[KeyTheme]))
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
            ApplicationData.Current.LocalSettings.Values[key] = value;
            Log.Information($"{name} initialised to default.");
        } // end method InitialiseSetting

        #endregion Methods
    } // end class SettingsHelper
} // end namespace PaimonTray.Helpers