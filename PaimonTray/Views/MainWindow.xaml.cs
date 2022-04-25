using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
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

        private bool _isFirstLoad = true; // A flag indicating if it is the 1st time the window is loaded.
        private WindowId _windowId;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The main window's <see cref="AppWindow"/>.
        /// </summary>
        public AppWindow AppWin { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the main window. No need to activate it manually for the 1st time.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CustomiseWindow();
            UpdateUiText(true);

            MenuFlyoutItemMainMenuHelpShowLogs.CommandParameter = (Application.Current as App)?.LogsDirectory;
            TaskbarIconApp.Visibility = Visibility.Visible;
        } // end constructor MainWindow

        #endregion Constructors

        #region Methods

        // Customise the window.
        private void CustomiseWindow()
        {
            _windowId = WindowsHelper.GetWindowId(this);
            AppWin = WindowsHelper.GetAppWindow(_windowId);
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
            appWindowOverlappedPresenter.SetBorderAndTitleBar(false, false);
        } // end method CustomiseWindow

        // Update the UI text.
        private void UpdateUiText(bool isFirstLoad = false)
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
            NavigationViewItemBodyAddAccount.Content = resourceLoader.GetString("AddAccount");
            ToolTipService.SetToolTip(ButtonMainMenu, resourceLoader.GetString("MainMenuButtonTooltip"));

            if (isFirstLoad &&
                (bool)ApplicationData.Current.LocalSettings.Values[SettingsHelper.KeyGreetingNotification])
                new ToastContentBuilder()
                    .AddText(resourceLoader.GetString("GreetingNotificationTitle"))
                    .AddText(resourceLoader.GetString("GreetingNotificationContent"))
                    .Show(toast =>
                    {
                        toast.Group = Package.Current.DisplayName;
                        toast.Tag = AppConstantsHelper.NotificationTagGreeting;
                    });
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the root stack panel's size changed event.
        private void StackPanelRoot_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var stackPanelRootHeight = (int)Math.Ceiling(e.NewSize.Height);
            var stackPanelRootWidth = (int)Math.Ceiling(e.NewSize.Width);
            var workArea = DisplayArea.GetFromWindowId(_windowId, DisplayAreaFallback.Primary).WorkArea;

            AppWin.MoveAndResize(new RectInt32
            {
                Height = stackPanelRootHeight, Width = stackPanelRootWidth,
                X = workArea.Width - stackPanelRootWidth - AppConstantsHelper.MainWindowPositionOffset,
                Y = workArea.Height - stackPanelRootHeight - AppConstantsHelper.MainWindowPositionOffset
            });

            if (!_isFirstLoad) return;

            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
            _isFirstLoad = false;
        } // end method StackPanelRoot_OnSizeChanged

        #endregion Event Handlers
    } // end class MainWindow
} // end namespace PaimonTray.Views