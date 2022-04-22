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
            ShowThemeSelection();
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Methods

        // Show the theme selection.
        private void ShowThemeSelection()
        {
            switch (ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme])
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
        } // end method ShowThemeSelection

        #endregion Methods

        #region Event Handlers

        // Handle the theme radio buttons' checked event.
        private void RadioButtonsTheme_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton)?.Tag != null)
            {
                ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] = ((RadioButton)sender).Tag;
                ThemesHelper.ApplyThemeSelection(((RadioButton)sender).Tag as string);
            } // end if

            TextBlockThemeSelection.Text = (sender as RadioButton)?.Content as string;
        } // end method RadioButtonsTheme_Checked

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views