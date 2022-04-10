using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The window management helper.
    /// </summary>
    internal class WindowManagementHelper
    {
        #region Methods

        /// <summary>
        /// Get the <see cref="AppWindow"/> instance for the given <see cref="WindowId"/> instance.
        /// </summary>
        /// <param name="windowId">The <see cref="WindowId"/> instance.</param>
        /// <returns>The <see cref="AppWindow"/> instance.</returns>
        public static AppWindow GetAppWindow(WindowId windowId)
        {
            return AppWindow.GetFromWindowId(windowId);
        } // end method GetAppWindow(WindowId)

        /// <summary>
        /// Get the <see cref="AppWindow"/> instance for the given <see cref="Window"/> instance.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> instance.</param>
        /// <returns>The <see cref="AppWindow"/> instance.</returns>
        public static AppWindow GetAppWindow(Window window)
        {
            return GetAppWindow(GetWindowId(window));
        } // end method GetAppWindow(Window)

        /// <summary>
        /// Get the window ID for the given <see cref="Window"/> instance.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> instance.</param>
        /// <returns>The <see cref="WindowId"/> instance.</returns>
        public static WindowId GetWindowId(Window window)
        {
            return Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(window));
        } // end method GetWindowId

        #endregion Methods
    } // end class WindowManagementHelper
} // end namespace PaimonTray.Helpers