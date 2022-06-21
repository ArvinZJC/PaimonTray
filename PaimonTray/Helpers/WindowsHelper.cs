using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Models;
using PaimonTray.ViewModels;
using PaimonTray.Views;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Graphics;
using WinRT;
using WinRT.Interop;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The windows helper.
    /// </summary>
    public class WindowsHelper
    {
        #region Constructors

        /// <summary>
        /// Initialise the windows helper.
        /// </summary>
        public WindowsHelper()
        {
            _app = Application.Current as App;
            ExistingWindows = new List<ExistingWindow>();
        } // end constructor WindowsHelper

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~WindowsHelper()
        {
            _app = null;
        } // end destructor

        #endregion Destructor

        #region Event Handlers

        // Handle the window's activated event.
        private void Window_OnActivated(object sender, WindowActivatedEventArgs args)
        {
            var existingWindowTarget =
                ExistingWindows.FirstOrDefault(existingWindow => existingWindow.Win == sender as Window, null);

            if (existingWindowTarget is null)
            {
                Log.Warning($"Failed to find the existing window (sender type: {sender?.GetType()}).");
                return;
            } // end if

            var systemBackdropConfiguration = existingWindowTarget.SystemBackdropConfig;

            if (systemBackdropConfiguration is null)
            {
                Log.Warning($"No system backdrop configuration (window type: {existingWindowTarget.Win.GetType()}).");
                return;
            } // end if

            systemBackdropConfiguration.IsInputActive =
                args.WindowActivationState is not WindowActivationState.Deactivated;
        } // end method Window_OnActivated

        // Handle the window's closed event.
        private void Window_OnClosed(object sender, WindowEventArgs args)
        {
            var existingWindowTarget =
                ExistingWindows.FirstOrDefault(existingWindow => existingWindow.Win == sender as Window, null);

            if (existingWindowTarget is null || !ExistingWindows.Remove(existingWindowTarget))
            {
                Log.Warning($"Failed to remove the existing window (sender type: {sender?.GetType()}).");
                return;
            } // end if

            if (existingWindowTarget.DesktopAcrylicC is not null)
            {
                existingWindowTarget.DesktopAcrylicC.Dispose();
                existingWindowTarget.DesktopAcrylicC = null;
            } // end if

            if (existingWindowTarget.MicaC is not null)
            {
                existingWindowTarget.MicaC.Dispose();
                existingWindowTarget.MicaC = null;
            } // end if

            if (existingWindowTarget.SystemBackdropConfig is not null)
            {
                existingWindowTarget.Win.Activated -= Window_OnActivated;
                existingWindowTarget.SystemBackdropConfig = null;
            } // end if

            existingWindowTarget.Win.Closed -= Window_OnClosed;
            existingWindowTarget.Win = null;
        } // end method Window_OnClosed

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The main window's position offset.
        /// </summary>
        public static readonly int MainWindowPositionOffset = 12;

        /// <summary>
        /// The main window's side length offset.
        /// </summary>
        public static readonly int MainWindowSideLengthOffset = 2;

        #endregion Fields

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
        /// Get the existing main window.
        /// NOTE: The main window will be opened if not exists. Use the specific command in <see cref="CommandsViewModel"/> to show/hide the window.
        /// </summary>
        /// <returns>The existing main window, or <c>null</c> if the operation fails.</returns>
        public ExistingWindow GetExistingMainWindow()
        {
            return ShowWindow(typeof(MainWindow), false);
        } // end method GetExistingMainWindow

        /// <summary>
        /// Get the existing settings window.
        /// NOTE: The settings window will be opened if not exists. Otherwise, it will be activated.
        /// </summary>
        /// <returns>The existing settings window, or <c>null</c> if the operation fails.</returns>
        public ExistingWindow GetExistingSettingsWindow()
        {
            return ShowWindow(typeof(SettingsWindow));
        } // end method GetExistingSettingsWindow

        /// <summary>
        /// Get the main window's page's max size.
        /// </summary>
        /// <returns>The max size.</returns>
        public SizeInt32 GetMainWindowPageMaxSize()
        {
            var isMainWindowTopNavigationPane =
                _app.SettingsH.DecideMainWindowNavigationViewPaneDisplayMode() is NavigationViewPaneDisplayMode.Top;
            var mainExistingWindow = GetExistingMainWindow();
            var workArea =
                GetWorkArea(mainExistingWindow is null ? new WindowId() : GetWindowId(mainExistingWindow.Win));
            var workAreaOffset = 2 * MainWindowPositionOffset;
            var workAreaAdditionalOffset = 4 * MainWindowPositionOffset; // Reserved for the navigation pane.

            return new SizeInt32(
                workArea.Width - workAreaOffset - (isMainWindowTopNavigationPane ? 0 : workAreaAdditionalOffset),
                workArea.Height - workAreaOffset - (isMainWindowTopNavigationPane ? workAreaAdditionalOffset : 0));
        } // end method GetMainWindowPageMaxSize

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
        /// Open/Activate the specific window.
        /// </summary>
        /// <param name="windowType">The window type.</param>
        /// <param name="activateIfExists">A flag indicating if the window should be activated if exists.</param>
        /// <returns>The existing window, or <c>null</c> if the operation fails.</returns>
        private ExistingWindow ShowWindow(Type windowType, bool activateIfExists = true)
        {
            if (windowType is null)
            {
                Log.Warning("Null window type.");
                return null;
            } // end if

            var existingWindowTarget =
                ExistingWindows.FirstOrDefault(existingWindow => existingWindow.Win.GetType() == windowType, null);

            if (existingWindowTarget is null)
            {
                if (Activator.CreateInstance(windowType) is not Window window)
                {
                    Log.Warning(
                        $"Failed to open the specific window based on the provided window type ({windowType}).");
                    return null;
                } // end if

                var existingWindow = new ExistingWindow { Win = window };

                ExistingWindows.Add(existingWindow); // Should add the existing window to the list as soon as possible.

                if (new DispatcherQueueControllerHelper().EnsureDispatcherQueueController())
                {
                    if (MicaController.IsSupported())
                    {
                        existingWindow.SystemBackdropConfig = new SystemBackdropConfiguration { IsInputActive = true };
                        window.Activated += Window_OnActivated;
                        existingWindow.MicaC = new MicaController();
                        existingWindow.MicaC.AddSystemBackdropTarget(window.As<ICompositionSupportsSystemBackdrop>());
                        existingWindow.MicaC.SetSystemBackdropConfiguration(existingWindow.SystemBackdropConfig);
                    }
                    else
                    {
                        Log.Information("Mica is unsupported.");

                        if (DesktopAcrylicController.IsSupported())
                        {
                            existingWindow.SystemBackdropConfig = new SystemBackdropConfiguration
                                { IsInputActive = true };
                            window.Activated += Window_OnActivated;
                            existingWindow.DesktopAcrylicC = new DesktopAcrylicController();
                            existingWindow.DesktopAcrylicC.AddSystemBackdropTarget(
                                window.As<ICompositionSupportsSystemBackdrop>());
                            existingWindow.DesktopAcrylicC.SetSystemBackdropConfiguration(existingWindow
                                .SystemBackdropConfig);
                        }
                        else Log.Information("Background Acrylic is unsupported.");
                    } // end if...else
                } // end if

                _app.SettingsH.ApplyThemeSelection();
                window.Closed += Window_OnClosed;
                return existingWindow;
            } // end if

            if (activateIfExists) existingWindowTarget.Win.Activate();

            return existingWindowTarget;
        } // end method ShowWindow

        #endregion Methods

        #region Properties

        /// <summary>
        /// A list of existing windows.
        /// </summary>
        public List<ExistingWindow> ExistingWindows { get; }

        #endregion Properties
    } // end class WindowsHelper
} // end namespace PaimonTray.Helpers