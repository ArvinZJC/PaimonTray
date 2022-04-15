using H.NotifyIcon;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using PaimonTray.Views;
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

                    // Hide the windows first to avoid any uneven window closing process.
                    ((App)Application.Current).MainWin?.AppWin?.Hide();
                    ((App)Application.Current).SettingsWin?.AppWin?.Hide();

                    if (e.Parameter is TaskbarIcon taskBarIconApp)
                        taskBarIconApp.Dispose(); // Ensure the tray icon is removed.

                    ToastNotificationManager.History.Remove("TaskbarIconApp_Ready", Package.Current.DisplayName);
                    Log.CloseAndFlush();
                    Application.Current.Exit();
                };
                return xamlUiCommand;
            } // end get
        } // end property ExitAppCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to open links in the default browser.
        /// </summary>
        public ICommand OpenLinksInDefaultBrowserCommand
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
        } // end property OpenLinksInDefaultBrowserCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to open the settings window.
        /// </summary>
        public ICommand OpenSettingsWindowCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    if (((App)Application.Current).SettingsWin == null)
                        ((App)Application.Current).SettingsWin = new SettingsWindow();

                    ((App)Application.Current).SettingsWin.Activate();
                };
                return xamlUiCommand;
            } // end get
        } // end property OpenSettingsWindowCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to show logs in Explorer.
        /// </summary>
        public ICommand ShowLogsInExplorerCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    Process.Start(new ProcessStartInfo
                        { FileName = ((App)Application.Current).LogsDirectory, UseShellExecute = true });
                };
                return xamlUiCommand;
            } // end get
        } // end property ShowLogsInExplorerCommand

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to show the main window.
        /// </summary>
        public ICommand ToggleMainWindowVisibilityCommand
#pragma warning restore CA1822 // Mark members as static
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, e) =>
                {
                    if (e.Parameter is not AppWindow appWindowMain) return;

                    if (appWindowMain.IsVisible)
                        appWindowMain.Hide();
                    else
                        appWindowMain.Show();
                };
                return xamlUiCommand;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand

        #endregion Properties
    } // end class CommandsViewModel
} // end namespace PaimonTray.ViewModels