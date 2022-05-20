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
        #region Fields;

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
            UpdateUiText();
        } // end constructor GeneralSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show the language selection.
        /// </summary>
        private void ShowLanguageSelection()
        {
            ComboBoxLanguage.SelectedItem = _propertySetSettings[SettingsHelper.KeyLanguage] switch
            {
                SettingsHelper.TagLanguageEn => ComboBoxItemLanguageEn,
                SettingsHelper.TagLanguageZhCn => ComboBoxItemLanguageZhCn,
                SettingsHelper.TagSystem => ComboBoxItemLanguageSystem,
                _ => null
            };
        } // end method ShowLanguageSelection

        /// <summary>
        /// Show the selection of showing the main window when the app starts.
        /// </summary>
        private void ShowMainWindowShowWhenAppStartsSelection()
        {
            ToggleSwitchMainWindowTopNavigationPane.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts];
        } // end method ShowMainWindowShowWhenAppStartsSelection

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
            CheckBoxNotificationClear.IsChecked = (bool)_propertySetSettings[SettingsHelper.KeyNotificationClear];
        } // end method ShowNotificationClearSelection

        /// <summary>
        /// Show the greeting notification selection.
        /// </summary>
        private void ShowNotificationGreetingSelection()
        {
            CheckBoxNotificationGreeting.IsChecked = (bool)_propertySetSettings[SettingsHelper.KeyNotificationGreeting];
        } // end method ShowNotificationGreetingSelection

        /// <summary>
        /// Show the theme selection.
        /// </summary>
        private void ShowThemeSelection()
        {
            ComboBoxTheme.SelectedItem = _propertySetSettings[SettingsHelper.KeyTheme] switch
            {
                SettingsHelper.TagSystem => ComboBoxItemThemeSystem,
                SettingsHelper.TagThemeDark => ComboBoxItemThemeDark,
                SettingsHelper.TagThemeLight => ComboBoxItemThemeLight,
                _ => null
            };
        } // end method ShowThemeSelection

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
            InfoBarLanguageAppliedAfterAppRestart.Title = resourceLoader.GetString("LanguageAppliedAfterAppRestart");
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
            var comboBoxLanguageSelectedItem = ComboBoxLanguage.SelectedItem as ComboBoxItem;

            if (comboBoxLanguageSelectedItem == null) return;

            var comboBoxLanguageSelectedItemTag = comboBoxLanguageSelectedItem.Tag as string;

            _propertySetSettings[SettingsHelper.KeyLanguage] = comboBoxLanguageSelectedItemTag;

            InfoBarLanguageAppliedAfterAppRestart.IsOpen = comboBoxLanguageSelectedItemTag !=
                                                           (Application.Current as App)?.LanguageSelectionApplied;

            if (InfoBarLanguageAppliedAfterAppRestart.IsOpen)
                InfoBarLanguageAppliedAfterAppRestart.Margin = new Thickness(0, 0, 0, 8);
        } // end method ComboBoxLanguage_OnSelectionChanged

        // Handle the theme combo box's selection changed event.
        private void ComboBoxTheme_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxThemeSelectedItem = ComboBoxTheme.SelectedItem as ComboBoxItem;

            if (comboBoxThemeSelectedItem == null) return;

            _propertySetSettings[SettingsHelper.KeyTheme] = comboBoxThemeSelectedItem.Tag as string;
            SettingsHelper.ApplyThemeSelection();
        } // end method ComboBoxTheme_OnSelectionChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            ShowLanguageSelection();
            ShowMainWindowShowWhenAppStartsSelection();
            ShowMainWindowTopNavigationPaneSelection();
            ShowNotificationClearSelection();
            ShowNotificationGreetingSelection();
            ShowThemeSelection();
        } // end method GridRoot_OnLoaded

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

#pragma warning disable CA1822 // Mark members as static
        // Handle the closing event of the info bar for informing the language applied after the app restart.
        private void InfoBarLanguageAppliedAfterAppRestart_OnClosing(InfoBar sender, InfoBarClosingEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            sender.Margin = new Thickness(0);
        } // end method InfoBarLanguageAppliedAfterAppRestart_OnClosing

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
            SettingsHelper.ApplyMainWindowTopNavigationPaneSelection();
        } // end method ToggleSwitchMainWindowTopNavigationPane_OnToggled

        #endregion Event Handlers
    } // end class GeneralSettingsPage
} // end namespace PaimonTray.Views