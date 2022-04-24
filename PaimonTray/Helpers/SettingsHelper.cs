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
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyLanguage) ||
                !new[] { TagLanguageEn, TagLanguageZhCn, TagSystem }.Contains(
                    ApplicationData.Current.LocalSettings.Values[KeyLanguage]))
            {
                ApplicationData.Current.LocalSettings.Values[KeyLanguage] = TagSystem;
                Log.Information("Language setting initialised to default.");
            } // end if

            // ReSharper disable once InvertIf
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyTheme) ||
                !new[] { TagSystem, TagThemeDark, TagThemeLight }.Contains(
                    ApplicationData.Current.LocalSettings.Values[KeyTheme]))
            {
                ApplicationData.Current.LocalSettings.Values[KeyTheme] = TagSystem;
                Log.Information("Theme setting initialised to default.");
            } // end if
        } // end method InitialiseSettings

        #endregion Methods
    } // end class SettingsHelper
} // end namespace PaimonTray.Helpers