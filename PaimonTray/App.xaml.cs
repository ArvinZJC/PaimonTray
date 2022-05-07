using Microsoft.UI.Xaml;
using PaimonTray.Helpers;
using Serilog;
using System.IO;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.Storage;

namespace PaimonTray
{
    /// <summary>
    /// Provide application-specific behaviour to supplement the default <see cref="App"/> class.
    /// </summary>
    public partial class App
    {
        #region Properties

        /// <summary>
        /// The app version.
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        /// The accounts helper.
        /// </summary>
        public AccountsHelper AccHelper { get; } 

        /// <summary>
        /// The logs directory.
        /// </summary>
        public string LogsDirectory { get; private set; }

        /// <summary>
        /// The language selection applied at present since the changes will be applied after app restart.
        /// </summary>
        public string LanguageSelectionApplied { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the singleton application object.
        /// </summary>
        public App()
        {
            ConfigLogger();
            GenerateAppVersion();
            Log.Information("{DisplayName} V{AppVersion} started.", Package.Current.DisplayName, AppVersion);
            SettingsHelper.InitialiseSettings();

            // Apply the language selection.
            LanguageSelectionApplied = ApplicationData.Current.LocalSettings
                .Containers[SettingsHelper.ContainerKeySettings].Values[SettingsHelper.KeyLanguage] as string;
            ApplicationLanguages.PrimaryLanguageOverride = LanguageSelectionApplied == SettingsHelper.TagSystem
                ? string.Empty
                : LanguageSelectionApplied;

            AccHelper = new AccountsHelper();
            InitializeComponent();
        } // end constructor App

        #endregion Constructors

        #region Methods

        // Configure the logger.
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

        // Generate the app version from the package version.
        private void GenerateAppVersion()
        {
            var packageVersion = Package.Current.Id.Version;
            var suffix = packageVersion.Revision switch
            {
                < AppConstantsHelper.VersionRevisionBetaMin => $"-alpha.{packageVersion.Revision + 1}",
                < AppConstantsHelper.VersionRevisionStable =>
                    $"-beta.{packageVersion.Revision - AppConstantsHelper.VersionRevisionBetaMin + 1}",
                _ => string.Empty
            };

            AppVersion = $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}{suffix}";
        } // end method GetAppVersion

        #endregion Methods

        #region Event Handlers

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// Other entry points will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            WindowsHelper.ShowMainWindow();
        } // end method OnLaunched

        #endregion Event Handlers
    } // end class App
} // end namespace PaimonTray