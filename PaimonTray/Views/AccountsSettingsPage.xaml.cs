using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaimonTray.Helpers;
using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using Windows.ApplicationModel.Resources;
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

        private readonly ResourceLoader _resourceLoader;

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
            _resourceLoader = _app?.SettingsH.ResLoader;
            InitializeComponent();
            UpdateUiText();

            CommandBarFlyoutCommandBarAccountGroups.Translation += new Vector3(0, 0, 32);
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
            ToggleStatusVisibility();
            base.OnNavigatedTo(e);
        } // end method OnNavigatedTo

        /// <summary>
        /// Show/Hide the status.
        /// </summary>
        private void ToggleStatusVisibility()
        {
            if (_app.AccountsH.IsManaging)
            {
                GridStatusWarning.Visibility = Visibility.Collapsed;
                ProgressRingStatusLoading.Visibility = Visibility.Visible;
                TextBlockStatus.Text = _resourceLoader.GetString("StatusLoading");
                GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
            }
            else
            {
                var accountGroupInfoLists = _app.AccountsH.AccountGroupInfoLists
                    .OrderBy(accountGroupInfoList => accountGroupInfoList.Key).ToImmutableList();

                CollectionViewSourceAccountGroups.Source = accountGroupInfoLists;

                if (accountGroupInfoLists.Count > 0) GridStatus.Visibility = Visibility.Collapsed;
                else
                {
                    GridStatusWarning.Visibility = Visibility.Visible;
                    ProgressRingStatusLoading.Visibility = Visibility.Collapsed;
                    TextBlockStatus.Text = _resourceLoader.GetString("AccountGroupNoCharacter");
                    GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
                } // end if...else
            } // end if...else
        } // end method ToggleStatusVisibility

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            ComboBoxItemServerCn.Content = _resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = _resourceLoader.GetString("ServerGlobal");
            ContentDialogueAccountGroupsRemove.CloseButtonText = _resourceLoader.GetString("No");
            ContentDialogueAccountGroupsRemove.PrimaryButtonText = _resourceLoader.GetString("Yes");
            InfoBarLoginAlternativeAlwaysAppliedLater.Title =
                _resourceLoader.GetString("LoginAlternativeAlwaysAppliedLater");
            InfoBarServerDefaultAppliedLater.Title = _resourceLoader.GetString("ServerDefaultAppliedLater");
            TextBlockAccountsManagement.Text = _resourceLoader.GetString("AccountsManagement");
            TextBlockAccountsManagementExplanation.Text = _resourceLoader.GetString("AccountsManagementExplanation");
            TextBlockLoginAlternativeAlways.Text = _resourceLoader.GetString("LoginAlternativeAlways");
            TextBlockLoginAlternativeAlwaysExplanation.Text =
                _resourceLoader.GetString("LoginAlternativeAlwaysExplanation");
            TextBlockServerDefault.Text = _resourceLoader.GetString("ServerDefault");
            TextBlockServerDefaultExplanation.Text = _resourceLoader.GetString("ServerDefaultExplanation");
            ToolTipService.SetToolTip(AppBarButtonAccountGroupsCheckRefresh,
                _resourceLoader.GetString("AccountGroupsCheckRefresh"));
            ToolTipService.SetToolTip(AppBarButtonAccountGroupsRemove,
                _resourceLoader.GetString("AccountGroupsRemove"));
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the account group info lists' collection changed event.
        private void AccountGroupInfoLists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleStatusVisibility();
        } // end method AccountGroupInfoLists_CollectionChanged

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is AccountsHelper.PropertyNameIsManaging) ToggleStatusVisibility();
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the click event of the app bar button for checking and refreshing the account group(s).
        private async void AppBarButtonAccountGroupsCheckRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            var appBarButton = sender as AppBarButton;
            var shouldCheckRefreshAccountGroups = appBarButton == AppBarButtonAccountGroupsCheckRefresh;

            if (shouldCheckRefreshAccountGroups) _app.AccountsH.CheckAccountsAsync(true);
            else await _app.AccountsH.CheckAccountAsync(appBarButton?.Tag as string, true, true);
        } // end method AppBarButtonAccountGroupsCheckRefresh_OnClick

        // Handle the click event of the app bar button for removing the account group(s).
        private async void AppBarButtonAccountGroupsRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var appBarButton = sender as AppBarButton;
            var shouldRemoveAccountGroups = appBarButton == AppBarButtonAccountGroupsRemove;

            ContentDialogueAccountGroupsRemove.Content = _resourceLoader.GetString(shouldRemoveAccountGroups
                ? "AccountGroupsRemoveExplanation"
                : "AccountGroupRemoveExplanation");
            ContentDialogueAccountGroupsRemove.Title = _resourceLoader.GetString(shouldRemoveAccountGroups
                ? "AccountsRemoveConfirmation"
                : "AccountRemoveConfirmation");

            if (await ContentDialogueAccountGroupsRemove.ShowAsync() is not ContentDialogResult.Primary) return;

            if (shouldRemoveAccountGroups) _app.AccountsH.RemoveAccounts();
            else _app.AccountsH.RemoveAccount(appBarButton?.Tag as string);
        } // end method AppBarButtonAccountGroupsRemove_OnClick

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

        // Handle the account groups list view's selection changed event.
        private void ListViewAccountGroups_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        } // end method ListViewAccountGroups_OnSelectionChanged

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