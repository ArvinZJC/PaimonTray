using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using PaimonTray.Models;
using PaimonTray.ViewModels;
using System.Collections.ObjectModel;
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

        private readonly ObservableCollection<GroupInfoList> _groupedCharacters;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the real-time notes page.
        /// </summary>
        public RealTimeNotesPage()
        {
            _app = Application.Current as App;
            _groupedCharacters = _app?.AccountsH.GetGroupedCharactersFromLocal();

            InitializeComponent();
            UpdateUiText();
            ToggleGridAccountAddCharacterVisibility();

            if (_groupedCharacters != null) _groupedCharacters.CollectionChanged += GroupedCharacters_CollectionChanged;

            CollectionViewSourceCharacters.Source = _groupedCharacters;
        } // end constructor RealTimeNotesPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Invoked immediately after the page is unloaded and is no longer the current source of a parent frame.
        /// </summary>
        /// <param name="e">Details about the navigation that has unloaded the current page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _mainWindow.MainWinViewModel.PropertyChanged -= MainWindowViewModel_OnPropertyChanged;
            _groupedCharacters.CollectionChanged -= GroupedCharacters_CollectionChanged;
            base.OnNavigatedFrom(e);
        } // end method OnNavigatedFrom

        /// <summary>
        /// Set the page size.
        /// </summary>
        private void SetPageSize()
        {
            var pageMaxSize = _app.WindowsH.GetMainWindowPageMaxSize();

            Height = pageMaxSize.Height < GridBody.ActualHeight ? pageMaxSize.Height : GridBody.ActualHeight;
            Width = pageMaxSize.Width < GridBody.ActualWidth ? pageMaxSize.Width : GridBody.ActualWidth;
        } // end method SetPageSize

        /// <summary>
        /// Show/Hide the grid for indicating no character.
        /// </summary>
        private void ToggleGridAccountAddCharacterVisibility()
        {
            GridAccountAddCharacter.Visibility =
                _groupedCharacters.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        } // end method ToggleGridAccountAddCharacterVisibility

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            TextBlockAccountAddCharacter.Text = resourceLoader.GetString("AccountAddCharacter");
            TextBlockTitle.Text = resourceLoader.GetString("RealTimeNotes");
            ToolTipService.SetToolTip(ButtonAccountAddCharacter, resourceLoader.GetString("AccountAddUpdate"));
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the click event of the button for navigating to adding/updating an account.
        private void ButtonAccountAddCharacter_OnClick(object sender, RoutedEventArgs e)
        {
            _mainWindow.NavigationViewBody.SelectedItem = _mainWindow.NavigationViewItemBodyAddUpdateAccount;
        } // end method ButtonAccountAddCharacter_OnClick

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
            ToggleGridAccountAddCharacterVisibility();
        } // end method GroupedCharacters_CollectionChanged

        // Handle the main window view model's property changed event.
        private void MainWindowViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MainWindowViewModel.PropertyNameNavViewPaneDisplayMode) SetPageSize();
        } // end method MainWindowViewModel_OnPropertyChanged

        #endregion Event Handlers
    } // end class RealTimeNotesPage
} // end namespace PaimonTray.Views