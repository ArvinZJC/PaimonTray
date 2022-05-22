using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The accounts settings page.
    /// </summary>
    public sealed partial class AccountsSettingsPage
    {
        #region Fields

        private readonly MainWindow _mainWindow;
        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the accounts settings page.
        /// </summary>
        public AccountsSettingsPage()
        {
            _mainWindow = WindowsHelper.ShowMainWindow();
            _propertySetSettings = ApplicationData.Current.LocalSettings.Containers[SettingsHelper.ContainerKeySettings]
                .Values;
            InitializeComponent();
            UpdateUiText();
        } // end constructor AccountsSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show the selection for always using the alternative login method.
        /// </summary>
        private void ShowLoginAlternativeAlwaysSelection()
        {
            ToggleSwitchLoginAlternativeAlways.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways];
        } // end method ShowLoginAlternativeAlwaysSelection

        /// <summary>
        /// Show the selection for the default server.
        /// </summary>
        private void ShowServerDefaultSelection()
        {
            ComboBoxServerDefault.SelectedItem = _propertySetSettings[SettingsHelper.KeyServerDefault] switch
            {
                AccountsHelper.TagServerCn => ComboBoxItemServerCn,
                AccountsHelper.TagServerGlobal => ComboBoxItemServerGlobal,
                _ => null
            };
        } // end method ShowServerDefaultExplanation

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            ComboBoxItemServerCn.Content = resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = resourceLoader.GetString("ServerGlobal");
            InfoBarLoginAlternativeAlwaysAppliedLater.Title =
                resourceLoader.GetString("LoginAlternativeAlwaysAppliedLater");
            InfoBarServerDefaultAppliedLater.Title = resourceLoader.GetString("ServerDefaultAppliedLater");
            TextBlockAccountsManagement.Text = resourceLoader.GetString("AccountsManagement");
            TextBlockLoginAlternativeAlways.Text = resourceLoader.GetString("LoginAlternativeAlways");
            TextBlockLoginAlternativeAlwaysExplanation.Text =
                resourceLoader.GetString("LoginAlternativeAlwaysExplanation");
            TextBlockServerDefault.Text = resourceLoader.GetString("ServerDefault");
            TextBlockServerDefaultExplanation.Text = resourceLoader.GetString("ServerDefaultExplanation");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the server combo box item's loaded event.
        private void ComboBoxItemServer_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemServerActualWidth = ((ComboBoxItem)sender).ActualWidth;

            if (ComboBoxServerDefault.MinWidth < comboBoxItemServerActualWidth)
                ComboBoxServerDefault.MinWidth = comboBoxItemServerActualWidth;
        } // end method ComboBoxItemServer_OnLoaded

        // Handle the default server combo box's selection changed event.
        private void ComboBoxServerDefault_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBoxServerDefaultSelectedItem = ComboBoxServerDefault.SelectedItem as ComboBoxItem;

            if (comboBoxServerDefaultSelectedItem == null ||
                _propertySetSettings[SettingsHelper.KeyServerDefault] as string ==
                comboBoxServerDefaultSelectedItem.Tag as string) return;

            _propertySetSettings[SettingsHelper.KeyServerDefault] = comboBoxServerDefaultSelectedItem.Tag as string;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAddUpdateAccount) return;

            InfoBarServerDefaultAppliedLater.IsOpen = true;
            InfoBarServerDefaultAppliedLater.Margin = new Thickness(0, 0, 0, 4);
        } // end method ComboBoxServerDefault_OnSelectionChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            ShowLoginAlternativeAlwaysSelection();
            ShowServerDefaultSelection();
        } // end method GridRoot_OnLoaded

#pragma warning disable CA1822 // Mark members as static
        // Handle the info bar's closing event.
        private void InfoBar_OnClosing(InfoBar sender, InfoBarClosingEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            sender.Margin = new Thickness(0);
        } // end method InfoBar_OnClosing

        // Handle the toggled event of the toggle switch of the setting for always using the alternative login method.
        private void ToggleSwitchLoginAlternativeAlways_OnToggled(object sender, RoutedEventArgs e)
        {
            if ((bool)_propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] ==
                ToggleSwitchLoginAlternativeAlways.IsOn) return;

            _propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] = ToggleSwitchLoginAlternativeAlways.IsOn;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAddUpdateAccount) return;

            InfoBarLoginAlternativeAlwaysAppliedLater.IsOpen = true;
            InfoBarLoginAlternativeAlwaysAppliedLater.Margin = new Thickness(0, 0, 0, 4);
        } // end method ToggleSwitchLoginAlternativeAlways_OnToggled

        #endregion Event Handlers
    } // end class AccountsSettingsPage
} // end namespace PaimonTray.Views