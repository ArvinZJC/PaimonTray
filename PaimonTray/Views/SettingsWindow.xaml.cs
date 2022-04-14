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
            var appWin = WindowManagementHelper.GetAppWindow(this);

            if (appWin == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

            appWin.Closing += AppWin_OnClosing;
        } // end method CustomiseWindow

        #endregion Methods

        #region Event Handlers

        // Handle the AppWindow closing event.
        private static void AppWin_OnClosing(object sender, AppWindowClosingEventArgs e)
        {
            ((App)Application.Current).SettingsWin = null;
        } // end method AppWin_OnClosing

        #endregion Event Handlers
    } // end class SettingsWindow
} // end namespace PaimonTray.Views