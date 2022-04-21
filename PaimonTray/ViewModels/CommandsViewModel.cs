using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using PaimonTray.Helpers;
using Serilog;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Notifications;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The taskbar icon view model.
    /// </summary>
    internal class CommandsViewModel
    {
        #region Properties

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to exit the app.
        /// </summary>
        public ICommand ExitAppCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, e) =>
                {
                    Log.Information("Exit the app requested.");

                    if (e.Parameter is TaskbarIcon taskBarIconApp)
                        taskBarIconApp.Dispose(); // Ensure the tray icon is removed.

                    ToastNotificationManager.History.Remove(AppConstantsHelper.NotificationTagTaskbarIconAppReady,
                        Package.Current.DisplayName);
                    Log.CloseAndFlush();
                    Application.Current.Exit();
                };
                return xamlUiCommand;
            } // end get
        } // end property ExitAppCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to open a specific link in the default program.
        /// </summary>
        public ICommand OpenLinkInDefaultCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, e) =>
                {
                    if (e.Parameter is string link)
                        Process.Start(new ProcessStartInfo { FileName = link, UseShellExecute = true });
                };
                return xamlUiCommand;
            } // end get
        } // end property OpenLinkInDefaultCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to open or activate the settings window.
        /// </summary>
        public ICommand ShowSettingsWindowCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, _) => WindowsHelper.ShowSettingsWindow();
                return xamlUiCommand;
            } // end get
        } // end property ShowSettingsWindowCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to show or hide the main window.
        /// </summary>
        public ICommand ToggleMainWindowVisibilityCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    var mainWindow = WindowsHelper.ShowMainWindow();

                    if (mainWindow.AppWin.IsVisible)
                        mainWindow.AppWin.Hide();
                    else
                        mainWindow.AppWin.Show();
                };
                return xamlUiCommand;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand

        #endregion Properties
    } // end class CommandsViewModel
} // end namespace PaimonTray.ViewModels