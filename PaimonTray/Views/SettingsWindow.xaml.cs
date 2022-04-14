using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using PaimonTray.Helpers;
using Serilog;

namespace PaimonTray.Views
{
    /// <summary>
    /// The settings window.
    /// </summary>
    public sealed partial class SettingsWindow
    {
        #region Fields

        private AppWindow _appWin;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the settings window.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            CustomiseWindow();
        } // end constructor SettingsWindow

        #endregion Constructors

        #region Methods

        // Customise the window.
        private void CustomiseWindow()
        {
            _appWin = WindowManagementHelper.GetAppWindow(this);

            if (_appWin == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

            _appWin.Closing += AppWin_OnClosing;

            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                _appWin.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                _appWin.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                _appWin.TitleBar.ExtendsContentIntoTitleBar = true;
            }
            else
            {
                ExtendsContentIntoTitleBar = true;
            } // end if...else
        } // end method CustomiseWindow

        #endregion Methods

        #region Event Handlers

        // Handle the AppWindow closing event.
        private void AppWin_OnClosing(object sender, AppWindowClosingEventArgs e)
        {
            _appWin.Hide(); // Hide the main window first to avoid the uneven window closing process.
            ((App)Application.Current).SettingsWin = null;
        } // end method AppWin_OnClosing

        #endregion Event Handlers
    } // end class SettingsWindow
} // end namespace PaimonTray.Views