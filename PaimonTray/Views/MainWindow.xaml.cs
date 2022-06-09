using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using PaimonTray.Helpers;
using PaimonTray.Models;
using PaimonTray.ViewModels;
using Serilog;
using System;
using System.ComponentModel;
using Windows.ApplicationModel;
using Windows.Foundation.Collections;
using Windows.Graphics;

namespace PaimonTray.Views
{
    /// <summary>
    /// The main window.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// The app window.
        /// </summary>
        private AppWindow _appWindow;

        /// <summary>
        /// A flag indicating if it is the 1st time the window is loaded.
        /// </summary>
        private bool _isFirstLoad;

        /// <summary>
        /// The existing window.
        /// </summary>
        private ExistingWindow _existingWindow;

        /// <summary>
        /// The settings property set.
        /// </summary>
        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The main window view model.
        /// </summary>
        public MainWindowViewModel MainWinViewModel { get; }

        /// <summary>
        /// The parameter for navigating to the real-time notes page.
        /// </summary>
        public object RealTimeNotesPageParameter { get; set; }

        /// <summary>
        /// The main window's window ID.
        /// </summary>
        public WindowId WinId { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the main window. No need to activate it manually for the 1st time.
        /// </summary>
        public MainWindow()
        {
            _app = Application.Current as App;
            _isFirstLoad = true;
            _propertySetSettings = _app?.SettingsH.PropertySetSettings;
            MainWinViewModel = new MainWindowViewModel();
            InitializeComponent();
            CustomiseWindow();
            UpdateUiText();

            MenuFlyoutItemMainMenuHelpLogsShow.CommandParameter = _app?.LogsDirectory;
            TaskbarIconApp.Visibility = Visibility.Visible; // Show the taskbar icon when ready.
            GridRoot.Background =
                new SolidColorBrush(((SolidColorBrush)GridRoot.Resources["RootGridAcrylicBackground"]).Color);
        } // end constructor MainWindow

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Customise the window.
        /// </summary>
        private void CustomiseWindow()
        {
            Title = Package.Current.DisplayName;
            WinId = WindowsHelper.GetWindowId(this);
            _appWindow = WindowsHelper.GetAppWindow(WinId);

            if (_appWindow is null)
            {
                Log.Warning("The main window's AppWindow is null.");
                return;
            } // end if

            _appWindow.IsShownInSwitchers = false;

            if (_appWindow.Presenter is not OverlappedPresenter appWindowOverlappedPresenter)
            {
                Log.Warning("The main window's AppWindow's presenter is null.");
                return;
            } // end if

            appWindowOverlappedPresenter.IsAlwaysOnTop = true;
            appWindowOverlappedPresenter.IsMaximizable = false;
            appWindowOverlappedPresenter.IsMinimizable = false;
            appWindowOverlappedPresenter.IsResizable = false;
            appWindowOverlappedPresenter.SetBorderAndTitleBar(true, false);
        } // end method CustomiseWindow

        /// <summary>
        /// Set the root grid's background.
        /// </summary>
        private void SetRootGridBackground()
        {
            if (_existingWindow?.MicaC is null)
            {
                GridRoot.Background = new SolidColorBrush(((SolidColorBrush)GridRoot.Resources[
                        _existingWindow?.DesktopAcrylicC is not null
                            ? "RootGridAcrylicBackground"
                            : "RootGridFallbackBackground"])
                    .Color); // Use this format for the resources to make the brush transition work properly.
                return;
            } // end if

            GridRoot.Background = null;
        } // end method SetRootGridBackground

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            MenuFlyoutItemAppMenuMainWindowVisibility.Text = resourceLoader.GetString("MainWindowHide");
            MenuFlyoutItemMainMenuGiteeRepo.Text = resourceLoader.GetString("GiteeRepo");
            MenuFlyoutItemMainMenuGitHubRepo.Text = resourceLoader.GetString("GitHubRepo");
            MenuFlyoutItemMainMenuHelpHome.Text = $"{Package.Current.DisplayName} {resourceLoader.GetString("Site")}";
            MenuFlyoutItemMainMenuHelpLogsShow.Text = resourceLoader.GetString("LogsShow");
            MenuFlyoutItemMainMenuIssuesView.Text = resourceLoader.GetString("IssuesView");
            MenuFlyoutItemMainMenuMainWindowHide.Text = resourceLoader.GetString("MainWindowHide");
            MenuFlyoutItemMainMenuReleaseNotes.Text = resourceLoader.GetString("ReleaseNotes");
            MenuFlyoutItemMainMenuUserManual.Text = resourceLoader.GetString("UserManual");
            MenuFlyoutSubItemMainMenuHelp.Text = resourceLoader.GetString("Help");
            TaskbarIconApp.ToolTipText =
                $"{Package.Current.DisplayName} - {resourceLoader.GetString("TaskbarIconAppTooltip")}";
            ToolTipService.SetToolTip(ButtonMainMenu, resourceLoader.GetString("MainMenuButtonTooltip"));
            ToolTipService.SetToolTip(NavigationViewItemBodyAddUpdateAccount,
                resourceLoader.GetString("AccountAddUpdate"));
            ToolTipService.SetToolTip(NavigationViewItemBodyRealTimeNotes, resourceLoader.GetString("RealTimeNotes"));

            if ((bool)_propertySetSettings[SettingsHelper.KeyNotificationGreeting])
                new ToastContentBuilder()
                    .AddText(resourceLoader.GetString("NotificationGreetingTitle"))
                    .AddText(resourceLoader.GetString("NotificationGreetingContent"))
                    .Show(toast =>
                    {
                        toast.Group = Package.Current.DisplayName;
                        toast.Tag = AppConstantsHelper.TagNotificationGreeting;
                    });
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is AccountsHelper.PropertyNameIsAddingUpdating)
                NavigationViewItemBodyRealTimeNotes.IsEnabled = !_app.AccountsH.IsAddingUpdating;
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the body frame's size changed event.
        private void FrameBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var frameBodyContent = (FrameworkElement)FrameBody.Content;
            int winHeight;
            int winWidth;
            var workArea = WindowsHelper.GetWorkArea(WinId);

            // Avoid using "e.NewSize" to prevent window resizing delay.
            if (NavigationViewBody.PaneDisplayMode is NavigationViewPaneDisplayMode.Top)
            {
                winHeight = (int)(Math.Ceiling(frameBodyContent.ActualHeight) + NavigationViewBody.CompactPaneLength) +
                            WindowsHelper.MainWindowSideLengthOffset;
                winWidth = (int)Math.Ceiling(frameBodyContent.ActualWidth) + WindowsHelper.MainWindowSideLengthOffset;
            }
            else
            {
                winHeight = (int)Math.Ceiling(frameBodyContent.ActualHeight) + WindowsHelper.MainWindowSideLengthOffset;
                winWidth = (int)(Math.Ceiling(frameBodyContent.ActualWidth) + NavigationViewBody.CompactPaneLength) +
                           WindowsHelper.MainWindowSideLengthOffset;
            } // end if...else

            _appWindow.MoveAndResize(new RectInt32
            {
                Height = winHeight, Width = winWidth,
                X = workArea.Width - winWidth - WindowsHelper.MainWindowPositionOffset,
                Y = workArea.Height - winHeight - WindowsHelper.MainWindowPositionOffset
            });

            if (!_isFirstLoad) return;

            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
            _isFirstLoad = false;

            if (!(bool)_propertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts])
                _app.CommandsVm.ToggleMainWindowVisibilityCommand.Execute(null);
        } // end method FrameBody_OnSizeChanged

        // Handle the root grid's actual theme changed event.
        private void GridRoot_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetRootGridBackground();
        } // end method GridRoot_OnActualThemeChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            _existingWindow = _app.WindowsH.GetExistingMainWindow();
            SetRootGridBackground();
        } // end method GridRoot_OnLoaded

        // Handle the main window's closed event.
        private void MainWindow_OnClosed(object sender, WindowEventArgs args)
        {
            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
        } // end method MainWindow_OnClosed

        // Handle the body navigation view's selection changed event.
        private void NavigationViewBody_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            Type pageType;
            object parameter = null;

            if (NavigationViewBody.SelectedItem is not NavigationViewItem navigationViewBodySelectedItem) return;

            if (navigationViewBodySelectedItem == NavigationViewItemBodyAddUpdateAccount)
                pageType = typeof(AddUpdateAccountPage);
            else
            {
                pageType = typeof(RealTimeNotesPage);
                parameter = RealTimeNotesPageParameter;
            } // end if...else

            FrameBody.Navigate(pageType, parameter, new EntranceNavigationTransitionInfo());
        } // end method NavigationViewBody_OnSelectionChanged

        #endregion Event Handlers
    } // end class MainWindow
} // end namespace PaimonTray.Views