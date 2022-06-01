using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaimonTray.Helpers;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.Foundation.Collections;

namespace PaimonTray.Views
{
    /// <summary>
    /// The accounts settings page.
    /// </summary>
    public sealed partial class AccountsSettingsPage
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// The settings property set.
        /// </summary>
        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the accounts settings page.
        /// </summary>
        public AccountsSettingsPage()
        {
            _app = Application.Current as App;
            _mainWindow = _app?.WindowsH.GetMainWindow();
            _propertySetSettings = _app?.SettingsH.PropertySetSettings;
            InitializeComponent();
            UpdateUiText();
        } // end constructor AccountsSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Invoked immediately after the page is unloaded and is no longer the current source of a parent frame.
        /// </summary>
        /// <param name="e">Details about the navigation that has unloaded the current page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged -= AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            base.OnNavigatedFrom(e);
        } // end method OnNavigatedFrom

        /// <summary>
        /// Invoked when the page is loaded and becomes the current source of a parent frame.
        /// </summary>
        /// <param name="e">Details about the pending navigation that will load the current page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged += AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            CollectionViewSourceAccountGroups.Source = _app.AccountsH.AccountGroupInfoLists
                .OrderBy(accountGroupInfoList => accountGroupInfoList.Key).ToImmutableList();
            base.OnNavigatedTo(e);
        } // end method OnNavigatedTo

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            ComboBoxItemServerCn.Content = resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = resourceLoader.GetString("ServerGlobal");
            InfoBarLoginAlternativeAlwaysAppliedLater.Title =
                resourceLoader.GetString("LoginAlternativeAlwaysAppliedLater");
            InfoBarServerDefaultAppliedLater.Title = resourceLoader.GetString("ServerDefaultAppliedLater");
            TextBlockAccountsManagement.Text = resourceLoader.GetString("AccountsManagement");
            TextBlockAccountsManagementExplanation.Text = resourceLoader.GetString("AccountsManagementExplanation");
            TextBlockLoginAlternativeAlways.Text = resourceLoader.GetString("LoginAlternativeAlways");
            TextBlockLoginAlternativeAlwaysExplanation.Text =
                resourceLoader.GetString("LoginAlternativeAlwaysExplanation");
            TextBlockServerDefault.Text = resourceLoader.GetString("ServerDefault");
            TextBlockServerDefaultExplanation.Text = resourceLoader.GetString("ServerDefaultExplanation");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the account group info lists' collection changed event.
        private void AccountGroupInfoLists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionViewSourceAccountGroups.Source = _app.AccountsH.AccountGroupInfoLists
                .OrderBy(accountGroupInfoList => accountGroupInfoList.Key).ToImmutableList();
        } // end method AccountGroupInfoLists_CollectionChanged

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is AccountsHelper.PropertyNameIsChecking)
                CollectionViewSourceAccountGroups.Source = _app.AccountsH.AccountGroupInfoLists
                    .OrderBy(accountGroupInfoList => accountGroupInfoList.Key).ToImmutableList();
        } // end method AccountsHelper_OnPropertyChanged

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
            if (ComboBoxServerDefault.SelectedItem is not ComboBoxItem comboBoxServerDefaultSelectedItem ||
                _propertySetSettings[SettingsHelper.KeyServerDefault] as string ==
                comboBoxServerDefaultSelectedItem.Tag as string) return;

            _propertySetSettings[SettingsHelper.KeyServerDefault] = comboBoxServerDefaultSelectedItem.Tag as string;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAddUpdateAccount) return;

            InfoBarServerDefaultAppliedLater.IsOpen = true;
            InfoBarServerDefaultAppliedLater.Margin = new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
        } // end method ComboBoxServerDefault_OnSelectionChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Show the settings' selection.
            ComboBoxServerDefault.SelectedItem = _propertySetSettings[SettingsHelper.KeyServerDefault] switch
            {
                AccountsHelper.TagServerCn => ComboBoxItemServerCn,
                AccountsHelper.TagServerGlobal => ComboBoxItemServerGlobal,
                _ => null
            };
            ToggleSwitchLoginAlternativeAlways.IsOn =
                (bool)_propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways];
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
            InfoBarLoginAlternativeAlwaysAppliedLater.Margin =
                new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
        } // end method ToggleSwitchLoginAlternativeAlways_OnToggled

        #endregion Event Handlers
    } // end class AccountsSettingsPage
} // end namespace PaimonTray.Views