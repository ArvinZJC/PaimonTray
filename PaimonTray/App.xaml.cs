using Microsoft.UI.Xaml;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
using Serilog;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;
using PaimonTray.Views;

namespace PaimonTray
{
    /// <summary>
    /// Provide application-specific behaviour to supplement the default <see cref="App"/> class.
    /// </summary>
    public partial class App
    {
        #region Constructors

        /// <summary>
        /// Initialise the singleton application object.
        /// </summary>
        public App()
        {
            ConfigLogger();
            GenerateAppVersion();
            Log.Information($"{Package.Current.DisplayName} V{AppVersion} started.");
            SettingsH = new SettingsHelper(); // Need to initialise the settings helper first.
            HttpClientH =
                new HttpClientHelper(); // Need to initialise the HTTP client helper before any other parts requiring the HTTP client.
            AccountsH = new AccountsHelper();
            UrlGitHubRepoRelease = $"{AppFieldsHelper.UrlBaseGitHubRepoRelease}{AppVersion}";
            WindowsH = new WindowsHelper();
            InitializeComponent();
        } // end constructor App

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Configure the logger.
        /// </summary>
        private void ConfigLogger()
        {
            LoggerConfiguration loggerConfig = new();

            if (Package.Current.IsDevelopmentMode) loggerConfig.MinimumLevel.Debug();

            LogsDirectory = Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "Logs");
            Log.Logger = loggerConfig.WriteTo.Async(a =>
                a.File(
                    Path.Combine(LogsDirectory,
                        "log_" + (Package.Current.IsDevelopmentMode ? "dev_" : string.Empty) + ".log"),
                    rollingInterval: RollingInterval.Day)).CreateLogger();
        } // end method ConfigLogger

        /// <summary>
        /// Generate the app version from the package version.
        /// </summary>
        private void GenerateAppVersion()
        {
            var packageVersion = Package.Current.Id.Version;

            AppVersion = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}";
        } // end method GetAppVersion

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// Other entry points will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            CommandsVm = new CommandsViewModel(WindowsH.GetExistingMainWindow()?.Win as MainWindow);
            base.OnLaunched(e);
        } // end method OnLaunched

        #endregion Methods

        #region Properties

        /// <summary>
        /// The accounts helper.
        /// </summary>
        public AccountsHelper AccountsH { get; }

        /// <summary>
        /// The app version.
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        /// The commands view model.
        /// </summary>
        public CommandsViewModel CommandsVm { get; private set; }

        /// <summary>
        /// The HTTP client helper.
        /// </summary>
        public HttpClientHelper HttpClientH { get; }

        /// <summary>
        /// The logs directory.
        /// </summary>
        public string LogsDirectory { get; private set; }

        /// <summary>
        /// The settings helper.
        /// </summary>
        public SettingsHelper SettingsH { get; }

        /// <summary>
        /// The app's GitHub repository's specific release URL.
        /// </summary>
        public string UrlGitHubRepoRelease { get; }

        /// <summary>
        /// The windows helper.
        /// </summary>
        public WindowsHelper WindowsH { get; }

        #endregion Properties
    } // end class App
} // end namespace PaimonTray