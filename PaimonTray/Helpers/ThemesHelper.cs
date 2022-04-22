using Microsoft.UI.Xaml;
using Serilog;

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
        /// <param name="themeSelection">The selected theme option tag.</param>
        public static void ApplyThemeSelection(string themeSelection)
        {
            ElementTheme theme;

            switch (themeSelection)
            {
                case SettingsHelper.TagThemeDark:
                    theme = ElementTheme.Dark;
                    break;

                case SettingsHelper.TagThemeLight:
                    theme = ElementTheme.Light;
                    break;

                case SettingsHelper.TagThemeSystem:
                    theme = ElementTheme.Default;
                    break;

                default:
                    theme = ElementTheme.Default;
                    Log.Warning($"Invalid theme selection ({themeSelection}).");
                    break;
            } // end switch-case

            foreach (var window in WindowsHelper.ExistingWindowList)
                ((FrameworkElement)window.Content).RequestedTheme = theme;
        } // end method ApplyThemeSelection

        #endregion Methods
    } // end class ThemesHelper
} // end namespace PaimonTray.Helpers