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
using Windows.Foundation.Collections;
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

        private AppWindow _appWindow;

        /// <summary>
        /// A flag indicating if it is the 1st time the window is loaded.
        /// </summary>
        private bool _isFirstLoad = true;

        private readonly App _app;
        private readonly IPropertySet _propertySetSettings;

        #endregion Fields

        #region Properties

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
            _app = Application.Current as App;
            _propertySetSettings = ApplicationData.Current.LocalSettings.Containers[SettingsHelper.ContainerKeySettings]
                .Values;
            MainWinViewModel = new MainWindowViewModel();
            InitializeComponent();
            CustomiseWindow();
            UpdateUiText();

            MenuFlyoutItemMainMenuHelpLogsShow.CommandParameter = _app?.LogsDirectory;
            TaskbarIconApp.Visibility = Visibility.Visible; // Show the taskbar icon when ready.
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

            if (_appWindow == null)
            {
                Log.Warning("The main window's AppWindow is null.");
                return;
            } // end if

            _appWindow.IsShownInSwitchers = false;

            var appWindowOverlappedPresenter = _appWindow.Presenter as OverlappedPresenter;

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
            ToolTipService.SetToolTip(NavigationViewItemBodyAddAccount, resourceLoader.GetString("AddAccount"));

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

        // Handle the body frame's size changed event.
        private void FrameBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var frameBodyContent = (FrameworkElement)FrameBody.Content;
            int winHeight;
            int winWidth;
            var workArea = DisplayArea.GetFromWindowId(WinId, DisplayAreaFallback.Primary).WorkArea;

            // Avoid using "e.NewSize" to prevent window resizing delay.
            if (NavigationViewBody.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                winHeight = (int)(Math.Ceiling(frameBodyContent.ActualHeight) + NavigationViewBody.CompactPaneLength) +
                            AppConstantsHelper.MainWindowSideLengthOffset;
                winWidth = (int)Math.Ceiling(frameBodyContent.ActualWidth) +
                           AppConstantsHelper.MainWindowSideLengthOffset;
            }
            else
            {
                winHeight = (int)Math.Ceiling(frameBodyContent.ActualHeight) +
                            AppConstantsHelper.MainWindowSideLengthOffset;
                winWidth = (int)(Math.Ceiling(frameBodyContent.ActualWidth) + NavigationViewBody.CompactPaneLength) +
                           AppConstantsHelper.MainWindowSideLengthOffset;
            } // end if...else

            _appWindow.MoveAndResize(new RectInt32
            {
                Height = winHeight, Width = winWidth,
                X = workArea.Width - winWidth - AppConstantsHelper.MainWindowPositionOffset,
                Y = workArea.Height - winHeight - AppConstantsHelper.MainWindowPositionOffset
            });

            if (!_isFirstLoad) return;

            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
            _isFirstLoad = false;

            if (!(bool)_propertySetSettings[SettingsHelper.KeyMainWindowShowWhenAppStarts])
                new CommandsViewModel().ToggleMainWindowVisibilityCommand.Execute(null);
        } // end method FrameBody_OnSizeChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            var shouldSelectFirst = true;

            foreach (var containerKeyAccount in ApplicationData.Current.LocalSettings
                         .Containers[AccountsHelper.ContainerKeyAccounts].Containers.Keys)
                if (_app.AccHelper.AddOrUpdateCharactersNavigation(containerKeyAccount, null, shouldSelectFirst))
                    shouldSelectFirst = false;
        } // end method GridRoot_OnLoaded

        // Handle the body navigation view's selection changed event.
        private void NavigationViewBody_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            Type pageType;
            object parameter = null;
            var selectedItem = args.SelectedItem as NavigationViewItem;

            if (selectedItem == null) return;

            if (selectedItem == NavigationViewItemBodyAddAccount)
                pageType = typeof(AddAccountPage);
            else
            {
                pageType = typeof(RealTimeNotesPage);
                parameter = selectedItem.Tag;
            } // end if...else

            FrameBody.Navigate(pageType, parameter, new EntranceNavigationTransitionInfo());
        } // end method NavigationViewBody_OnSelectionChanged

        #endregion Event Handlers
    } // end class MainWindow
} // end namespace PaimonTray.Views