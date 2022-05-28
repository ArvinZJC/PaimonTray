using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaimonTray.Helpers;
using PaimonTray.Models;
using PaimonTray.ViewModels;
using System.Collections.Specialized;
using System.ComponentModel;
using Windows.ApplicationModel.Resources;

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
            _resourceLoader = _app?.SettingsH.ResLoader;

            InitializeComponent();
            ToggleStatusVisibility();
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
            _app.AccountsH.GroupedCharacters.CollectionChanged -= GroupedCharacters_CollectionChanged;
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
            _app.AccountsH.GroupedCharacters.CollectionChanged += GroupedCharacters_CollectionChanged;
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            CollectionViewSourceGroupedCharacters.Source = _app.AccountsH.GroupedCharacters;
            base.OnNavigatedTo(e);
        } // end method OnNavigatedTo

        /// <summary>
        /// Set the page size and other controls' sizes related to the page size.
        /// </summary>
        private void SetPageSize()
        {
            var pageMaxSize = _app.WindowsH.GetMainWindowPageMaxSize();

            Height = pageMaxSize.Height < GridBody.ActualHeight ? pageMaxSize.Height : GridBody.ActualHeight;
            ListViewGroupedCharacters.MaxHeight = Height * 2 / 3;
            Width = pageMaxSize.Width < GridBody.ActualWidth ? pageMaxSize.Width : GridBody.ActualWidth;
            ListViewGroupedCharacters.MaxWidth = Width;
        } // end method SetPageSize

        /// <summary>
        /// Show/Hide the status.
        /// </summary>
        private void ToggleStatusVisibility()
        {
            if (_app.AccountsH.IsChecking)
            {
                GridStatusWarning.Visibility = Visibility.Collapsed;
                ProgressRingStatusLoading.Visibility = Visibility.Visible;
                TextBlockStatus.Text = _resourceLoader.GetString("StatusLoading");
                GridStatus.Visibility = Visibility.Visible; // Show the status grid when ready.
            }
            else
            {
                if (_app.AccountsH.GroupedCharacters.Count > 0) GridStatus.Visibility = Visibility.Collapsed;
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
            TextBlockTitle.Text = _resourceLoader.GetString("RealTimeNotes");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is AccountsHelper.PropertyNameIsChecking) ToggleStatusVisibility();
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

        // Handle the grouped characters' collection changed event.
        private void GroupedCharacters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ToggleStatusVisibility();
        } // end method GroupedCharacters_CollectionChanged

        // Handle the grouped characters list view's selection changed event.
        private void ListViewGroupedCharacters_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Serilog.Log.Debug((ListViewGroupedCharacters.SelectedItem as AccountCharacter)?.CUid); // TODO
        } // end method ListViewGroupedCharacters_OnSelectionChanged

        // Handle the main window view model's property changed event.
        private void MainWindowViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is MainWindowViewModel.PropertyNameNavViewPaneDisplayMode) SetPageSize();
        } // end method MainWindowViewModel_OnPropertyChanged

        #endregion Event Handlers
    } // end class RealTimeNotesPage
} // end namespace PaimonTray.Views