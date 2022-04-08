using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace PaimonTray.Helpers
{
    public class MainWindowHelper
    {
        #region Fields

        private readonly AppWindow _mainAppWindow;

        #endregion Fields

        #region Constructors

        /// <summary>
        ///Initialise the main window helper.
        /// </summary>
        public MainWindowHelper()
        {
            _mainAppWindow =
                AppWindow.GetFromWindowId(
                    Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(App.MainWindow)));

            if (_mainAppWindow.IsShownInSwitchers)
                _mainAppWindow.IsShownInSwitchers = false;
        } // end constructor AppWindowHelper

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Hide the main window.
        /// </summary>
        public void Hide()
        {
            if (_mainAppWindow != null)
                _mainAppWindow.Hide();
        } // end method Hide

        /// <summary>
        /// Show the main window.
        /// </summary>
        public void Show()
        {
            if (_mainAppWindow != null)
                _mainAppWindow.Show();
        } // end method Show

        #endregion Public Methods
    } // end class MainWindowHelper
} // end namespace PaimonTray.Helpers