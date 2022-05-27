using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PaimonTray.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;

namespace PaimonTray.Views
{
    /// <summary>
    /// The general settings page.
    /// </summary>
    public sealed partial class GeneralSettingsPage
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// The settings property set.
        /// </summary>
        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the general settings page.
        /// </summary>
        public GeneralSettingsPage()
        {
            _app = Application.Current as App;
            _propertySetSettings = _app?.SettingsH.PropertySetSettings;
            InitializeComponent();
            UpdateUiText();
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            CheckBoxNotificationClear.Content = resourceLoader.GetString("NotificationClear");
            CheckBoxNotificationGreeting.Content = resourceLoader.GetString("NotificationGreeting");
            ComboBoxItemLanguageEn.Content = resourceLoader.GetString("LanguageEn");
            ComboBoxItemLanguageSystem.Content = resourceLoader.GetString("SystemDefault");
            ComboBoxItemLanguageZhCn.Content = resourceLoader.GetString("LanguageZhCn");
            ComboBoxItemThemeDark.Content = resourceLoader.GetString("ThemeDark");
            ComboBoxItemThemeLight.Content = resourceLoader.GetString("ThemeLight");
            ComboBoxItemThemeSystem.Content = resourceLoader.GetString("SystemDefault");
            InfoBarLanguageAppliedLater.Title = resourceLoader.GetString("LanguageAppliedLater");
            RunLaunchOnWindowsStartupLinkText.Text = resourceLoader.GetString("LaunchOnWindowsStartupLinkText");
            RunNotificationsLinkText.Text = resourceLoader.GetString("NotificationsLinkText");
            TextBlockLanguage.Text = resourceLoader.GetString("Language");
            TextBlockLanguageExplanation.Text = resourceLoader.GetString("LanguageExplanation");
            TextBlockLaunchOnWindowsStartup.Text = resourceLoader.GetString("LaunchOnWindowsStartup");
            TextBlockLaunchOnWindowsStartupExplanation.Text =
                resourceLoader.GetString("LaunchOnWindowsStartupExplanation");
            TextBlockMainWindowShowWhenAppStarts.Text = resourceLoader.GetString("MainWindowShowWhenAppStarts");
            TextBlockMainWindowTopNavigationPane.Text = resourceLoader.GetString("MainWindowTopNavigationPane");
            TextBlockMainWindowTopNavigationPaneExplanation.Text =
                resourceLoader.GetString("MainWindowTopNavigationPaneExplanation");
            TextBlockNotifications.Text = resourceLoader.GetString("Notifications");
            TextBlockNotificationsExplanation.Text = resourceLoader.GetString("NotificationsExplanation");
            TextBlockNotificationsNotice.Text = resourceLoader.GetString("NotificationsNotice");
            TextBlockTheme.Text = resourceLoader.GetString("Theme");
            TextBlockThemeExplanation.Text = resourceLoader.GetString("ThemeExplanation");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the checked event of the check box for clearing notifications when the app exits.
        private void CheckBoxNotificationClear_OnChecked(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyNotificationClear] = CheckBoxNotificationClear.IsChecked;
        } // end method CheckBoxNotificationClear_OnChecked

        // Handle the unchecked event of the check box for clearing notifications when the app exits.
        private void CheckBoxNotificationClear_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyNotificationClear] = CheckBoxNotificationClear.IsChecked;
        } // end method CheckBoxNotificationClear_OnUnchecked

        // Handle the checked event of the greeting notification check box.
        private void CheckBoxNotificationGreeting_OnChecked(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyNotificationGreeting] = CheckBoxNotificationGreeting.IsChecked;
        } // end method CheckBoxNotificationGreeting_OnChecked

        // Handle the unchecked event of the greeting notification check box.
        private void CheckBoxNotificationGreeting_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyNotificationGreeting] = CheckBoxNotificationGreeting.IsChecked;
        } // end method CheckBoxNotificationGreeting_OnUnchecked

        // Handle the language combo box item's loaded event.
        private void ComboBoxItemLanguage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemLanguageActualWidth = ((ComboBoxItem)sender).ActualWidth;

            if (ComboBoxLanguage.MinWidth < comboBoxItemLanguageActualWidth)
                ComboBoxLanguage.MinWidth = comboBoxItemLanguageActualWidth;
        } // end method ComboBoxItemLanguage_OnLoaded

        // Handle the theme combo box item's loaded event.
        private void ComboBoxItemTheme_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemThemeActualWidth = ((ComboBoxItem)sender).ActualWidth;

            if (ComboBoxTheme.MinWidth < comboBoxItemThemeActualWidth)
                ComboBoxTheme.MinWidth = comboBoxItemThemeActualWidth;
        } // end method ComboBoxItemTheme_OnLoaded

        // Handle the language combo box's selection changed event.
        private void ComboBoxLanguage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxLanguage.SelectedItem is not ComboBoxItem comboBoxLanguageSelectedItem) return;

            var comboBoxLanguageSelectedItemTag = comboBoxLanguageSelectedItem.Tag as string;

            _propertySetSettings[SettingsHelper.KeyLanguage] = comboBoxLanguageSelectedItemTag;

            InfoBarLanguageAppliedLater.IsOpen =
                comboBoxLanguageSelectedItemTag != _app.SettingsH.LanguageSelectionApplied;

            if (InfoBarLanguageAppliedLater.IsOpen)
                InfoBarLanguageAppliedLater.Margin = new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
        } // end method ComboBoxLanguage_OnSelectionChanged

        // Handle the theme combo box's selection changed event.
        private void ComboBoxTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTheme.SelectedItem is not ComboBoxItem comboBoxThemeSelectedItem) return;

            _propertySetSettings[SettingsHelper.KeyTheme] = comboBoxThemeSelectedItem.Tag as string;
            _app.SettingsH.ApplyThemeSelection();
        } // end method ComboBoxTheme_OnSelectionChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Show the settings' selection.
            CheckBoxNotificationClear.IsChecked = (bool)_propertySetSettings[SettingsHelper.KeyNotificationClear];
            CheckBoxNotificationGreeting.IsChecked = (bool)_propertySetSettings[SettingsHelper.KeyNotificationGreeting];
            ComboBoxLanguage.SelectedItem = _propertySetSettings[SettingsHelper.KeyLanguage] switch
            {
                SettingsHelper.TagLanguageEn => ComboBoxItemLanguageEn,
                SettingsHelper.TagLanguageZhCn => ComboBoxItemLanguageZhCn,
                SettingsHelper.TagSystem => ComboBoxItemLanguageSystem,
                _ => null
            };
            ComboBoxTheme.SelectedItem = _propertySetSettings[SettingsHelper.KeyTheme] switch
            {
                SettingsHelper.TagSystem => ComboBoxItemThemeSystem,
                SettingsHelper.TagThemeDark => ComboBoxItemThemeDark,
                SettingsHelper.TagThemeLight => ComboBoxItemThemeLight,
                _ => null
            };
            ToggleSwitchMainWindowShowWhenAppStarts.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts];
            ToggleSwitchMainWindowTopNavigationPane.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyMainWindowTopNavigationPane];
        } // end method GridRoot_OnLoaded

        // Handle the click event of the link of the setting for configuring launch on Windows startup.
        private void HyperlinkLaunchOnWindowsStartupLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            _app.CommandsVm.OpenLinkInDefaultCommand.Execute(AppConstantsHelper.UriSystemSettingsStartupApps);
        } // end method HyperlinkLaunchOnWindowsStartupLink_OnClick

        // Handle the click event of the link of the notifications setting.
        private void HyperlinkNotificationsLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            _app.CommandsVm.OpenLinkInDefaultCommand.Execute(AppConstantsHelper.UriSystemSettingsNotifications);
        } // end method HyperlinkNotificationsLink_OnClick

#pragma warning disable CA1822 // Mark members as static
        // Handle the closing event of the info bar for informing the language applied later.
        private void InfoBarLanguageAppliedLater_OnClosing(InfoBar sender, InfoBarClosingEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            sender.Margin = new Thickness(0);
        } // end method InfoBarLanguageAppliedLater_OnClosing

        // Handle the toggled event of the toggle switch of the setting for showing the main window when the app starts.
        private void ToggleSwitchMainWindowShowWhenAppStarts_OnToggled(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts] =
                ToggleSwitchMainWindowShowWhenAppStarts.IsOn;
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        // Handle the toggled event of the toggle switch of the setting for the main window's top navigation pane.
        private void ToggleSwitchMainWindowTopNavigationPane_OnToggled(object sender, RoutedEventArgs e)
        {
            _propertySetSettings[SettingsHelper.KeyMainWindowTopNavigationPane] =
                ToggleSwitchMainWindowTopNavigationPane.IsOn;
            _app.SettingsH.ApplyMainWindowTopNavigationPaneSelection(); // Apply after changing.
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views