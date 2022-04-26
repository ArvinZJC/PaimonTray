using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
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
            ShowGreetingNotificationSelection();
            ShowLanguageSelection();
            ShowMainWindowTopNavigationPaneSelection();
            ShowThemeSelection();
            UpdateUiText();
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show the greeting notification selection.
        /// </summary>
        private void ShowGreetingNotificationSelection()
        {
            ToggleSwitchGreetingNotification.IsOn =
                (bool)ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyGreetingNotification];
        } // end method ShowGreetingNotificationSelection

        /// <summary>
        /// Show the language selection.
        /// </summary>
        private void ShowLanguageSelection()
        {
            switch (ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyLanguage])
            {
                case SettingsHelper.TagLanguageEn:
                    RadioButtonLanguageEn.IsChecked = true;
                    break;

                case SettingsHelper.TagLanguageZhCn:
                    RadioButtonLanguageZhCn.IsChecked = true;
                    break;

                default:
                    RadioButtonLanguageSystem.IsChecked = true;
                    break;
            } // end switch-case
        } // end method ShowLanguageSelection

        /// <summary>
        /// Show the selection of the main window's top navigation pane.
        /// </summary>
        private void ShowMainWindowTopNavigationPaneSelection()
        {
            ToggleSwitchMainWindowTopNavigationPane.IsOn =
                (bool)ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyMainWindowTopNavigationPane];
            SettingsHelper.ApplyMainWindowTopNavigationPaneSelection();
        } // end method ShowMainWindowTopNavigationPaneSelection

        /// <summary>
        /// Show the theme selection.
        /// </summary>
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

        /// <summary>
        /// Update the UI text.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            RadioButtonLanguageSystem.Content = resourceLoader.GetString("SystemDefault");
            RadioButtonThemeDark.Content = resourceLoader.GetString("DarkTheme");
            RadioButtonThemeLight.Content = resourceLoader.GetString("LightTheme");
            RadioButtonThemeSystem.Content = resourceLoader.GetString("SystemDefault");
            RunLaunchOnWindowsStartupLinkText.Text = resourceLoader.GetString("LaunchOnWindowsStartupLinkText");
            RunNotificationsLinkText.Text = resourceLoader.GetString("NotificationsLinkText");
            TextBlockGreetingNotification.Text = resourceLoader.GetString("GreetingNotification");
            TextBlockLanguage.Text = resourceLoader.GetString("Language");
            TextBlockLanguageAppliedAfterAppRestart.Text = resourceLoader.GetString("ChangesAppliedAfterAppRestart");
            TextBlockLanguageExplanation.Text = resourceLoader.GetString("LanguageExplanation");
            TextBlockLanguageSelection.Text = (RadioButtonsLanguage.SelectedItem as RadioButton)?.Content as string;
            TextBlockLaunchOnWindowsStartup.Text = resourceLoader.GetString("LaunchOnWindowsStartup");
            TextBlockLaunchOnWindowsStartupExplanation.Text =
                resourceLoader.GetString("LaunchOnWindowsStartupExplanation");
            TextBlockMainWindowTopNavigationPane.Text = resourceLoader.GetString("MainWindowTopNavigationPane");
            TextBlockNotifications.Text = resourceLoader.GetString("Notifications");
            TextBlockNotificationsExplanation.Text = resourceLoader.GetString("NotificationsExplanation");
            TextBlockTheme.Text = resourceLoader.GetString("Theme");
            TextBlockThemeExplanation.Text = resourceLoader.GetString("ThemeExplanation");
            TextBlockThemeSelection.Text = (RadioButtonsTheme.SelectedItem as RadioButton)?.Content as string;
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

#pragma warning disable CA1822 // Mark members as static
        // Handle the click event of the link of the setting for configuring launch on Windows startup.
        private void HyperlinkLaunchOnWindowsStartupLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            new CommandsViewModel().OpenLinkInDefaultCommand.Execute(AppConstantsHelper.SystemSettingsStartupAppsUri);
        } // end method HyperlinkLaunchOnWindowsStartupLink_OnClick

#pragma warning disable CA1822 // Mark members as static
        // Handle the click event of the link of the notifications setting.
        private void HyperlinkNotificationsLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            new CommandsViewModel().OpenLinkInDefaultCommand.Execute(AppConstantsHelper.SystemSettingsNotificationsUri);
        } // end method HyperlinkNotificationsLink_OnClick

        // Handle the language radio button's checked event.
        private void RadioButtonLanguage_OnChecked(object sender, RoutedEventArgs e)
        {
            if (RadioButtonsLanguage.SelectedItem != sender && (sender as RadioButton)?.Tag != null)
                ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyLanguage] = ((RadioButton)sender).Tag;

            RadioButtonsLanguage.SelectedItem = sender;
            TextBlockLanguageAppliedAfterAppRestart.Visibility =
                ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyLanguage] as string ==
                (Application.Current as App)?.LanguageSelectionApplied
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            TextBlockLanguageSelection.Text = (sender as RadioButton)?.Content as string;
        } // end method RadioButtonLanguage_OnChecked

        // Handle the theme radio button's checked event.
        private void RadioButtonTheme_Checked(object sender, RoutedEventArgs e)
        {
            if (RadioButtonsTheme.SelectedItem != sender && (sender as RadioButton)?.Tag != null)
            {
                ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyTheme] = ((RadioButton)sender).Tag;
                SettingsHelper.ApplyThemeSelection();
            } // end if

            RadioButtonsTheme.SelectedItem = sender;
            TextBlockThemeSelection.Text = (sender as RadioButton)?.Content as string;
        } // end method RadioButtonsTheme_Checked

        // Handle the greeting notification toggle switch's toggled event.
        private void ToggleSwitchGreetingNotification_OnToggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyGreetingNotification] =
                ToggleSwitchGreetingNotification.IsOn;
        } // end method ToggleSwitchGreetingNotification_OnToggled

        // Handle the toggled event of the toggle switch of the setting for configuring the main window's top navigation pane.
        private void ToggleSwitchMainWindowTopNavigationPane_OnToggled(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyMainWindowTopNavigationPane] =
                ToggleSwitchMainWindowTopNavigationPane.IsOn;
            SettingsHelper.ApplyMainWindowTopNavigationPaneSelection();
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views