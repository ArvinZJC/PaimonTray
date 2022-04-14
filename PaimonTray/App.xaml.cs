using Microsoft.UI.Xaml;
using PaimonTray.Views;
using Serilog;
using System.IO;
using Windows.ApplicationModel;

namespace PaimonTray
{
    /// <summary>
    /// Provide application-specific behaviour to supplement the default <see cref="App"/> class.
    /// </summary>
    public partial class App
    {
        #region Properties

        /// <summary>
        /// The logs directory.
        /// </summary>
        public string LogsDirectory { get; private set; }

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
            ConfigLogger();
            Log.Information("{DisplayName} V{Major}.{Minor}.{Build}.{Revision} started.", Package.Current.DisplayName,
                Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build,
                Package.Current.Id.Version.Revision);
        } // end constructor App

        #endregion Constructors

        #region Methods

        // Configure the logger.
        private void ConfigLogger()
        {
            LoggerConfiguration loggerConfig = new();

            if (Package.Current.IsDevelopmentMode)
                loggerConfig.MinimumLevel.Debug();

            LogsDirectory = Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path, "Logs");
            Log.Logger = loggerConfig.WriteTo.Async(a =>
                a.File(Path.Combine(LogsDirectory, "log_" + (Package.Current.IsDevelopmentMode ? "dev_" : "") + ".log"),
                    rollingInterval: RollingInterval.Day)).CreateLogger();
        } // end method ConfigLogger

        #endregion Methods

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