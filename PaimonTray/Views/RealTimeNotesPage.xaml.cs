using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Converters;
using PaimonTray.Helpers;
using PaimonTray.Models;
using PaimonTray.ViewModels;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace PaimonTray.Views
{
    /// <summary>
    /// The real-time notes page.
    /// </summary>
    public sealed partial class RealTimeNotesPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the real-time notes page.
        /// </summary>
        public RealTimeNotesPage()
        {
            _app = Application.Current as App;
            _mainWindow = _app?.WindowsH.GetExistingMainWindow()?.Win as MainWindow;

            InitializeComponent();
            UpdateUiText();
        } // end constructor RealTimeNotesPage

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
            if ((e.PropertyName is AccountsHelper.PropertyNameIsAccountCharacterUpdated &&
                 _app.AccountsH.IsAccountCharacterUpdated) ||
                (e.PropertyName is AccountsHelper.PropertyNameIsAccountGroupUpdated &&
                 _app.AccountsH.IsAccountGroupUpdated) ||
                e.PropertyName is AccountsHelper.PropertyNameIsManaging) ToggleStatusVisibility();

            var uidCharacterRealTimeNotesUpdated = _app.AccountsH.UidCharacterRealTimeNotesUpdated;

            if (e.PropertyName is not AccountsHelper.PropertyNameUidCharacterRealTimeNotesUpdated ||
                string.IsNullOrWhiteSpace(uidCharacterRealTimeNotesUpdated)) return;

            if (ListViewAccountGroups.SelectedItem is not AccountCharacter accountCharacter) return;

            if (uidCharacterRealTimeNotesUpdated is not AccountsHelper.TagRealTimeNotesUpdatedCharactersAllEnabled &&
                uidCharacterRealTimeNotesUpdated != accountCharacter.UidCharacter) return;

            UpdateRealTimeNotesArea(accountCharacter.Key, accountCharacter.UidCharacter);
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the midnight dispatcher timer's tick event.
        private void DispatcherTimerMidnight_OnTick(object sender, object e)
        {
            ToggleStatusVisibility();

            if (ListViewAccountGroups.SelectedItem is not AccountCharacter accountCharacter) return;

            UpdateRealTimeNotesArea(accountCharacter.Key, accountCharacter.UidCharacter);
        } // end method DispatcherTimerMidnight_OnTick

        // Handle the body grid's size changed event.
        private void GridBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageSize();
        } // end method GridBody_OnSizeChanged

        // Handle the account groups list view's selection changed event.
        // NOTE: The list view's tag is used to store the UID from the last accepted selected item.
        private void ListViewAccountGroups_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonCharacterSwitch.Flyout.Hide();

            var propertySetAccounts = _app.AccountsH.ApplicationDataContainerAccounts.Values;
            var uidCharacterSelected = propertySetAccounts[AccountsHelper.KeyUidCharacterSelected] as string;

            if (ListViewAccountGroups.SelectedItem is not AccountCharacter accountCharacter)
            {
                // Ignore the case when the selected item is invalid but the selected character exists.
                if (uidCharacterSelected is not null) return;

                InitialiseRealTimeNotesArea();
                ListViewAccountGroups.Tag = null;
                TextBlockNicknameCharacter.Text = AppConstantsHelper.Unknown;
                TextBlockOtherInfoCharacter.Text = AppConstantsHelper.Unknown;
            }
            else
            {
                var accountCharacterConverter = Resources["AccountCharacterConverter"] as AccountCharacterConverter;
                var nicknameCharacter =
                    accountCharacterConverter?.Convert(accountCharacter, null,
                        AccountCharacterConverter.ParameterNicknameCharacter, null) as string ??
                    AppConstantsHelper.Unknown;
                var otherInfoCharacter =
                    accountCharacterConverter?.Convert(accountCharacter, null,
                        AccountCharacterConverter.ParameterOtherInfoCharacter, null) as string ??
                    AppConstantsHelper.Unknown;

                if (uidCharacterSelected != accountCharacter.UidCharacter)
                    propertySetAccounts[AccountsHelper.KeyUidCharacterSelected] = accountCharacter.UidCharacter;

                TextBlockNicknameCharacter.Text = nicknameCharacter;
                TextBlockOtherInfoCharacter.Text = otherInfoCharacter;
                ToolTipService.SetToolTip(TextBlockNicknameCharacter, nicknameCharacter);
                ToolTipService.SetToolTip(TextBlockOtherInfoCharacter, otherInfoCharacter);

                if (ListViewAccountGroups.Tag as string == accountCharacter.UidCharacter) return;

                UpdateRealTimeNotesArea(accountCharacter.Key, accountCharacter.UidCharacter);
                ListViewAccountGroups.Tag =
                    accountCharacter.UidCharacter; // Store the UID from the selected item when ready.
            } // end if...else
        } // end method ListViewAccountGroups_OnSelectionChanged

        // Handle the character's real-time notes' general section list view's size changed event.
        private void ListViewCharacterRealTimeNotesGeneral_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridCharacter.Width = ListViewCharacterRealTimeNotesGeneral.ActualWidth;
            GridCharacterRealTimeNotes.Width = GridCharacter.Width;
            GridCharacterRealTimeNotesTimeUpdateLast.MaxWidth = GridCharacter.Width;
            ListViewCharacterRealTimeNotesExpeditions.MaxWidth = GridCharacter.Width;
            TextBlockTitle.MaxWidth = GridCharacter.Width;
        } // end method ListViewCharacterRealTimeNotesGeneral_OnSizeChanged

        // Handle the main window view model's property changed event.
        private void MainWindowViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is MainWindowViewModel.PropertyNameNavViewPaneDisplayMode) SetPageSize();
        } // end method MainWindowViewModel_OnPropertyChanged

        // Handle the real-time notes page's loaded event.
        private void RealTimeNotesPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged += AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            _mainWindow.MainWinViewModel.PropertyChanged += MainWindowViewModel_OnPropertyChanged;
            SetMidnightDispatcherTimerInterval();
            ToggleStatusVisibility();
        } // end method RealTimeNotesPage_OnLoaded

        // Handle the real-time notes page's unloaded event.
        private void RealTimeNotesPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.AccountGroupInfoLists.CollectionChanged -= AccountGroupInfoLists_CollectionChanged;
            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            _dispatcherTimerMidnight.Stop();
            _dispatcherTimerMidnight.Tick -= DispatcherTimerMidnight_OnTick;
            _mainWindow.MainWinViewModel.PropertyChanged -= MainWindowViewModel_OnPropertyChanged;

            _app = null;
            _mainWindow = null;
        } // end method RealTimeNotesPage_OnUnloaded

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The midnight dispatcher timer.
        /// </summary>
        private DispatcherTimer _dispatcherTimerMidnight;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initialise the real-time notes area.
        /// </summary>
        private void InitialiseRealTimeNotesArea()
        {
            GridCharacterRealTimeNotesStatusDisabled.Visibility = Visibility.Collapsed;
            GridCharacterRealTimeNotesStatusFail.Visibility = Visibility.Collapsed;
            GridCharacterRealTimeNotesStatusReady.Visibility = Visibility.Collapsed;
            GridCharacterRealTimeNotesStatusUpdating.Visibility = Visibility.Collapsed;
            ListViewCharacterRealTimeNotesExpeditions.ItemsSource = null;
            ListViewCharacterRealTimeNotesGeneral.ItemsSource = null;
            ListViewHeaderItemCharacterRealTimeNotesExpeditions.Visibility = Visibility.Collapsed;
            RunCharacterRealTimeNotesTimeUpdateLast.Text = null;
            TextBlockCharacterRealTimeNotesExpeditionsExplanation.Text = null;
            TextBlockCharacterRealTimeNotesExpeditionsStatus.Text = null;
            TextBlockCharacterRealTimeNotesExpeditionsTitle.Text = null;
        } // end method InitialiseRealTimeNotesArea

        /// <summary>
        /// Set the midnight dispatcher timer's interval.
        /// </summary>
        private void SetMidnightDispatcherTimerInterval()
        {
            if (_dispatcherTimerMidnight is null)
            {
                _dispatcherTimerMidnight = new DispatcherTimer();
                _dispatcherTimerMidnight.Tick += DispatcherTimerMidnight_OnTick; // Add the tick event handler first.
            } // end if

            _dispatcherTimerMidnight.Interval = AccountsHelper.GetTimeSpanBetweenCurrentAndMidnight();

            if (!_dispatcherTimerMidnight.IsEnabled)
                _dispatcherTimerMidnight.Start(); // The 1st tick occurs when the timer interval has elapsed.
        } // end method SetMidnightDispatcherTimerInterval

        /// <summary>
        /// Set the page size and other controls' sizes related to the page size.
        /// </summary>
        private void SetPageSize()
        {
            var pageMaxSize = _app.WindowsH.GetMainWindowPageMaxSize();

            Height = pageMaxSize.Height < GridBody.ActualHeight ? pageMaxSize.Height : GridBody.ActualHeight;
            ListViewAccountGroups.MaxHeight = Height * 2 / 3;
            Width = pageMaxSize.Width < GridBody.ActualWidth ? pageMaxSize.Width : GridBody.ActualWidth;
            ListViewAccountGroups.Width = Width;
        } // end method SetPageSize

        /// <summary>
        /// Show the status indicating no character (enabled) in the account groups.
        /// </summary>
        /// <param name="statusText">The status text.</param>
        private void ShowAccountGroupNoCharacterStatus(string statusText)
        {
            GridStatusWarning.Visibility = Visibility.Visible;
            InitialiseRealTimeNotesArea();
            ProgressRingStatusLoading.Visibility = Visibility.Collapsed;
            TextBlockStatus.Text = statusText;
            GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
        } // end method ShowAccountGroupNoCharacterStatus

        /// <summary>
        /// Show/Hide the status.
        /// </summary>
        private void ToggleStatusVisibility()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            if (_app.AccountsH.IsManaging)
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

                CollectionViewSourceAccountGroups.Source = accountGroupInfoLists;

                if (accountGroupInfoLists.Count > 0)
                {
                    var hasCharacterEnabled = false;

                    foreach (var accountGroupInfoList in accountGroupInfoLists)
                    {
                        hasCharacterEnabled = accountGroupInfoList.Cast<AccountCharacter>().Any(accountCharacter =>
                            accountCharacter.IsEnabled && accountCharacter.UidCharacter is not null);

                        if (hasCharacterEnabled) break;
                    } // end foreach

                    if (hasCharacterEnabled)
                    {
                        var uidCharacterSelected =
                            _app.AccountsH.ApplicationDataContainerAccounts.Values[
                                AccountsHelper.KeyUidCharacterSelected] as string;
                        AccountCharacter accountCharacterSelected = null;

                        foreach (var accountCharacters in accountGroupInfoLists.Select(accountGroupInfoList =>
                                     accountGroupInfoList.Cast<AccountCharacter>()))
                        {
                            accountCharacterSelected = uidCharacterSelected is null
                                ? accountCharacters.FirstOrDefault(
                                    accountCharacter =>
                                        accountCharacter.UidCharacter is not null && accountCharacter.IsEnabled, null)
                                : accountCharacters.FirstOrDefault(
                                    accountCharacter => accountCharacter.UidCharacter == uidCharacterSelected, null);

                            if (accountCharacterSelected is not null) break;
                        } // end foreach

                        ListViewAccountGroups.SelectedItem = accountCharacterSelected;
                        GridStatus.Visibility = Visibility.Collapsed; // Hide the status grid when ready.
                    }
                    else ShowAccountGroupNoCharacterStatus(resourceLoader.GetString("AccountGroupNoCharacterEnabled"));
                }
                else ShowAccountGroupNoCharacterStatus(resourceLoader.GetString("AccountGroupNoCharacter"));
            } // end if...else
        } // end method ToggleStatusVisibility

        /// <summary>
        /// Update the real-time notes area.
        /// </summary>
        /// <param name="containerKeyAccount">The account container key.</param>
        /// <param name="containerKeyCharacter">The character container key.</param>
        /// <returns>A task just to indicate that any later operation needs to wait.</returns>
        private void UpdateRealTimeNotesArea(string containerKeyAccount, string containerKeyCharacter)
        {
            var (realTimeNotesExpeditionsHeader, realTimeNotesExpeditionNotes, realTimeNotesGeneralNotes,
                    realTimeNotesStatus, realTimeNotesTimeUpdateLast) =
                _app.AccountsH.GetRealTimeNotes(containerKeyAccount, containerKeyCharacter);

            GridCharacterRealTimeNotesStatusDisabled.Visibility =
                realTimeNotesStatus is AccountsHelper.TagStatusDisabled ? Visibility.Visible : Visibility.Collapsed;
            GridCharacterRealTimeNotesStatusFail.Visibility =
                realTimeNotesStatus is null or AccountsHelper.TagStatusFail
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            GridCharacterRealTimeNotesStatusReady.Visibility = realTimeNotesStatus is AccountsHelper.TagStatusReady
                ? Visibility.Visible
                : Visibility.Collapsed;
            GridCharacterRealTimeNotesStatusUpdating.Visibility =
                realTimeNotesStatus is AccountsHelper.TagStatusUpdating ? Visibility.Visible : Visibility.Collapsed;
            ListViewCharacterRealTimeNotesExpeditions.ItemsSource = realTimeNotesExpeditionNotes;
            ListViewCharacterRealTimeNotesGeneral.ItemsSource = realTimeNotesGeneralNotes;
            ListViewHeaderItemCharacterRealTimeNotesExpeditions.Visibility = Visibility.Visible;
            RunCharacterRealTimeNotesTimeUpdateLast.Text = realTimeNotesTimeUpdateLast;
            TextBlockCharacterRealTimeNotesExpeditionsExplanation.Text = realTimeNotesExpeditionsHeader.Explanation;
            TextBlockCharacterRealTimeNotesExpeditionsStatus.Text = realTimeNotesExpeditionsHeader.Status;
            TextBlockCharacterRealTimeNotesExpeditionsTitle.Text = realTimeNotesExpeditionsHeader.Title;
        } // end method UpdateRealTimeNotesArea

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            RunCharacterRealTimeNotesUpdatedLast.Text =
                $"{resourceLoader.GetString("UpdatedLast")}{resourceLoader.GetString("Colon")}";
            TextBlockTitle.Text = resourceLoader.GetString("RealTimeNotes");
            ToolTipService.SetToolTip(ButtonCharacterSwitch, resourceLoader.GetString("CharacterSwitch"));
            ToolTipService.SetToolTip(FontIconCharacterRealTimeNotesStatusDisabled,
                resourceLoader.GetString("RealTimeNotesStatusDisabledExplanation"));
            ToolTipService.SetToolTip(FontIconCharacterRealTimeNotesStatusFail,
                resourceLoader.GetString("RealTimeNotesStatusFailExplanation"));
            ToolTipService.SetToolTip(FontIconCharacterRealTimeNotesStatusReady,
                resourceLoader.GetString("RealTimeNotesStatusReadyExplanation"));
            ToolTipService.SetToolTip(FontIconCharacterRealTimeNotesStatusUpdating,
                resourceLoader.GetString("RealTimeNotesStatusUpdatingExplanation"));
        } // end method UpdateUiText

        #endregion Methods
    } // end class RealTimeNotesPage
} // end namespace PaimonTray.Views