﻿using H.NotifyIcon;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The taskbar icon view model.
    /// </summary>
    internal class TaskbarIconViewModel
    {
        /// <summary>
        /// The command to exit the app.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static
        public ICommand ExitAppCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand command = new();

                command.ExecuteRequested += (_, e) =>
                {
                    if (e.Parameter is TaskbarIcon taskBarIconApp)
                        taskBarIconApp.Dispose(); // Ensure the tray icon is removed.

                    Application.Current.Exit();
                };
                return command;
            } // end get
        } // end property ExitAppCommand

        /// <summary>
        /// The command to show the main window.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static
        public ICommand ToggleMainWindowVisibilityCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand command = new();

                command.ExecuteRequested += (_, e) =>
                {
                    if (e.Parameter is not AppWindow mainAppWindow) return;

                    if (mainAppWindow.IsVisible)
                        mainAppWindow.Hide();
                    else
                        mainAppWindow.Show();
                };
                return command;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand
    } // end class TaskbarIconViewModel
} // end namespace PaimonTray.ViewModels