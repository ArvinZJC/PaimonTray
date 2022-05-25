using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using PaimonTray.Views;
using Serilog;
using System.Diagnostics;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Notifications;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The commands view model.
    /// </summary>
    public class CommandsViewModel
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// The resource loader.
        /// </summary>
        private readonly ResourceLoader _resourceLoader;

        #endregion Fields

        #region Properties

        /// <summary>
        /// The command to add/update an account.
        /// </summary>
        public ICommand AddUpdateAccountCommand

        {
            get
            {
                XamlUICommand xamlUiCommand = new()
                {
                    IconSource = new SymbolIconSource { Symbol = Symbol.AddFriend },
                    Label = _resourceLoader.GetString("AccountAddUpdate")
                };

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    _mainWindow.NavigationViewBody.SelectedItem = _mainWindow.NavigationViewItemBodyAddUpdateAccount;

                    if (!_mainWindow.Visible) ToggleMainWindowVisibilityCommand.Execute(null); // TODO
                };
                return xamlUiCommand;
            } // end get
        } // end property AddUpdateAccountCommand

        /// <summary>
        /// The command to exit the app.
        /// </summary>
        public ICommand ExitAppCommand
        {
            get
            {
                XamlUICommand xamlUiCommand = new() { Label = _resourceLoader.GetString("Exit") };

                xamlUiCommand.ExecuteRequested += (_, e) =>
                {
                    Log.Information("Exit the app requested.");

                    if (e.Parameter is TaskbarIcon taskBarIconApp)
                        taskBarIconApp.Dispose(); // Ensure the tray icon is removed.

                    if ((bool)ApplicationData.Current.LocalSettings.Containers[SettingsHelper.ContainerKeySettings]
                            .Values[SettingsHelper.KeyNotificationClear]) ToastNotificationManager.History.Clear();

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

        /// <summary>
        /// The command to open/activate the settings window.
        /// </summary>
        public ICommand ShowSettingsWindowCommand
        {
            get
            {
                XamlUICommand xamlUiCommand = new()
                {
                    IconSource = new SymbolIconSource { Symbol = Symbol.Setting },
                    Label = _resourceLoader.GetString("Settings")
                };

                xamlUiCommand.ExecuteRequested += (_, _) => _app.WindowsH.ShowSettingsWindow();
                return xamlUiCommand;
            } // end get
        } // end property ShowSettingsWindowCommand

        /// <summary>
        /// The command to show/hide the main window.
        /// </summary>
        public ICommand ToggleMainWindowVisibilityCommand
        {
            get
            {
                XamlUICommand xamlUiCommand = new();

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    // Set the text of the main window's menu flyout item for the main window's visibility here to avoid any possible exception when setting in the main window's visibility changed event.
                    if (_mainWindow.Visible)
                    {
                        _mainWindow.Hide();
                        _mainWindow.MenuFlyoutItemAppMenuMainWindowVisibility.Text =
                            _resourceLoader.GetString("MainWindowShow");

                        var navigationViewItemBodyRealTimeNotes = _mainWindow.NavigationViewItemBodyRealTimeNotes;

                        if (navigationViewItemBodyRealTimeNotes.IsEnabled)
                            _mainWindow.NavigationViewBody.SelectedItem = navigationViewItemBodyRealTimeNotes;
                    }
                    else
                    {
                        _mainWindow.Show();
                        _mainWindow.MenuFlyoutItemAppMenuMainWindowVisibility.Text =
                            _resourceLoader.GetString("MainWindowHide");
                    } // end if...else
                };
                return xamlUiCommand;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the commands view model.
        /// </summary>
        public CommandsViewModel(MainWindow mainWindow)
        {
            _app = Application.Current as App;
            _mainWindow = mainWindow;
            _resourceLoader = ResourceLoader.GetForViewIndependentUse();
        } // end constructor CommandsViewModel

        #endregion Constructors
    } // end class CommandsViewModel
} // end namespace PaimonTray.ViewModels