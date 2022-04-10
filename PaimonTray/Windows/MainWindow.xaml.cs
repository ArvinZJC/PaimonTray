using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.ApplicationModel;
using Windows.Graphics;
using WinRT.Interop;

namespace PaimonTray.Windows
{
    /// <summary>
    /// The main window to show the retrieved Genshin data.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Fields

        private int _stackPanelRootHeight;
        private int _stackPanelRootWidth;

        private readonly AppWindow _appWindow;
        private readonly WindowId _windowId;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(this));
            _appWindow = AppWindow.GetFromWindowId(_windowId); // Get AppWindow from Window.

            if (_appWindow == null) return;

            _appWindow.IsShownInSwitchers = false;

            var appWindowOverlappedPresenter = _appWindow.Presenter as OverlappedPresenter;

            if (appWindowOverlappedPresenter != null)
            {
                appWindowOverlappedPresenter.IsAlwaysOnTop = true;
                appWindowOverlappedPresenter.IsMaximizable = false;
                appWindowOverlappedPresenter.IsMinimizable = false;
                appWindowOverlappedPresenter.IsResizable = false;
                appWindowOverlappedPresenter.SetBorderAndTitleBar(false, false);
            } // end if

            TaskbarIconApp.DoubleClickCommandParameter = _appWindow;
            TaskbarIconApp.LeftClickCommandParameter = _appWindow;
            TaskbarIconApp.ToolTipText = Package.Current.DisplayName;
        } // end constructor MainWindow

        #endregion Constructors

        #region Event Handlers

#pragma warning disable IDE0060 // Remove unused parameter
        private void StackPanelRoot_OnSizeChanged(object sender, SizeChangedEventArgs e)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (_stackPanelRootHeight > 0 && _stackPanelRootWidth > 0) return;

            var workArea = DisplayArea.GetFromWindowId(_windowId, DisplayAreaFallback.Primary).WorkArea;

            _stackPanelRootHeight = (int)Math.Ceiling(e.NewSize.Height);
            _stackPanelRootWidth = (int)Math.Ceiling(e.NewSize.Width);
            _appWindow.MoveAndResize(new RectInt32
            {
                Height = _stackPanelRootHeight, Width = _stackPanelRootWidth,
                X = (workArea.Width - _stackPanelRootWidth - 12), Y = (workArea.Height - _stackPanelRootHeight - 12)
            });
            Activate(); // Activate the window here to prevent being flicked when moving and resizing.
        } // end method StackPanelRoot_OnSizeChanged

        #endregion Event Handlers
    } // end class MainWindow
} // end namespace PaimonTray.Windows