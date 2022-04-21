using System;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PaimonTray.Views;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using WinRT.Interop;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The windows helper.
    /// </summary>
    internal class WindowsHelper
    {
        #region Properties

        /// <summary>
        /// A list of existing windows.
        /// </summary>
        public static List<Window> ExistingWindowList { get; } = new();

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get the <see cref="AppWindow"/> instance for the given <see cref="WindowId"/> instance.
        /// </summary>
        /// <param name="windowId">The <see cref="WindowId"/> instance.</param>
        /// <returns>The <see cref="AppWindow"/> instance.</returns>
        public static AppWindow GetAppWindow(WindowId windowId)
        {
            return AppWindow.GetFromWindowId(windowId);
        } // end method GetAppWindow

        /// <summary>
        /// Get the window ID for the given <see cref="Window"/> instance.
        /// </summary>
        /// <param name="window">The <see cref="Window"/> instance.</param>
        /// <returns>The <see cref="WindowId"/> instance.</returns>
        public static WindowId GetWindowId(Window window)
        {
            return Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(window));
        } // end method GetWindowId

        /// <summary>
        /// Open or activate the main window.
        /// </summary>
        /// <returns>The <see cref="MainWindow"/> instance.</returns>
        public static MainWindow ShowMainWindow()
        {
            return ShowWindow(typeof(MainWindow), false) as MainWindow;
        } // end method ShowMainWindow

        /// <summary>
        /// Open or activate the settings window.
        /// </summary>
        public static void ShowSettingsWindow()
        {
            ShowWindow(typeof(SettingsWindow));
        } // end method ShowSettingsWindow

        // Open or activate the specific window based on the provided window type. The flag is set to indicate if the window should be activated if exists.
        private static Window ShowWindow(Type windowType, bool activateIfExists = true)
        {
            foreach (var existingWindow in ExistingWindowList.Where(existingWindow =>
                         existingWindow.GetType() == windowType))
            {
                if (activateIfExists) existingWindow.Activate();

                return existingWindow;
            } // end foreach

            var window = Activator.CreateInstance(windowType) as Window;

            if (window == null)
            {
                Log.Warning($"Failed to open the specific window based on the provided window type ({windowType}).");
                return null;
            } // end if

            window.Closed += (_, _) => ExistingWindowList.Remove(window);
            ExistingWindowList.Add(window);

            return window;
        } // end method ShowWindow

        #endregion Methods
    } // end class WindowsHelper
} // end namespace PaimonTray.Helpers