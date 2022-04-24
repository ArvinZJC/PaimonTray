﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

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
            UpdateUiText();
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

        // Update the UI text.
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            RadioButtonThemeDark.Content = resourceLoader.GetString("DarkTheme");
            RadioButtonThemeLight.Content = resourceLoader.GetString("LightTheme");
            RadioButtonThemeSystem.Content = resourceLoader.GetString("SystemTheme");
            TextBlockLanguage.Text = resourceLoader.GetString("Language");
            TextBlockLanguageExplanation.Text = resourceLoader.GetString("LanguageExplanation");
            TextBlockLanguageSelection.Text = (RadioButtonsLanguage.SelectedItem as RadioButton)?.Content as string;
            TextBlockTheme.Text = resourceLoader.GetString("Theme");
            TextBlockThemeExplanation.Text = resourceLoader.GetString("ThemeExplanation");
            TextBlockThemeSelection.Text = (RadioButtonsTheme.SelectedItem as RadioButton)?.Content as string;
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the theme radio button's checked event.
        private void RadioButtonTheme_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton)?.Tag != null)
            {
                ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] = ((RadioButton)sender).Tag;
                ThemesHelper.ApplyThemeSelection(((RadioButton)sender).Tag as string);
            } // end if

            RadioButtonsTheme.SelectedItem = sender as RadioButton;
            TextBlockThemeSelection.Text = (sender as RadioButton)?.Content as string;
        } // end method RadioButtonsTheme_Checked

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views