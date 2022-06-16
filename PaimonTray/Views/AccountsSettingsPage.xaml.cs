using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using PaimonTray.Models;
using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Numerics;

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
        private App _app;

        /// <summary>
        /// A flag indicating if the program is updating the account groups source.
        /// </summary>
        private bool _isUpdatingAccountGroupsSource;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the accounts settings page.
        /// </summary>
        public AccountsSettingsPage()
        {
            _app = Application.Current as App;
            _isUpdatingAccountGroupsSource = false;
            _mainWindow = _app?.WindowsH.GetExistingMainWindow()?.Win as MainWindow;
            InitializeComponent();
            UpdateUiText();

            CommandBarFlyoutCommandBarAccountGroups.Translation += new Vector3(0, 0, 32);
        } // end constructor AccountsSettingsPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show/Hide the status.
        /// </summary>
        private void ToggleStatusVisibility()
        {
            ContentDialogueAccountGroupsRemove.Hide();

            var resourceLoader = _app.SettingsH.ResLoader;

            if (_app.AccountsH.IsAddingUpdating || _app.AccountsH.IsManaging)
            {
                GridStatusWarning.Visibility = Visibility.Collapsed;
                ProgressRingStatusLoading.Visibility = Visibility.Visible;
                TextBlockStatus.Text = resourceLoader.GetString("StatusLoading");
                GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
            }
            else
            {
                var accountGroupInfoLists = _app.AccountsH.AccountGroupInfoLists
                    .OrderBy(accountGroupInfoList => accountGroupInfoList.Key).ToImmutableList();

                _isUpdatingAccountGroupsSource = true;
                CollectionViewSourceAccountGroups.Source = accountGroupInfoLists;
                _isUpdatingAccountGroupsSource = false;

                if (accountGroupInfoLists.Count > 0) GridStatus.Visibility = Visibility.Collapsed;
                else
                {
                    GridStatusWarning.Visibility = Visibility.Visible;
                    ProgressRingStatusLoading.Visibility = Visibility.Collapsed;
                    TextBlockStatus.Text = resourceLoader.GetString("AccountGroupNoCharacter");
                    GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
                } // end if...else
            } // end if...else
        } // end method ToggleStatusVisibility

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            ComboBoxItemServerCn.Content = resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = resourceLoader.GetString("ServerGlobal");
            ContentDialogueAccountGroupsRemove.CloseButtonText = resourceLoader.GetString("No");
            ContentDialogueAccountGroupsRemove.PrimaryButtonText = resourceLoader.GetString("Yes");
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
            ToolTipService.SetToolTip(AppBarButtonAccountGroupsCheckRefresh,
                resourceLoader.GetString("AccountGroupsCheckRefresh"));
            ToolTipService.SetToolTip(AppBarButtonAccountGroupsRemove, resourceLoader.GetString("AccountGroupsRemove"));
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
            if ((e.PropertyName is AccountsHelper.PropertyNameIsAccountGroupUpdated &&
                 _app.AccountsH.IsAccountGroupUpdated) ||
                e.PropertyName is AccountsHelper.PropertyNameIsAddingUpdating or AccountsHelper.PropertyNameIsManaging)
                ToggleStatusVisibility();
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the accounts settings page's loaded event.
        private void AccountsSettingsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged += AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            ToggleStatusVisibility();

            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            // Show the settings' selection.
            ComboBoxServerDefault.SelectedItem = propertySetSettings[SettingsHelper.KeyServerDefault] switch
            {
                AccountsHelper.TagServerCn => ComboBoxItemServerCn,
                AccountsHelper.TagServerGlobal => ComboBoxItemServerGlobal,
                _ => null
            };
            ToggleSwitchLoginAlternativeAlways.IsOn =
                propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] as bool? ??
                SettingsHelper.DefaultLoginAlternativeAlways;
        } // end method AccountsSettingsPage_OnLoaded

        // Handle the accounts settings page's unloaded event.
        private void AccountsSettingsPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged -= AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            _app = null;
            _mainWindow = null;
        } // end method AccountsSettingsPage_OnUnloaded

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
            var resourceLoader = _app.SettingsH.ResLoader;
            var shouldRemoveAccountGroups = appBarButton == AppBarButtonAccountGroupsRemove;

            ContentDialogueAccountGroupsRemove.Content = resourceLoader.GetString(shouldRemoveAccountGroups
                ? "AccountGroupsRemoveExplanation"
                : "AccountGroupRemoveExplanation");
            ContentDialogueAccountGroupsRemove.Title = resourceLoader.GetString(shouldRemoveAccountGroups
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
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            if (ComboBoxServerDefault.SelectedItem is not ComboBoxItem comboBoxServerDefaultSelectedItem ||
                propertySetSettings[SettingsHelper.KeyServerDefault] as string ==
                comboBoxServerDefaultSelectedItem.Tag as string) return;

            propertySetSettings[SettingsHelper.KeyServerDefault] = comboBoxServerDefaultSelectedItem.Tag as string;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAccountAddUpdate) return;

            InfoBarServerDefaultAppliedLater.IsOpen = true;
            InfoBarServerDefaultAppliedLater.Margin = new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
        } // end method ComboBoxServerDefault_OnSelectionChanged

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
            if (_app.AccountsH.IsAddingUpdating || _app.AccountsH.IsManaging || _isUpdatingAccountGroupsSource) return;

            var haveAddedItems = e.AddedItems.Count > 0;
            var characterSelectionChanged = (haveAddedItems ? e.AddedItems[0] : e.RemovedItems[0]) as AccountCharacter;

            _app.AccountsH.ApplyCharacterStatus(characterSelectionChanged?.Key, characterSelectionChanged?.UidCharacter,
                haveAddedItems);
        } // end method ListViewAccountGroups_OnSelectionChanged

        // Handle the toggled event of the toggle switch of the setting for always using the alternative login method.
        private void ToggleSwitchLoginAlternativeAlways_OnToggled(object sender, RoutedEventArgs e)
        {
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            if (propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] as bool? ==
                ToggleSwitchLoginAlternativeAlways.IsOn) return;

            propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] = ToggleSwitchLoginAlternativeAlways.IsOn;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAccountAddUpdate) return;

            InfoBarLoginAlternativeAlwaysAppliedLater.IsOpen = true;
            InfoBarLoginAlternativeAlwaysAppliedLater.Margin =
                new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
        } // end method ToggleSwitchLoginAlternativeAlways_OnToggled

        #endregion Event Handlers
    } // end class AccountsSettingsPage
} // end namespace PaimonTray.Views