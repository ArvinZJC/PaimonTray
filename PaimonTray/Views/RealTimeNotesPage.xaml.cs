using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaimonTray.Converters;
using PaimonTray.Helpers;
using PaimonTray.Models;
using PaimonTray.ViewModels;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Collections;

namespace PaimonTray.Views
{
    /// <summary>
    /// The real-time notes page.
    /// </summary>
    public sealed partial class RealTimeNotesPage
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// The accounts property set.
        /// </summary>
        private readonly IPropertySet _propertySetAccounts;

        /// <summary>
        /// The resource loader.
        /// </summary>
        private readonly ResourceLoader _resourceLoader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the real-time notes page.
        /// </summary>
        public RealTimeNotesPage()
        {
            _app = Application.Current as App;
            _propertySetAccounts = _app?.AccountsH.ApplicationDataContainerAccounts.Values;
            _resourceLoader = _app?.SettingsH.ResLoader;

            InitializeComponent();
            UpdateUiText();
        } // end constructor RealTimeNotesPage

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
            _mainWindow.MainWinViewModel.PropertyChanged -= MainWindowViewModel_OnPropertyChanged;
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
            ProgressRingStatusLoading.Visibility = Visibility.Collapsed;
            TextBlockStatus.Text = statusText;
            GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
        } // end method ShowAccountGroupNoCharacterStatus

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
                            _propertySetAccounts[AccountsHelper.KeyUidCharacterSelected] as string;

                        foreach (var accountCharacters in accountGroupInfoLists.Select(accountGroupInfoList =>
                                     accountGroupInfoList.Cast<AccountCharacter>()))
                        {
                            ListViewAccountGroups.SelectedItem = uidCharacterSelected is null
                                ? accountCharacters.FirstOrDefault(
                                    accountCharacter => accountCharacter.UidCharacter is not null, null)
                                : accountCharacters.FirstOrDefault(
                                    accountCharacter => accountCharacter.UidCharacter == uidCharacterSelected, null);

                            if (ListViewAccountGroups.SelectedItem is not null) break;
                        } // end foreach

                        GridStatus.Visibility = Visibility.Collapsed; // Hide the status grid when ready.
                    }
                    else ShowAccountGroupNoCharacterStatus(_resourceLoader.GetString("AccountGroupNoCharacterEnabled"));
                }
                else ShowAccountGroupNoCharacterStatus(_resourceLoader.GetString("AccountGroupNoCharacter"));
            } // end if...else
        } // end method ToggleStatusVisibility

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            TextBlockTitle.Text = _resourceLoader.GetString("RealTimeNotes");
            ToolTipService.SetToolTip(ButtonSwitchCharacter, _resourceLoader.GetString("CharacterSwitch"));
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

        // Handle the body grid's size changed event.
        private void GridBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageSize();
        } // end method GridBody_OnSizeChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainWindow =
                _app.WindowsH
                    .GetMainWindow(); // Need to get the main window here to avoid any possible exception, as the page is navigated first when the app starts.
            _mainWindow.MainWinViewModel.PropertyChanged += MainWindowViewModel_OnPropertyChanged;
        } // end method GridRoot_OnLoaded

        // Handle the account groups list view's selection changed event.
        private void ListViewAccountGroups_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewAccountGroups.SelectedItem is not AccountCharacter accountCharacter)
            {
                TextBlockCharacterNickname.Text = string.Empty;
                TextBlockCharacterOtherInfo.Text = string.Empty;
            }
            else
            {
                var accountCharacterConverter = Resources["AccountCharacterConverter"] as AccountCharacterConverter;

                _propertySetAccounts[AccountsHelper.KeyUidCharacterSelected] = accountCharacter.UidCharacter;
                TextBlockCharacterNickname.Text =
                    accountCharacterConverter?.Convert(accountCharacter, null,
                        AccountCharacterConverter.ParameterNicknameCharacter, null) as string;
                TextBlockCharacterOtherInfo.Text =
                    accountCharacterConverter?.Convert(accountCharacter, null,
                        AccountCharacterConverter.ParameterOtherInfoCharacter, null) as string;
            } // end if...else
        } // end method ListViewAccountGroups_OnSelectionChanged

        // Handle the character's real-time notes list view's size changed event.
        private void ListViewCharacterRealTimeNotes_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            GridCharacter.Width = ListViewCharacterRealTimeNotes.ActualWidth;
            GridCharacterRealTimeNotes.Width = GridCharacter.Width;
        } // end method ListViewCharacterRealTimeNotes_OnSizeChanged

        // Handle the main window view model's property changed event.
        private void MainWindowViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is MainWindowViewModel.PropertyNameNavViewPaneDisplayMode) SetPageSize();
        } // end method MainWindowViewModel_OnPropertyChanged

        #endregion Event Handlers
    } // end class RealTimeNotesPage
} // end namespace PaimonTray.Views