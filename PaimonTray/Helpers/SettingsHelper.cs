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
        /// The greeting notification setting key.
        /// </summary>
        public const string KeyGreetingNotification = "GreetingNotification";

        /// <summary>
        /// The language setting key.
        /// </summary>
        public const string KeyLanguage = "Language";

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
        /// Initialise the settings when initialising the app.
        /// </summary>
        public static void InitialiseSettings()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyGreetingNotification) ||
                ApplicationData.Current.LocalSettings.Values[KeyGreetingNotification] is not bool)
                InitialiseSetting(KeyGreetingNotification, "Greeting notification setting", true);

            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyLanguage) ||
                !new[] { TagLanguageEn, TagLanguageZhCn, TagSystem }.Contains(
                    ApplicationData.Current.LocalSettings.Values[KeyLanguage]))
                InitialiseSetting(KeyLanguage, "Language setting", TagSystem);

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