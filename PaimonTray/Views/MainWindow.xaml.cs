using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
using Serilog;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Graphics;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The main window to show the retrieved Genshin data.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Fields

        /// <summary>
        /// A flag indicating if it is the 1st time the window is loaded.
        /// </summary>
        private bool _isFirstLoad = true;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The main window's <see cref="AppWindow"/>.
        /// </summary>
        public AppWindow AppWin { get; private set; }

        /// <summary>
        /// The main window's <see cref="MainWindowViewModel"/>.
        /// </summary>
        public MainWindowViewModel MainWinViewModel { get; }

        /// <summary>
        /// The main window's <see cref="WindowId"/>.
        /// </summary>
        public WindowId WinId { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the main window. No need to activate it manually for the 1st time.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CustomiseWindow();
            UpdateUiText();

            MainWinViewModel = new MainWindowViewModel();
            MenuFlyoutItemMainMenuHelpShowLogs.CommandParameter = (Application.Current as App)?.LogsDirectory;
            TaskbarIconApp.Visibility = Visibility.Visible;
        } // end constructor MainWindow

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Customise the window.
        /// </summary>
        private void CustomiseWindow()
        {
            WinId = WindowsHelper.GetWindowId(this);
            AppWin = WindowsHelper.GetAppWindow(WinId);
            Title = Package.Current.DisplayName;

            if (AppWin == null)
            {
                Log.Warning("The main window's AppWindow is null.");
                return;
            } // end if

            AppWin.IsShownInSwitchers = false;

            var appWindowOverlappedPresenter = AppWin.Presenter as OverlappedPresenter;

            if (appWindowOverlappedPresenter == null)
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
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            MenuFlyoutItemAppMenuExit.Text = resourceLoader.GetString("Exit");
            MenuFlyoutItemAppMenuSettings.Text = resourceLoader.GetString("Settings");
            MenuFlyoutItemMainMenuExit.Text = resourceLoader.GetString("Exit");
            MenuFlyoutItemMainMenuGiteeRepo.Text = resourceLoader.GetString("GiteeRepo");
            MenuFlyoutItemMainMenuGitHubRepo.Text = resourceLoader.GetString("GitHubRepo");
            MenuFlyoutItemMainMenuHelpHome.Text = $"{Package.Current.DisplayName} {resourceLoader.GetString("Site")}";
            MenuFlyoutItemMainMenuHelpShowLogs.Text = resourceLoader.GetString("ShowLogs");
            MenuFlyoutItemMainMenuHide.Text = resourceLoader.GetString("Hide");
            MenuFlyoutItemMainMenuAddAccount.Text = resourceLoader.GetString("AddAccount");
            MenuFlyoutItemMainMenuReleaseNotes.Text = resourceLoader.GetString("ReleaseNotes");
            MenuFlyoutItemMainMenuSettings.Text = resourceLoader.GetString("Settings");
            MenuFlyoutItemMainMenuUserManual.Text = resourceLoader.GetString("UserManual");
            MenuFlyoutItemMainMenuViewIssues.Text = resourceLoader.GetString("ViewIssues");
            MenuFlyoutSubItemMainMenuHelp.Text = resourceLoader.GetString("Help");
            ToolTipService.SetToolTip(ButtonMainMenu, resourceLoader.GetString("MainMenuButtonTooltip"));
            ToolTipService.SetToolTip(NavigationViewItemBodyAddAccount, resourceLoader.GetString("AddAccount"));

            if ((bool)ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyGreetingNotification])
                new ToastContentBuilder()
                    .AddText(resourceLoader.GetString("GreetingNotificationTitle"))
                    .AddText(resourceLoader.GetString("GreetingNotificationContent"))
                    .Show(toast =>
                    {
                        toast.Group = Package.Current.DisplayName;
                        toast.Tag = AppConstantsHelper.TagGreetingNotification;
                    });
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the body frame's size changed event.
        private void FrameBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            int winHeight;
            int winWidth;
            var workArea = DisplayArea.GetFromWindowId(WinId, DisplayAreaFallback.Primary).WorkArea;

            // Avoid using "e.NewSize" to prevent window resizing delay.
            if (NavigationViewBody.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                winHeight = (int)(Math.Ceiling(((FrameworkElement)FrameBody.Content).ActualHeight) +
                                  NavigationViewBody.CompactPaneLength) +
                            AppConstantsHelper.WindowMainSideLengthOffset;
                winWidth = (int)Math.Ceiling(((FrameworkElement)FrameBody.Content).ActualWidth) +
                           AppConstantsHelper.WindowMainSideLengthOffset;
            }
            else
            {
                winHeight = (int)Math.Ceiling(((FrameworkElement)FrameBody.Content).ActualHeight) +
                            AppConstantsHelper.WindowMainSideLengthOffset;
                winWidth = (int)(Math.Ceiling(((FrameworkElement)FrameBody.Content).ActualWidth) +
                                 NavigationViewBody.CompactPaneLength) +
                           AppConstantsHelper.WindowMainSideLengthOffset;
            } // end if...else

            AppWin.MoveAndResize(new RectInt32
            {
                Height = winHeight, Width = winWidth,
                X = workArea.Width - winWidth - AppConstantsHelper.WindowMainPositionOffset,
                Y = workArea.Height - winHeight - AppConstantsHelper.WindowMainPositionOffset
            });

            if (!_isFirstLoad) return;

            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
            _isFirstLoad = false;
        } // end method FrameBody_OnSizeChanged

        // Handle the body navigation view's selection changed event.
        private void NavigationViewBody_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            FrameBody.Navigate(
                args.SelectedItem as NavigationViewItem == NavigationViewItemBodyAddAccount
                    ? typeof(AddAccountPage)
                    : typeof(DataPage), null, new EntranceNavigationTransitionInfo());
        } // end method NavigationViewBody_OnSelectionChanged

        #endregion Event Handlers
    } // end class MainWindow
} // end namespace PaimonTray.Views