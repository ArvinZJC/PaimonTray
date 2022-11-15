using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using PaimonTray.Helpers;
using PaimonTray.Views;
using PaimonTray.ViewModels;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;
using AppInstance = Microsoft.Windows.AppLifecycle.AppInstance;

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
            Log.Information(
                $"{Package.Current.DisplayName} v{AppVersionTag} ({Package.Current.Id.FamilyName}) started.");
            InitializeComponent();
            UnhandledException += (_, args) =>
            {
                Log.Error($"Unhandled exception: {args.Message}");
                Log.Error($"  - Inner exception: {args.Exception.InnerException}");
                Log.Error($"  - Stack trace: {args.Exception.StackTrace}");
                Log.Error($"  - HRESULT: {args.Exception.HResult}");
                Log.Error($"  - Help link: {args.Exception.HelpLink}");
                Log.CloseAndFlush();
            };
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
        /// Exit the app with no window.
        /// </summary>
        private void ExitAppWithNoWindow()
        {
            Log.CloseAndFlush();
            _ = new Window(); // Exiting the app takes no effect if no window instances. (Reference: https://github.com/microsoft/microsoft-ui-xaml/issues/5931)
            Exit();
        } // end method ExitAppWithNoWindow

        /// <summary>
        /// Generate the app version from the package version.
        /// </summary>
        private void GenerateAppVersion()
        {
            var packageVersion = Package.Current.Id.Version; // Get the package version first.
            var appVersionBase = $"{packageVersion.Major}.{packageVersion.Minor}";
            string suffix;

            if (packageVersion.Build < AppFieldsHelper.VersionBuildBetaMin)
                suffix = $"{AppFieldsHelper.BuildAlpha}.{packageVersion.Build + 1}";
            else if (packageVersion.Build < AppFieldsHelper.VersionBuildStableMin)
                suffix =
                    $"{AppFieldsHelper.BuildBeta}.{packageVersion.Build - AppFieldsHelper.VersionBuildBetaMin + 1}";
            else
                suffix =
                    $"{AppFieldsHelper.BuildStable}.{packageVersion.Build - AppFieldsHelper.VersionBuildStableMin + 1}";

            AppVersion = $"{appVersionBase}-{suffix}";
            AppVersionTag = $"{appVersionBase}.{packageVersion.Build}";
        } // end method GetAppVersion

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// Other entry points will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (DeploymentManager.GetStatus().Status is not DeploymentStatus.Ok)
            {
                Log.Warning("The Windows App SDK runtime not in a good deployment status.");

                var initialiseTask = Task.Run(() =>
                    DeploymentManager.Initialize(new DeploymentInitializeOptions { ForceDeployment = true }));

                initialiseTask.Wait();

                if (initialiseTask.Result.Status is not DeploymentStatus.Ok)
                {
                    Log.Error(
                        $"Failed to ensure a good deployment status of the Windows App SDK runtime: {initialiseTask.Result.ExtendedError.Message}");
                    Log.Error($"  - Inner exception: {initialiseTask.Result.ExtendedError.InnerException}");
                    Log.Error($"  - Stack trace: {initialiseTask.Result.ExtendedError.StackTrace}");
                    Log.Error($"  - HRESULT: {initialiseTask.Result.ExtendedError.HResult}");
                    Log.Error($"  - Help link: {initialiseTask.Result.ExtendedError.HelpLink}");

                    if (initialiseTask.Result.ExtendedError.HResult == AppFieldsHelper.HResultAccessDenied &&
                        !Environment.CommandLine.Contains(AppFieldsHelper.TaskIdElevatedAppRestart))
                    {
                        Log.Information("Restarting the app elevated to try fixing the deployment failure.");
                        AppInstance.GetCurrent().UnregisterKey();
                        ExitAppWithNoWindow(); // Need to exit the app before starting a new app instance elevated.
                        Process.Start(new ProcessStartInfo
                        {
                            Arguments = AppFieldsHelper.TaskIdElevatedAppRestart,
                            FileName = $"shell:appsFolder\\{Package.Current.Id.FamilyName}!App",
                            UseShellExecute = true,
                            Verb = "runas"
                        });
                    }
                    else
                    {
                        Log.Information(
                            "The app cannot solve the deployment failure, will open the Windows App SDK runtime download link, and will be exited.");
                        await Launcher.LaunchUriAsync(new Uri(
                            $"{AppFieldsHelper.UrlBaseWindowsAppSdkRuntimeDownload}" +
                            $"{AppFieldsHelper.VersionMajorNuGetWindowsAppSdk}.{AppFieldsHelper.VersionMinorNuGetWindowsAppSdk}/" +
                            $"{AppFieldsHelper.VersionMajorNuGetWindowsAppSdk}.{AppFieldsHelper.VersionMinorNuGetWindowsAppSdk}.{AppFieldsHelper.VersionBuildNuGetWindowsAppSdk}.{AppFieldsHelper.VersionRevisionNuGetWindowsAppSdk}/" +
                            $"windowsappruntimeinstall-{Package.Current.Id.Architecture.ToString().ToLower()}.exe"));
                        ExitAppWithNoWindow();
                    } // end if...else

                    return;
                } // end if
            } // end if

            SettingsH = new SettingsHelper(); // Need to initialise the settings helper first.
            HttpClientH =
                new HttpClientHelper(); // Need to initialise the HTTP client helper before any other parts requiring the HTTP client.
            AccountsH = new AccountsHelper();
            UrlGitHubRepoRelease = $"{AppFieldsHelper.UrlBaseGitHubRepoRelease}{AppVersionTag}";
            WindowsH = new WindowsHelper();
            CommandsVm =
                new CommandsViewModel(
                    WindowsH.GetExistingMainWindow()
                        ?.Win as MainWindow); // Need to initialise the commands view model at last.

            base.OnLaunched(e);
        } // end method OnLaunched

        #endregion Methods

        #region Properties

        /// <summary>
        /// The accounts helper.
        /// </summary>
        public AccountsHelper AccountsH { get; private set; }

        /// <summary>
        /// The app version.
        /// </summary>
        public string AppVersion { get; private set; }

        /// <summary>
        /// The app version tag.
        /// </summary>
        public string AppVersionTag { get; private set; }

        /// <summary>
        /// The commands view model.
        /// </summary>
        public CommandsViewModel CommandsVm { get; private set; }

        /// <summary>
        /// The HTTP client helper.
        /// </summary>
        public HttpClientHelper HttpClientH { get; private set; }

        /// <summary>
        /// The logs directory.
        /// </summary>
        public string LogsDirectory { get; private set; }

        /// <summary>
        /// The settings helper.
        /// </summary>
        public SettingsHelper SettingsH { get; private set; }

        /// <summary>
        /// The app's GitHub repository's specific release URL.
        /// </summary>
        public string UrlGitHubRepoRelease { get; private set; }

        /// <summary>
        /// The windows helper.
        /// </summary>
        public WindowsHelper WindowsH { get; private set; }

        #endregion Properties
    } // end class App
} // end namespace PaimonTray