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
        /// The theme setting key.
        /// </summary>
        public const string KeyTheme = "Theme";

        /// <summary>
        /// The dark theme option key.
        /// </summary>
        public const string TagThemeDark = "Dark";

        /// <summary>
        /// The light theme option key.
        /// </summary>
        public const string TagThemeLight = "Light";

        /// <summary>
        /// The system theme option tag.
        /// </summary>
        public const string TagThemeSystem = "System";

        #endregion Constants

        #region Methods

        /// <summary>
        /// Initialise the settings.
        /// </summary>
        public static void InitialiseSettings()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey(KeyTheme) ||
                !new[] { TagThemeDark, TagThemeLight, TagThemeSystem }.Contains(
                    ApplicationData.Current.LocalSettings.Values[KeyTheme] as string))
            {
                ApplicationData.Current.LocalSettings.Values[KeyTheme] = TagThemeSystem;
                Log.Information("Theme setting initialised to default.");
            } // end if
        } // end method InitialiseSettings

        #endregion Methods
    } // end class SettingsHelper
} // end namespace PaimonTray.Helpers