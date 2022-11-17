using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using PaimonTray.Helpers;
using PaimonTray.Views;
using System;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Notifications;

namespace PaimonTray.ViewModels
{
    /// <summary>
    /// The commands view model.
    /// </summary>
    public class CommandsViewModel
    {
        #region Constructors

        /// <summary>
        /// Initialise the commands view model.
        /// </summary>
        public CommandsViewModel(MainWindow mainWindow)
        {
            _app = Application.Current as App;
            _mainWindow = mainWindow;
        } // end constructor CommandsViewModel

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~CommandsViewModel()
        {
            _app = null;
            _mainWindow = null;
        } // end destructor

        #endregion Destructor

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

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
                    IconSource = new FontIconSource { Glyph = AppFieldsHelper.GlyphAddFriend },
                    Label = _app.SettingsH.ResLoader.GetString("AccountAddUpdate")
                };

                xamlUiCommand.ExecuteRequested += (_, _) =>
                {
                    if (_mainWindow is null) return;

                    _mainWindow.NavigationViewBody.SelectedItem = _mainWindow.NavigationViewItemBodyAccountAddUpdate;

                    if (!_mainWindow.Visible) ToggleMainWindowVisibilityCommand.Execute(null);
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
                XamlUICommand xamlUiCommand = new() { Label = _app.SettingsH.ResLoader.GetString("Exit") };

                xamlUiCommand.ExecuteRequested += (_, e) =>
                {
                    if (e.Parameter is TaskbarIcon taskBarIconApp)
                        taskBarIconApp.Dispose(); // Ensure the tray icon is removed.

                    if (_app.SettingsH.PropertySetSettings[SettingsHelper.KeyNotificationClear] is true)
                        ToastNotificationManager.History.Clear();

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

                xamlUiCommand.ExecuteRequested += async (_, e) =>
                {
                    if (e.Parameter is string link)
                        await Launcher.LaunchUriAsync(new Uri(link));
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
                    IconSource = new FontIconSource { Glyph = AppFieldsHelper.GlyphSettings },
                    Label = _app.SettingsH.ResLoader.GetString("Settings")
                };

                xamlUiCommand.ExecuteRequested += (_, _) => _app.WindowsH.GetExistingSettingsWindow();
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
                    if (_mainWindow is null) return;

                    var resourceLoader = _app.SettingsH.ResLoader;

                    // Set the text of the main window's menu flyout item for the main window's visibility here to avoid any possible exception when setting in the main window's visibility changed event.
                    if (_mainWindow.Visible)
                    {
                        _mainWindow.Hide();
                        _mainWindow.MenuFlyoutItemAppMenuMainWindowVisibility.Text =
                            resourceLoader.GetString("MainWindowShow");

                        var navigationViewItemBodyRealTimeNotes = _mainWindow.NavigationViewItemBodyRealTimeNotes;

                        if (navigationViewItemBodyRealTimeNotes.IsEnabled)
                            _mainWindow.NavigationViewBody.SelectedItem = navigationViewItemBodyRealTimeNotes;
                    }
                    else
                    {
                        _mainWindow.Show();
                        _mainWindow.MenuFlyoutItemAppMenuMainWindowVisibility.Text =
                            resourceLoader.GetString("MainWindowHide");
                    } // end if...else
                };
                return xamlUiCommand;
            } // end get
        } // end property ToggleMainWindowVisibilityCommand

        #endregion Properties
    } // end class CommandsViewModel
} // end namespace PaimonTray.ViewModels