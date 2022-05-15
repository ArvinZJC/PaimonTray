using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The general settings page.
    /// </summary>
    public sealed partial class GeneralSettingsPage
    {
        #region Fields

        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the general settings page.
        /// </summary>
        public GeneralSettingsPage()
        {
            _propertySetSettings = ApplicationData.Current.LocalSettings.Containers[SettingsHelper.ContainerKeySettings]
                .Values;
            InitializeComponent();
            ShowLanguageSelection();
            ShowMainWindowTopNavigationPaneSelection();
            ShowNotificationClearSelection();
            ShowNotificationGreetingSelection();
            ShowThemeSelection();
            UpdateUiText();
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show the language selection.
        /// </summary>
        private void ShowLanguageSelection()
        {
            RadioButtonsLanguage.SelectedItem = _propertySetSettings[SettingsHelper.KeyLanguage] switch
            {
                SettingsHelper.TagLanguageEn => RadioButtonLanguageEn,
                SettingsHelper.TagLanguageZhCn => RadioButtonLanguageZhCn,
                _ => RadioButtonLanguageSystem
            };
        } // end method ShowLanguageSelection

        /// <summary>
        /// Show the selection of the main window's top navigation pane.
        /// </summary>
        private void ShowMainWindowTopNavigationPaneSelection()
        {
            ToggleSwitchMainWindowTopNavigationPane.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyMainWindowTopNavigationPane];
            SettingsHelper.ApplyMainWindowTopNavigationPaneSelection();
        } // end method ShowMainWindowTopNavigationPaneSelection

        /// <summary>
        /// Show the selection for clearing notifications when the app exits.
        /// </summary>
        private void ShowNotificationClearSelection()
        {
            ToggleSwitchNotificationClear.IsOn = (bool)_propertySetSettings[SettingsHelper.KeyNotificationClear];
        } // end method ShowNotificationClearSelection

        /// <summary>
        /// Show the greeting notification selection.
        /// </summary>
        private void ShowNotificationGreetingSelection()
        {
            ToggleSwitchNotificationGreeting.IsOn = (bool)_propertySetSettings[SettingsHelper.KeyNotificationGreeting];
        } // end method ShowNotificationGreetingSelection

        /// <summary>
        /// Show the theme selection.
        /// </summary>
        private void ShowThemeSelection()
        {
            RadioButtonsTheme.SelectedItem = _propertySetSettings[SettingsHelper.KeyTheme] switch
            {
                SettingsHelper.TagThemeDark => RadioButtonThemeDark,
                SettingsHelper.TagThemeLight => RadioButtonThemeLight,
                _ => RadioButtonThemeSystem
            };
        } // end method ShowThemeSelection

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            RadioButtonLanguageEn.Content = resourceLoader.GetString("LanguageEn");
            RadioButtonLanguageSystem.Content = resourceLoader.GetString("SystemDefault");
            RadioButtonLanguageZhCn.Content = resourceLoader.GetString("LanguageZhCn");
            RadioButtonThemeDark.Content = resourceLoader.GetString("ThemeDark");
            RadioButtonThemeLight.Content = resourceLoader.GetString("ThemeLight");
            RadioButtonThemeSystem.Content = resourceLoader.GetString("SystemDefault");
            RunLaunchOnWindowsStartupLinkText.Text = resourceLoader.GetString("LaunchOnWindowsStartupLinkText");
            RunNotificationsLinkText.Text = resourceLoader.GetString("NotificationsLinkText");
            TextBlockLanguage.Text = resourceLoader.GetString("Language");
            TextBlockLanguageAppliedAfterAppRestart.Text = resourceLoader.GetString("ChangesAppliedAfterAppRestart");
            TextBlockLanguageExplanation.Text = resourceLoader.GetString("LanguageExplanation");
            TextBlockLanguageSelection.Text = (RadioButtonsLanguage.SelectedItem as RadioButton)?.Content as string;
            TextBlockLaunchOnWindowsStartup.Text = resourceLoader.GetString("LaunchOnWindowsStartup");
            TextBlockLaunchOnWindowsStartupExplanation.Text =
                resourceLoader.GetString("LaunchOnWindowsStartupExplanation");
            TextBlockMainWindowTopNavigationPane.Text = resourceLoader.GetString("MainWindowTopNavigationPane");
            TextBlockNotificationClear.Text = resourceLoader.GetString("NotificationClear");
            TextBlockNotificationGreeting.Text = resourceLoader.GetString("NotificationGreeting");
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
            new CommandsViewModel().OpenLinkInDefaultCommand.Execute(AppConstantsHelper.UriSystemSettingsStartupApps);
        } // end method HyperlinkLaunchOnWindowsStartupLink_OnClick

#pragma warning disable CA1822 // Mark members as static
        // Handle the click event of the link of the notifications setting.
        private void HyperlinkNotificationsLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            new CommandsViewModel().OpenLinkInDefaultCommand.Execute(AppConstantsHelper.UriSystemSettingsNotifications);
        } // end method HyperlinkNotificationsLink_OnClick

        // Handle the language radio buttons' selection changed event.
        private void RadioButtonsLanguage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var radioButtonsLanguageSelectedItem = RadioButtonsLanguage.SelectedItem as RadioButton;

            if (radioButtonsLanguageSelectedItem == null) return;

            var radioButtonsLanguageSelectedItemTag = radioButtonsLanguageSelectedItem.Tag as string;

            _propertySetSettings[SettingsHelper.KeyLanguage] = radioButtonsLanguageSelectedItemTag;

            TextBlockLanguageAppliedAfterAppRestart.Visibility =
                radioButtonsLanguageSelectedItemTag == (Application.Current as App)?.LanguageSelectionApplied
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            TextBlockLanguageSelection.Text = radioButtonsLanguageSelectedItem.Content as string;
        } // end method RadioButtonsLanguage_OnSelectionChanged

        // Handle the theme radio buttons' selection changed event.
        private void RadioButtonsTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var radioButtonsThemeSelectedItem = RadioButtonsTheme.SelectedItem as RadioButton;

            if (radioButtonsThemeSelectedItem == null) return;

            _propertySetSettings[SettingsHelper.KeyTheme] = radioButtonsThemeSelectedItem.Tag as string;
            SettingsHelper.ApplyThemeSelection();

            TextBlockThemeSelection.Text = radioButtonsThemeSelectedItem.Content as string;
        } // end method RadioButtonsTheme_OnSelectionChanged

        // Handle the toggled event of the toggle switch of the setting for the main window's top navigation pane.
        private void ToggleSwitchMainWindowTopNavigationPane_OnToggled(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyMainWindowTopNavigationPane] =
                ToggleSwitchMainWindowTopNavigationPane.IsOn;
            SettingsHelper.ApplyMainWindowTopNavigationPaneSelection();
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        // Handle the toggled event of the toggle switch for clearing notifications when the app exits.
        private void ToggleSwitchNotificationClear_OnToggled(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyNotificationClear] = ToggleSwitchNotificationClear.IsOn;
        } // end method ToggleSwitchNotificationGreeting_OnToggled

        // Handle the greeting notification toggle switch's toggled event.
        private void ToggleSwitchNotificationGreeting_OnToggled(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyNotificationGreeting] = ToggleSwitchNotificationGreeting.IsOn;
        } // end method ToggleSwitchNotificationGreeting_OnToggled

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views