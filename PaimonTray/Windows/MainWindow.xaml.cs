using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PaimonTray.Helpers;
using System;
using Windows.ApplicationModel;
using Windows.Graphics;

namespace PaimonTray.Windows
{
    /// <summary>
    /// The main window to show the retrieved Genshin data.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Fields

        private bool _isFirstLoad = true; // A flag indicating if it is the first time the window is loaded.
        private AppWindow _appWindow;
        private WindowId _windowId;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            CustomiseWindow();

            ButtonHide.CommandParameter = _appWindow; // TODO
            TaskbarIconApp.LeftClickCommandParameter = _appWindow;
            TaskbarIconApp.ToolTipText = Package.Current.DisplayName;
        } // end constructor MainWindow

        #endregion Constructors

        #region Methods

        // Customise the window.
        private void CustomiseWindow()
        {
            _windowId = WindowManagementHelper.GetWindowId(this);
            _appWindow = WindowManagementHelper.GetAppWindow(_windowId);

            if (_appWindow == null) return; // TODO: need logging.

            _appWindow.IsShownInSwitchers = false;

            var appWindowOverlappedPresenter = _appWindow.Presenter as OverlappedPresenter;

            if (appWindowOverlappedPresenter == null) return; // TODO: need logging.

            appWindowOverlappedPresenter.IsAlwaysOnTop = true;
            appWindowOverlappedPresenter.IsMaximizable = false;
            appWindowOverlappedPresenter.IsMinimizable = false;
            appWindowOverlappedPresenter.IsResizable = false;
            appWindowOverlappedPresenter.SetBorderAndTitleBar(false, false);
        } // end method CustomiseWindow

        #endregion Methods

        #region Event Handlers

#pragma warning disable IDE0060 // Remove unused parameter
        // Handle the root stack panel's size changed event.
        private void StackPanelRoot_OnSizeChanged(object sender, SizeChangedEventArgs e)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var stackPanelRootHeight = (int)Math.Ceiling(e.NewSize.Height);
            var stackPanelRootWidth = (int)Math.Ceiling(e.NewSize.Width);
            var workArea = DisplayArea.GetFromWindowId(_windowId, DisplayAreaFallback.Primary).WorkArea;

            _appWindow.MoveAndResize(new RectInt32
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
} // end namespace PaimonTray.Windows