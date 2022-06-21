using Microsoft.UI.Dispatching;
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

        #region Event Handlers

        // Handle the account group info lists' collection changed event.
        private void AccountGroupInfoLists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleStatusVisibility();
        } // end method AccountGroupInfoLists_CollectionChanged

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == AccountsHelper.PropertyNameIsAccountGroupUpdated &&
                 _app.AccountsH.IsAccountGroupUpdated) ||
                e.PropertyName == AccountsHelper.PropertyNameIsAddingUpdating ||
                e.PropertyName == AccountsHelper.PropertyNameIsManaging) ToggleStatusVisibility();
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the accounts settings page's loaded event.
        private void AccountsSettingsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged += AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            _dispatcherQueueTimer = DispatcherQueue.CreateTimer();
            _dispatcherQueueTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherQueueTimer.Tick += DispatcherQueueTimer_OnTick;
            ToggleStatusVisibility();

            var propertySetSettings = _app.SettingsH.PropertySetSettings;
            var realTimeNotesIntervalRefresh =
                propertySetSettings[SettingsHelper.KeyRealTimeNotesIntervalRefresh] as int?;
            var serverDefault = propertySetSettings[SettingsHelper.KeyServerDefault] as string;

            // Show the settings' selection.
            if (realTimeNotesIntervalRefresh == SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther1)
                ComboBoxRealTimeNotesIntervalRefresh.SelectedItem =
                    ComboBoxItemRealTimeNotesIntervalRefreshOptionOther1;
            else if (realTimeNotesIntervalRefresh == SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther2)
                ComboBoxRealTimeNotesIntervalRefresh.SelectedItem =
                    ComboBoxItemRealTimeNotesIntervalRefreshOptionOther2;
            else if (realTimeNotesIntervalRefresh == SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther3)
                ComboBoxRealTimeNotesIntervalRefresh.SelectedItem =
                    ComboBoxItemRealTimeNotesIntervalRefreshOptionOther3;
            else if (realTimeNotesIntervalRefresh == SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther4)
                ComboBoxRealTimeNotesIntervalRefresh.SelectedItem =
                    ComboBoxItemRealTimeNotesIntervalRefreshOptionOther4;
            else if (realTimeNotesIntervalRefresh == SettingsHelper.TagRealTimeNotesIntervalRefreshResinOriginal)
                ComboBoxRealTimeNotesIntervalRefresh.SelectedItem =
                    ComboBoxItemRealTimeNotesIntervalRefreshResinOriginal;
            else ComboBoxRealTimeNotesIntervalRefresh.SelectedItem = null;

            if (serverDefault == AccountsHelper.TagServerCn) ComboBoxServerDefault.SelectedItem = ComboBoxItemServerCn;
            else if (serverDefault == AccountsHelper.TagServerGlobal)
                ComboBoxServerDefault.SelectedItem = ComboBoxItemServerGlobal;
            else ComboBoxServerDefault.SelectedItem = null;

            ToggleSwitchAccountGroupsCheckRefreshWhenAppStarts.IsOn =
                propertySetSettings[SettingsHelper.KeyAccountGroupsCheckRefreshWhenAppStarts] as bool? ??
                SettingsHelper.DefaultAccountGroupsCheckRefreshWhenAppStarts;
            ToggleSwitchLoginAlternativeAlways.IsOn =
                propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] as bool? ??
                SettingsHelper.DefaultLoginAlternativeAlways;
            _dispatcherQueueTimer.Start(); // Start the dispatcher queue timer when ready.
        } // end method AccountsSettingsPage_OnLoaded

        // Handle the accounts settings page's unloaded event.
        private void AccountsSettingsPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged -= AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            _app = null;
            _dispatcherQueueTimer.Stop();
            _dispatcherQueueTimer.Tick -= DispatcherQueueTimer_OnTick;
            _mainWindow = null;
        } // end method AccountsSettingsPage_OnUnloaded

        // Handle the click event of the app bar button for checking and refreshing the account group(s).
        private async void AppBarButtonAccountGroupsCheckRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            var appBarButton = sender as AppBarButton;
            var shouldCheckRefreshAccountGroups = appBarButton == AppBarButtonAccountGroupsCheckRefresh;

            if (shouldCheckRefreshAccountGroups) _app.AccountsH.CheckAccountsAsync();
            else await _app.AccountsH.CheckAccountAsync(appBarButton?.Tag as string, true);
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

#pragma warning disable CA1822 // Mark members as static
        // Handle the combo box's drop-down closed event.
        private void ComboBox_OnDropDownClosed(object sender, object e)
#pragma warning restore CA1822 // Mark members as static
        {
            if (sender is not ComboBox comboBox) return;

            comboBox.MinWidth = 0;
        } // end method ComboBox_OnDropDownClosed

        // Handle the real-time notes refresh interval combo box item's loaded event.
        private void ComboBoxItemRealTimeNotesIntervalRefresh_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemRealTimeNotesIntervalRefreshActualWidth = (sender as ComboBoxItem)?.ActualWidth ?? 0;

            if (ComboBoxRealTimeNotesIntervalRefresh.MinWidth < comboBoxItemRealTimeNotesIntervalRefreshActualWidth)
                ComboBoxRealTimeNotesIntervalRefresh.MinWidth = comboBoxItemRealTimeNotesIntervalRefreshActualWidth;
        } // end method ComboBoxItemRealTimeNotesIntervalRefresh_OnLoaded

        // Handle the server combo box item's loaded event.
        private void ComboBoxItemServer_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemServerActualWidth = (sender as ComboBoxItem)?.ActualWidth ?? 0;

            if (ComboBoxServerDefault.MinWidth < comboBoxItemServerActualWidth)
                ComboBoxServerDefault.MinWidth = comboBoxItemServerActualWidth;
        } // end method ComboBoxItemServer_OnLoaded

        // Handle the real-time notes refresh interval combo box's selection changed event.
        private void ComboBoxRealTimeNotesIntervalRefresh_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            // It is necessary to ignore the case when the selected real-time notes refresh interval is the same as the stored.
            if (ComboBoxRealTimeNotesIntervalRefresh.SelectedItem is not ComboBoxItem
                    comboBoxRealTimeNotesIntervalRefreshSelectedItem ||
                propertySetSettings[SettingsHelper.KeyRealTimeNotesIntervalRefresh] as int? ==
                comboBoxRealTimeNotesIntervalRefreshSelectedItem.Tag as int?) return;

            propertySetSettings[SettingsHelper.KeyRealTimeNotesIntervalRefresh] =
                comboBoxRealTimeNotesIntervalRefreshSelectedItem.Tag as int?; // Update the setting value first.
            _app.AccountsH.SetRealTimeNotesDispatcherQueueTimerInterval();
        } // end method ComboBoxRealTimeNotesIntervalRefresh_OnSelectionChanged

        // Handle the default server combo box's selection changed event.
        private void ComboBoxServerDefault_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            // It is necessary to ignore the case when the selected default server is the same as the stored.
            if (ComboBoxServerDefault.SelectedItem is not ComboBoxItem comboBoxServerDefaultSelectedItem ||
                propertySetSettings[SettingsHelper.KeyServerDefault] as string ==
                comboBoxServerDefaultSelectedItem.Tag as string) return;

            propertySetSettings[SettingsHelper.KeyServerDefault] = comboBoxServerDefaultSelectedItem.Tag as string;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAccountAddUpdate) return;

            InfoBarServerDefaultAppliedLater.IsOpen = true;
            InfoBarServerDefaultAppliedLater.Margin = new Thickness(0, 0, 0, AppFieldsHelper.InfoBarMarginBottom);
        } // end method ComboBoxServerDefault_OnSelectionChanged

        // Handle the dispatcher queue timer's tick event.
        private void DispatcherQueueTimer_OnTick(object sender, object e)
        {
            if (_accountGroupInfoListsTimeLocalRefreshLast is null) return;

            TimeZoneInfo.ClearCachedData();

            var accountGroupInfoListsTimeLocalRefreshLast = (DateTimeOffset)_accountGroupInfoListsTimeLocalRefreshLast;

            if (accountGroupInfoListsTimeLocalRefreshLast.Offset != DateTimeOffset.Now.Offset ||
                accountGroupInfoListsTimeLocalRefreshLast.Date != DateTimeOffset.Now.Date) ToggleStatusVisibility();
        } // end method DispatcherQueueTimer_OnTick

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

        // Handle the toggled event of the toggle switch of the setting for checking and refreshing all accounts when the app starts.
        private void ToggleSwitchAccountGroupsCheckRefreshWhenAppStarts_OnToggled(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyAccountGroupsCheckRefreshWhenAppStarts] =
                ToggleSwitchAccountGroupsCheckRefreshWhenAppStarts.IsOn;
        } // end method ToggleSwitchAccountGroupsCheckRefreshWhenAppStarts_OnToggled

        // Handle the toggled event of the toggle switch of the setting for always using the alternative login method.
        private void ToggleSwitchLoginAlternativeAlways_OnToggled(object sender, RoutedEventArgs e)
        {
            _app.SettingsH.PropertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] =
                ToggleSwitchLoginAlternativeAlways.IsOn;

            if (_mainWindow.NavigationViewBody.SelectedItem as NavigationViewItem !=
                _mainWindow.NavigationViewItemBodyAccountAddUpdate) return;

            InfoBarLoginAlternativeAlwaysAppliedLater.IsOpen = true;
            InfoBarLoginAlternativeAlwaysAppliedLater.Margin =
                new Thickness(0, 0, 0, AppFieldsHelper.InfoBarMarginBottom);
        } // end method ToggleSwitchLoginAlternativeAlways_OnToggled

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The account group info lists' last refresh local time.
        /// </summary>
        private DateTimeOffset? _accountGroupInfoListsTimeLocalRefreshLast;

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The dispatcher queue timer.
        /// </summary>
        private DispatcherQueueTimer _dispatcherQueueTimer;

        /// <summary>
        /// A flag indicating if the program is updating the account groups source.
        /// </summary>
        private bool _isUpdatingAccountGroupsSource;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Generate the real-time notes refresh interval combo box item's content.
        /// </summary>
        /// <param name="optionValue">The option value.</param>
        /// <param name="resinOriginal">The Original Resin name.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <param name="isHour">A flag indicating if the time unit is an hour.</param>
        /// <returns>The real-time notes refresh interval combo box item's content.</returns>
        private static string GenerateRealTimeNotesRefreshIntervalComboBoxItemContent(int optionValue,
            string resinOriginal,
            string timeUnit, bool isHour = false)
        {
            var optionValueDisplay = isHour ? optionValue / 60.0 : optionValue;

            return
                $"{optionValueDisplay} {timeUnit} | {(double)optionValue / SettingsHelper.TagRealTimeNotesIntervalRefreshResinOriginal} {resinOriginal}";
        } // end method GenerateRealTimeNotesRefreshIntervalComboBoxItemContent

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
                _accountGroupInfoListsTimeLocalRefreshLast = DateTimeOffset.Now; // Record the time when ready.
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
            var resourceLoader = _app.SettingsH.ResLoader; // Get the resource loader first.
            var minutes = resourceLoader.GetString("Minutes");
            var resinOriginal = resourceLoader.GetString("ResinOriginal");

            ComboBoxItemRealTimeNotesIntervalRefreshOptionOther1.Content =
                GenerateRealTimeNotesRefreshIntervalComboBoxItemContent(
                    SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther1, resinOriginal, minutes);
            ComboBoxItemRealTimeNotesIntervalRefreshOptionOther2.Content =
                GenerateRealTimeNotesRefreshIntervalComboBoxItemContent(
                    SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther2, resinOriginal, minutes);
            ComboBoxItemRealTimeNotesIntervalRefreshOptionOther3.Content =
                GenerateRealTimeNotesRefreshIntervalComboBoxItemContent(
                    SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther3, resinOriginal, minutes);
            ComboBoxItemRealTimeNotesIntervalRefreshOptionOther4.Content =
                GenerateRealTimeNotesRefreshIntervalComboBoxItemContent(
                    SettingsHelper.TagRealTimeNotesIntervalRefreshOptionOther4, resinOriginal,
                    resourceLoader.GetString("Hour"), true);
            ComboBoxItemRealTimeNotesIntervalRefreshResinOriginal.Content =
                GenerateRealTimeNotesRefreshIntervalComboBoxItemContent(
                    SettingsHelper.TagRealTimeNotesIntervalRefreshResinOriginal, resinOriginal, minutes);
            ComboBoxItemServerCn.Content = resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = resourceLoader.GetString("ServerGlobal");
            ContentDialogueAccountGroupsRemove.CloseButtonText = resourceLoader.GetString("No");
            ContentDialogueAccountGroupsRemove.PrimaryButtonText = resourceLoader.GetString("Yes");
            InfoBarLoginAlternativeAlwaysAppliedLater.Title =
                resourceLoader.GetString("LoginAlternativeAlwaysAppliedLater");
            InfoBarServerDefaultAppliedLater.Title = resourceLoader.GetString("ServerDefaultAppliedLater");
            TextBlockAccountGroupsCheckRefreshWhenAppStarts.Text =
                resourceLoader.GetString("AccountGroupsCheckRefreshWhenAppStarts");
            TextBlockAccountsManagement.Text = resourceLoader.GetString("AccountsManagement");
            TextBlockAccountsManagementExplanation.Text = resourceLoader.GetString("AccountsManagementExplanation");
            TextBlockLoginAlternativeAlways.Text = resourceLoader.GetString("LoginAlternativeAlways");
            TextBlockLoginAlternativeAlwaysExplanation.Text =
                resourceLoader.GetString("LoginAlternativeAlwaysExplanation");
            TextBlockRealTimeNotesIntervalRefresh.Text = resourceLoader.GetString("RealTimeNotesIntervalRefresh");
            TextBlockRealTimeNotesIntervalRefreshExplanation.Text =
                resourceLoader.GetString("RealTimeNotesIntervalRefreshExplanation");
            TextBlockServerDefault.Text = resourceLoader.GetString("ServerDefault");
            TextBlockServerDefaultExplanation.Text = resourceLoader.GetString("ServerDefaultExplanation");
            ToolTipService.SetToolTip(AppBarButtonAccountGroupsCheckRefresh,
                resourceLoader.GetString("AccountGroupsCheckRefresh"));
            ToolTipService.SetToolTip(AppBarButtonAccountGroupsRemove, resourceLoader.GetString("AccountGroupsRemove"));
        } // end method UpdateUiText

        #endregion Methods
    } // end class AccountsSettingsPage
} // end namespace PaimonTray.Views