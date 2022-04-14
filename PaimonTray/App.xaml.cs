using Microsoft.UI.Xaml;
using PaimonTray.Views;

namespace PaimonTray
{
    /// <summary>
    /// Provide application-specific behaviour to supplement the default <see cref="App"/> class.
    /// </summary>
    public partial class App
    {
        #region Properties

        /// <summary>
        /// The main window.
        /// </summary>
        public Window MainWindow { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the singleton application object.
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
            MainWindow = new MainWindow();
        } // end method OnLaunched

        #endregion Event Handlers
    } // end class App
} // end namespace PaimonTray