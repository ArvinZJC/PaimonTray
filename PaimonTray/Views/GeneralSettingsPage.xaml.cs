using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;

namespace PaimonTray.Views
{
    /// <summary>
    /// The general settings page.
    /// </summary>
    public sealed partial class GeneralSettingsPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the general settings page.
        /// </summary>
        public GeneralSettingsPage()
        {
            InitializeComponent();

            switch (ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] as string)
            {
                case SettingsHelper.TagThemeDark:
                    RadioButtonThemeDark.IsChecked = true;
                    break;

                case SettingsHelper.TagThemeLight:
                    RadioButtonThemeLight.IsChecked = true;
                    break;

                default:
                    RadioButtonThemeSystem.IsChecked = true;
                    break;
            } // end switch-case
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Event Handlers

        private void RadioButtonsTheme_Checked(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton)?.Tag as string)
            {
                case SettingsHelper.TagThemeDark:
                    ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] = SettingsHelper.TagThemeDark;
                    break;

                case SettingsHelper.TagThemeLight:
                    ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] =
                        SettingsHelper.TagThemeLight;
                    break;

                case SettingsHelper.TagThemeSystem:
                    ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] =
                        SettingsHelper.TagThemeSystem;
                    break;
            } // end switch-case

            TextBlockThemeSelection.Text = (sender as RadioButton)?.Content as string;
        } // end method RadioButtonsTheme_Checked

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views