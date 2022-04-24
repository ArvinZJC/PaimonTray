using Microsoft.UI.Xaml;
using Windows.Storage;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The themes helper.
    /// </summary>
    internal class ThemesHelper
    {
        #region Methods

        /// <summary>
        /// Apply the theme selection.
        /// </summary>
        public static void ApplyThemeSelection()
        {
            foreach (var window in WindowsHelper.ExistingWindowList)
                ((FrameworkElement)window.Content).RequestedTheme =
                    ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] switch
                    {
                        SettingsHelper.TagThemeDark => ElementTheme.Dark,
                        SettingsHelper.TagThemeLight => ElementTheme.Light,
                        _ => ElementTheme.Default
                    };
        } // end method ApplyThemeSelection

        #endregion Methods
    } // end class ThemesHelper
} // end namespace PaimonTray.Helpers