using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using PaimonTray.Helpers;
using System.Globalization;

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
            _app = Application.Current as App;
            InitializeComponent();
            UpdateUiText();
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Event Handlers

        // Handle the checked event of the check box for clearing notifications when the app exits.
        private void CheckBoxNotificationClear_OnChecked(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyNotificationClear] =
                CheckBoxNotificationClear.IsChecked;
        } // end method CheckBoxNotificationClear_OnChecked

        // Handle the unchecked event of the check box for clearing notifications when the app exits.
        private void CheckBoxNotificationClear_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyNotificationClear] =
                CheckBoxNotificationClear.IsChecked;
        } // end method CheckBoxNotificationClear_OnUnchecked

        // Handle the checked event of the greeting notification check box.
        private void CheckBoxNotificationGreeting_OnChecked(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyNotificationGreeting] =
                CheckBoxNotificationGreeting.IsChecked;
        } // end method CheckBoxNotificationGreeting_OnChecked

        // Handle the unchecked event of the greeting notification check box.
        private void CheckBoxNotificationGreeting_OnUnchecked(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyNotificationGreeting] =
                CheckBoxNotificationGreeting.IsChecked;
        } // end method CheckBoxNotificationGreeting_OnUnchecked

#pragma warning disable CA1822 // Mark members as static
        // Handle the combo box's drop-down closed event.
        private void ComboBox_OnDropDownClosed(object sender, object e)
#pragma warning restore CA1822 // Mark members as static
        {
            if (sender is not ComboBox comboBox) return;

            comboBox.MinWidth = 0;
        } // end method ComboBox_OnDropDownClosed

        // Handle the language combo box item's loaded event.
        private void ComboBoxItemLanguage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemLanguageActualWidth = (sender as ComboBoxItem)?.ActualWidth ?? 0;

            if (ComboBoxLanguage.MinWidth < comboBoxItemLanguageActualWidth)
                ComboBoxLanguage.MinWidth = comboBoxItemLanguageActualWidth;
        } // end method ComboBoxItemLanguage_OnLoaded

        // Handle the theme combo box item's loaded event.
        private void ComboBoxItemTheme_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemThemeActualWidth = (sender as ComboBoxItem)?.ActualWidth ?? 0;

            if (ComboBoxTheme.MinWidth < comboBoxItemThemeActualWidth)
                ComboBoxTheme.MinWidth = comboBoxItemThemeActualWidth;
        } // end method ComboBoxItemTheme_OnLoaded

        // Handle the language combo box's selection changed event.
        private void ComboBoxLanguage_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxLanguage.SelectedItem is not ComboBoxItem comboBoxLanguageSelectedItem) return;

            var comboBoxLanguageSelectedItemTag = comboBoxLanguageSelectedItem.Tag as string;

            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyLanguage] = comboBoxLanguageSelectedItemTag;

            InfoBarLanguageAppliedLater.IsOpen =
                comboBoxLanguageSelectedItemTag != _app.SettingsH.LanguageSelectionApplied;

            if (InfoBarLanguageAppliedLater.IsOpen)
                InfoBarLanguageAppliedLater.Margin = new Thickness(0, 0, 0, AppFieldsHelper.InfoBarMarginBottom);
        } // end method ComboBoxLanguage_OnSelectionChanged

        // Handle the theme combo box's selection changed event.
        private void ComboBoxTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxTheme.SelectedItem is not ComboBoxItem comboBoxThemeSelectedItem) return;

            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyTheme] = comboBoxThemeSelectedItem.Tag as string;
            _app.SettingsH.ApplyThemeSelection();
        } // end method ComboBoxTheme_OnSelectionChanged

        // Handle the general settings page's loaded event.
        private void GeneralSettingsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            // Show the settings' selection.
            CheckBoxNotificationClear.IsChecked = propertySetSettings[SettingsHelper.KeyNotificationClear] as bool? ??
                                                  SettingsHelper.DefaultNotificationClear;
            CheckBoxNotificationGreeting.IsChecked =
                propertySetSettings[SettingsHelper.KeyNotificationGreeting] as bool? ??
                SettingsHelper.DefaultNotificationGreeting;

            var language = propertySetSettings[SettingsHelper.KeyLanguage] as string;
            var theme = propertySetSettings[SettingsHelper.KeyTheme] as string;

            ComboBoxLanguage.SelectedItem = language switch
            {
                SettingsHelper.TagLanguageEnGb => ComboBoxItemLanguageEnGb,
                SettingsHelper.TagLanguageEnUs => ComboBoxItemLanguageEnUs,
                SettingsHelper.TagLanguageZhHansCn => ComboBoxItemLanguageZhHansCn,
                SettingsHelper.TagSystem => ComboBoxItemLanguageSystem,
                _ => null
            };
            ComboBoxTheme.SelectedItem = theme switch
            {
                SettingsHelper.TagSystem => ComboBoxItemThemeSystem,
                SettingsHelper.TagThemeDark => ComboBoxItemThemeDark,
                SettingsHelper.TagThemeLight => ComboBoxItemThemeLight,
                _ => null
            };
            ToggleSwitchMainWindowShowWhenAppStarts.IsOn =
                propertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts] as bool? ??
                SettingsHelper.DefaultMainWindowShowWhenAppStarts;
            ToggleSwitchMainWindowTopNavigationPane.IsOn =
                propertySetSettings[SettingsHelper.KeyMainWindowTopNavigationPane] as bool? ??
                SettingsHelper.DefaultMainWindowTopNavigationPane;
        } // end method GeneralSettingsPage_OnLoaded

        // Handle the general settings page's unloaded event.
        private void GeneralSettingsPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _app = null;
        } // end method GeneralSettingsPage_OnUnloaded

        // Handle the click event of the link of the setting for configuring launch on Windows startup.
        private void HyperlinkLaunchAtWindowsStartupLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            _app.CommandsVm.OpenLinkInDefaultCommand.Execute(AppFieldsHelper.UriSystemSettingsStartupApps);
        } // end method HyperlinkLaunchAtWindowsStartupLink_OnClick

        // Handle the click event of the link of the notifications setting.
        private void HyperlinkNotificationsLink_OnClick(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            _app.CommandsVm.OpenLinkInDefaultCommand.Execute(AppFieldsHelper.UriSystemSettingsNotifications);
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
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts] =
                ToggleSwitchMainWindowShowWhenAppStarts.IsOn;
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        // Handle the toggled event of the toggle switch of the setting for the main window's top navigation pane.
        private void ToggleSwitchMainWindowTopNavigationPane_OnToggled(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyMainWindowTopNavigationPane] =
                ToggleSwitchMainWindowTopNavigationPane.IsOn;
            _app.SettingsH.ApplyMainWindowTopNavigationPaneSelection(); // Apply after changing.
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            CheckBoxNotificationClear.Content = resourceLoader.GetString("NotificationClear");
            CheckBoxNotificationGreeting.Content = resourceLoader.GetString("NotificationGreeting");
            ComboBoxItemLanguageEnGb.Content = new CultureInfo(SettingsHelper.TagLanguageEnGb).NativeName;
            ComboBoxItemLanguageEnUs.Content = new CultureInfo(SettingsHelper.TagLanguageEnUs).NativeName;
            ComboBoxItemLanguageSystem.Content = resourceLoader.GetString("SystemDefault");
            ComboBoxItemLanguageZhHansCn.Content = new CultureInfo(SettingsHelper.TagLanguageZhHansCn).NativeName;
            ComboBoxItemThemeDark.Content = resourceLoader.GetString("ThemeDark");
            ComboBoxItemThemeLight.Content = resourceLoader.GetString("ThemeLight");
            ComboBoxItemThemeSystem.Content = resourceLoader.GetString("SystemDefault");
            InfoBarLanguageAppliedLater.Title = resourceLoader.GetString("LanguageAppliedLater");
            RunLaunchAtWindowsStartupLinkText.Text = resourceLoader.GetString("LaunchAtWindowsStartupLinkText");
            RunNotificationsLinkText.Text = resourceLoader.GetString("NotificationsLinkText");
            TextBlockLanguage.Text = resourceLoader.GetString("Language");
            TextBlockLanguageExplanation.Text = resourceLoader.GetString("LanguageExplanation");
            TextBlockLaunchAtWindowsStartup.Text = resourceLoader.GetString("LaunchAtWindowsStartup");
            TextBlockLaunchAtWindowsStartupExplanation.Text =
                resourceLoader.GetString("LaunchAtWindowsStartupExplanation");
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
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views