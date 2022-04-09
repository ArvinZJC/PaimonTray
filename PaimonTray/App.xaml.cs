using Microsoft.UI.Xaml;
using PaimonTray.Windows;

namespace PaimonTray
{
    /// <summary>
    /// Provide application-specific behaviour to supplement the default App class.
    /// </summary>
    public partial class App
    {
        #region Constructors

        /// <summary>
        /// Initialise the singleton application object.
        /// This is the 1st line of authored code executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        } // end constructor App

        #endregion Constructors

        #region Event Handlers

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// Other entry points will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            new MainWindow().Activate();
        } // end method OnLaunched

        #endregion Event Handlers
    } // end class App
} // end namespace PaimonTray