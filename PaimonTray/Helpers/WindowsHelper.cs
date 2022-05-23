using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PaimonTray.ViewModels;
using PaimonTray.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Graphics;
using WinRT.Interop;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The windows helper.
    /// </summary>
    public class WindowsHelper
    {
        #region Constants

        /// <summary>
        /// The main window's position offset.
        /// </summary>
        public const int MainWindowPositionOffset = 12;

        /// <summary>
        /// The main window's side length offset.
        /// </summary>
        public const int MainWindowSideLengthOffset = 2;

        #endregion Constants

        #region Properties

        /// <summary>
        /// A list of existing windows.
        /// </summary>
        public List<Window> ExistingWindowList { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the windows helper.
        /// </summary>
        public WindowsHelper()
        {
            ExistingWindowList = new List<Window>();
        } // end constructor WindowsHelper

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Get the app window for the given window ID.
        /// </summary>
        /// <param name="windowId">The window ID.</param>
        /// <returns>The app window.</returns>
        public static AppWindow GetAppWindow(WindowId windowId)
        {
            return AppWindow.GetFromWindowId(windowId);
        } // end method GetAppWindow

        /// <summary>
        /// Get the main window.
        /// NOTE: The main window will be opened if not exists. Use the specific command in <see cref="CommandsViewModel"/> to show/hide the window.
        /// </summary>
        /// <returns>The main window.</returns>
        public MainWindow GetMainWindow()
        {
            return ShowWindow(typeof(MainWindow), false) as MainWindow;
        } // end method GetMainWindow

        /// <summary>
        /// Get the window ID for the given window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns>The window ID.</returns>
        public static WindowId GetWindowId(Window window)
        {
            return Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(window));
        } // end method GetWindowId

        /// <summary>
        /// Get the display area's work area for the given window ID.
        /// </summary>
        /// <param name="windowId">The window ID.</param>
        /// <returns>The display area's work area.</returns>
        public static RectInt32 GetWorkArea(WindowId windowId)
        {
            return DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary).WorkArea;
        } // end method GetWorkArea

        /// <summary>
        /// Open/Activate the settings window.
        /// </summary>
        public void ShowSettingsWindow()
        {
            ShowWindow(typeof(SettingsWindow));
        } // end method ShowSettingsWindow

        /// <summary>
        /// Open/Activate the specific window.
        /// </summary>
        /// <param name="windowType">The window type.</param>
        /// <param name="activateIfExists">A flag indicating if the window should be activated if exists.</param>
        /// <returns>The window.</returns>
        private Window ShowWindow(Type windowType, bool activateIfExists = true)
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

            ExistingWindowList.Add(window);
            (Application.Current as App)?.SettingsH.ApplyThemeSelection();
            window.Closed += (_, _) => ExistingWindowList.Remove(window);

            return window;
        } // end method ShowWindow

        #endregion Methods
    } // end class WindowsHelper
} // end namespace PaimonTray.Helpers