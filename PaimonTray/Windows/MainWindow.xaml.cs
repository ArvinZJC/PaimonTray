using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace PaimonTray.Windows
{
    /// <summary>
    /// The main window to show the retrieved Genshin data.
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Constructors

        /// <summary>
        /// Initialise the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var mainAppWindow =
                AppWindow.GetFromWindowId(
                    Win32Interop.GetWindowIdFromWindow(
                        WindowNative.GetWindowHandle(this))); // Get AppWindow from Window.

            if (mainAppWindow == null) return;

            mainAppWindow.IsShownInSwitchers = false;

            var mainAppWindowOverlappedPresenter = mainAppWindow.Presenter as OverlappedPresenter;

            if (mainAppWindowOverlappedPresenter != null)
            {
                mainAppWindowOverlappedPresenter.IsAlwaysOnTop = true;
                mainAppWindowOverlappedPresenter.IsMaximizable = false;
                mainAppWindowOverlappedPresenter.IsMinimizable = false;
                mainAppWindowOverlappedPresenter.IsResizable = false;
            } // end if

            TaskbarIconApp.DoubleClickCommandParameter = mainAppWindow;
            TaskbarIconApp.LeftClickCommandParameter = mainAppWindow;
        } // end constructor MainWindow

        #endregion Constructors
    } // end class MainWindow
} // end namespace PaimonTray.Windows