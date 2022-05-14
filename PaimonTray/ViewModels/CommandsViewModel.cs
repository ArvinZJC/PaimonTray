using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using Serilog;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Notifications;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The commands view model.
    /// </summary>
    internal class CommandsViewModel
    {
        #region Properties

#pragma warning disable CA1822 // Mark members as static
        /// <summary>
        /// The command to add an account.
        /// </summary>
        public ICommand AddAccountCommand
#pragma warning restore CA1822 // Mark members as static

        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    var mainWindow = WindowsHelper.ShowMainWindow();

                    mainWindow.NavigationViewBody.SelectedItem = mainWindow.NavigationViewBody.MenuItems.Last();

                    if (!mainWindow.Visible) ToggleMainWindowVisibilityCommand.Execute(null);
                };
                xamlUiCommand.IconSource = new SymbolIconSource() { Symbol = Symbol.AddFriend };
                xamlUiCommand.Label = ResourceLoader.GetForViewIndependentUse().GetString("AddAccount");
                return xamlUiCommand;
            } // end get
        } // end property AddAccountCommand

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

                    ToastNotificationManager.History.Clear();
                    Log.CloseAndFlush();
                    Application.Current.Exit();
                };
                xamlUiCommand.Label = ResourceLoader.GetForViewIndependentUse().GetString("Exit");
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
                xamlUiCommand.IconSource = new SymbolIconSource() { Symbol = Symbol.Setting };
                xamlUiCommand.Label = ResourceLoader.GetForViewIndependentUse().GetString("Settings");
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
                    var resourceLoader = ResourceLoader.GetForViewIndependentUse();

                    // Set the text of the main window's menu flyout item for toggling the main window's visibility here to avoid any possible exception when setting in the main window's visibility changed event.
                    if (mainWindow.Visible)
                    {
                        mainWindow.Hide();
                        mainWindow.MenuFlyoutItemAppMenuToggleMainWindowVisibility.Text =
                            resourceLoader.GetString("ShowMainWindow");
                    }
                    else
                    {
                        mainWindow.Show();
                        mainWindow.MenuFlyoutItemAppMenuToggleMainWindowVisibility.Text =
                            resourceLoader.GetString("HideMainWindow");
                    } // end if...else
                };
                return xamlUiCommand;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand

        #endregion Properties
    } // end class CommandsViewModel
} // end namespace PaimonTray.ViewModels