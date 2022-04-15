using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PaimonTray.Helpers;
using Serilog;
using System;
using Windows.ApplicationModel;
using Windows.Graphics;

namespace PaimonTray.Views
{
    /// <summary>
    /// The main window to show the retrieved Genshin data.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Fields

        private bool _isFirstLoad = true; // A flag indicating if it is the first time the window is loaded.
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
        /// Initialise the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CustomiseWindow();

            MenuFlyoutItemMoreHelpHome.Text = $"{Package.Current.DisplayName} site"; // TODO
            MenuFlyoutItemMoreHide.CommandParameter = AppWin; // TODO
            TaskbarIconApp.LeftClickCommandParameter = AppWin;
            TaskbarIconApp.ToolTipText = Package.Current.DisplayName;
            TaskbarIconApp.Visibility = Visibility.Visible;

            new ToastContentBuilder()
                .AddText("Paimon's now in your \"system tray\"!")
                .AddText(
                    "If the icon doesn't appear in the taskbar corner, you can find it in the taskbar corner overflow menu and move it to the taskbar corner by system settings or by dragging and dropping.")
                .Show(toast =>
                {
                    toast.Group = Package.Current.DisplayName;
                    toast.Tag = "TaskbarIconApp_Ready"; // TODO: make tag a constant.
                });
        } // end constructor MainWindow

        #endregion Constructors

        #region Methods

        // Customise the window.
        private void CustomiseWindow()
        {
            _windowId = WindowManagementHelper.GetWindowId(this);
            AppWin = WindowManagementHelper.GetAppWindow(_windowId);
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
                X = (workArea.Width - stackPanelRootWidth - 12),
                Y = (workArea.Height - stackPanelRootHeight - 12) // TODO: make 12 a constant.
            });

            if (!_isFirstLoad) return;

            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
            _isFirstLoad = false;
        } // end method StackPanelRoot_OnSizeChanged

        #endregion Event Handlers
    } // end class MainWindow
} // end namespace PaimonTray.Views