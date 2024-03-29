﻿using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
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
using Windows.Graphics;

namespace PaimonTray.Views
{
    /// <summary>
    /// The main window.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Constructors

        /// <summary>
        /// Initialise the main window. No need to activate it manually for the 1st time.
        /// </summary>
        public MainWindow()
        {
            _app = Application.Current as App;
            _isFirstLoad = true;
            MainWinViewModel = new MainWindowViewModel();
            InitializeComponent();
            CustomiseWindow();
            UpdateUiText();

            MenuFlyoutItemMainMenuHelpLogsShow.CommandParameter = _app?.LogsDirectory;
            MenuFlyoutItemMainMenuHelpReleaseNotes.CommandParameter = _app?.UrlGitHubRepoRelease;
            TaskbarIconApp.Visibility = Visibility.Visible; // Show the taskbar icon when ready.
        } // end constructor MainWindow

        #endregion Constructors

        #region Event Handlers

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is AccountsHelper.PropertyNameIsAddingUpdating)
                NavigationViewItemBodyRealTimeNotes.IsEnabled = !_app.AccountsH.IsAddingUpdating;
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the dispatcher queue timer's tick event.
        private void DispatcherQueueTimer_OnTick(object sender, object e)
        {
            var workArea = WindowsHelper.GetWorkArea(WinId);

            if (workArea.Height == _workAreaHeightLast && workArea.Width == _workAreaWidthLast) return;

            FrameBody_OnSizeChanged(FrameBody, null);
        } // end method DispatcherQueueTimer_OnTick

        // Handle the body frame's size changed event.
        private void FrameBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var frameBodyContent = (FrameworkElement)FrameBody.Content;
            var mainWindowPositionOffset =
                (int)Math.Ceiling(WindowsHelper.MainWindowPositionOffset * GridRoot.XamlRoot.RasterizationScale);
            double winHeightExpected;
            double winWidthExpected;
            var workArea = WindowsHelper.GetWorkArea(WinId);

            // Avoid using "e.NewSize" to prevent window resizing delay.
            if (NavigationViewBody.PaneDisplayMode is NavigationViewPaneDisplayMode.Top)
            {
                winHeightExpected = frameBodyContent.ActualHeight + NavigationViewBody.CompactPaneLength +
                                    WindowsHelper.MainWindowSideLengthOffset;
                winWidthExpected = frameBodyContent.ActualWidth + WindowsHelper.MainWindowSideLengthOffset;
            }
            else
            {
                winHeightExpected = frameBodyContent.ActualHeight + WindowsHelper.MainWindowSideLengthOffset;
                winWidthExpected = frameBodyContent.ActualWidth + NavigationViewBody.CompactPaneLength +
                                   WindowsHelper.MainWindowSideLengthOffset;
            } // end if...else

            var winHeight = (int)Math.Ceiling(winHeightExpected * GridRoot.XamlRoot.RasterizationScale);
            var winWidth = (int)Math.Ceiling(winWidthExpected * GridRoot.XamlRoot.RasterizationScale);

            _appWindow.MoveAndResize(new RectInt32
            {
                Height = winHeight, Width = winWidth,
                X = workArea.Width - winWidth - mainWindowPositionOffset,
                Y = workArea.Height - winHeight - mainWindowPositionOffset
            });
            _workAreaHeightLast = workArea.Height;
            _workAreaWidthLast = workArea.Width;

            if (!_isFirstLoad) return;

            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
            _isFirstLoad = false;

            if (_app.SettingsH.PropertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts] is false)
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
            _dispatcherQueueTimer = DispatcherQueue.CreateTimer();
            _dispatcherQueueTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherQueueTimer.Tick += DispatcherQueueTimer_OnTick;
            _existingWindow = _app.WindowsH.GetExistingMainWindow();
            SetRootGridBackground();
            _dispatcherQueueTimer.Start(); // Start the dispatcher queue timer when ready.
        } // end method GridRoot_OnLoaded

        // Handle the main window's closed event.
        private void MainWindow_OnClosed(object sender, WindowEventArgs args)
        {
            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            _app = null;
            _appWindow = null;
            _dispatcherQueueTimer.Stop();
            _dispatcherQueueTimer.Tick -= DispatcherQueueTimer_OnTick;
            _existingWindow = null;
        } // end method MainWindow_OnClosed

        // Handle the body navigation view's selection changed event.
        private void NavigationViewBody_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            if (NavigationViewBody.SelectedItem is not NavigationViewItem navigationViewBodySelectedItem) return;

            NavigationViewBody.IsPaneOpen =
                false; // A workaround to make sure the navigation view items are visible to the user.
            FrameBody.Navigate(
                navigationViewBodySelectedItem == NavigationViewItemBodyAccountAddUpdate
                    ? typeof(AddUpdateAccountPage)
                    : typeof(RealTimeNotesPage), null, new EntranceNavigationTransitionInfo());
        } // end method NavigationViewBody_OnSelectionChanged

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The app window.
        /// </summary>
        private AppWindow _appWindow;

        /// <summary>
        /// The dispatcher queue timer.
        /// </summary>
        private DispatcherQueueTimer _dispatcherQueueTimer;

        /// <summary>
        /// The existing window.
        /// </summary>
        private ExistingWindow _existingWindow;

        /// <summary>
        /// The work area's last height.
        /// </summary>
        private int _workAreaHeightLast;

        /// <summary>
        /// The work area's last width.
        /// </summary>
        private int _workAreaWidthLast;

        /// <summary>
        /// A flag indicating if it is the 1st time the window is loaded.
        /// </summary>
        private bool _isFirstLoad;

        #endregion Fields

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
                Log.Warning("The main window's app window is null.");
                return;
            } // end if

            _appWindow.IsShownInSwitchers = false;

            if (_appWindow.Presenter is not OverlappedPresenter appWindowOverlappedPresenter)
            {
                appWindowOverlappedPresenter = OverlappedPresenter.Create();
                _appWindow.SetPresenter(appWindowOverlappedPresenter);
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
                GridRoot.Resources.TryGetValue(
                    _existingWindow?.DesktopAcrylicC is not null
                        ? "RootGridAcrylicBackground"
                        : "RootGridFallbackBackground", out var gridRootBackground);

                if (gridRootBackground is SolidColorBrush solidColourBrushGridRootBackground)
                {
                    GridRoot.Background =
                        new SolidColorBrush(solidColourBrushGridRootBackground
                            .Color); // Code in this way to make the brush transition work properly.
                    return;
                } // end if
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
            MenuFlyoutItemMainMenuHelpAppSite.Text =
                $"{Package.Current.DisplayName} {resourceLoader.GetString("Site")}";
            MenuFlyoutItemMainMenuHelpGiteeRepo.Text = resourceLoader.GetString("GiteeRepo");
            MenuFlyoutItemMainMenuHelpGitHubRepo.Text = resourceLoader.GetString("GitHubRepo");
            MenuFlyoutItemMainMenuHelpIssues.Text = resourceLoader.GetString("Issues");
            MenuFlyoutItemMainMenuHelpLogsShow.Text = resourceLoader.GetString("LogsShow");
            MenuFlyoutItemMainMenuHelpReleaseNotes.Text = resourceLoader.GetString("ReleaseNotes");
            MenuFlyoutItemMainMenuHelpUserManual.Text = resourceLoader.GetString("UserManual");
            MenuFlyoutItemMainMenuMainWindowHide.Text = resourceLoader.GetString("MainWindowHide");
            MenuFlyoutSubItemMainMenuHelp.Text = resourceLoader.GetString("Help");
            TaskbarIconApp.ToolTipText = Package.Current.DisplayName;
            ToolTipService.SetToolTip(ButtonMainMenu, resourceLoader.GetString("MainMenuButtonTooltip"));
            ToolTipService.SetToolTip(NavigationViewItemBodyAccountAddUpdate,
                resourceLoader.GetString("AccountAddUpdate"));
            ToolTipService.SetToolTip(NavigationViewItemBodyRealTimeNotes, resourceLoader.GetString("RealTimeNotes"));

            if (_app.SettingsH.PropertySetSettings[SettingsHelper.KeyNotificationGreeting] is true)
                new ToastContentBuilder()
                    .AddText(resourceLoader.GetString("NotificationGreetingTitle"))
                    .AddText(resourceLoader.GetString("NotificationGreetingContent"))
                    .Show(toast =>
                    {
                        toast.Group = Package.Current.DisplayName;
                        toast.Tag = AppFieldsHelper.TagNotificationGreeting;
                    });
        } // end method UpdateUiText

        #endregion Methods

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
    } // end class MainWindow
} // end namespace PaimonTray.Views