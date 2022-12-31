using Microsoft.UI.Xaml;
using Microsoft.Win32;
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
                Log.Error("Unhandled exception occurs.");
                LogException(args.Exception);
            };
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                Log.Information("Exiting the app requested.");
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
        /// Determine if the elevated app restart can be processed.
        /// </summary>
        /// <returns>A flag indicating if the elevated app restart can be processed.</returns>
        private static bool DetermineElevatedAppRestart()
        {
            if (Environment.CommandLine.Contains(AppFieldsHelper.TaskIdElevatedAppRestart)) return false;

            if (Environment.OSVersion.Version.Major > AppFieldsHelper.VersionMajorWindows10Or11)
                return true; // Reserved for future Windows versions.

            try
            {
                var registryKeyVersionWindows =
                    Registry.LocalMachine.OpenSubKey(AppFieldsHelper.RegistryKeyVersionWindows);

                if (registryKeyVersionWindows is null)
                {
                    Log.Warning("Failed to get the Windows version registry key.");
                    return false;
                } // end if

                // Reference: https://learn.microsoft.com/windows/apps/windows-app-sdk/stable-channel#elevation
                if (int.TryParse(
                        registryKeyVersionWindows.GetValue(AppFieldsHelper.RegistryNameVersionRevisionWindows)
                            ?.ToString(), out var versionRevisionWindows))
                    return Environment.OSVersion.Version.Major == AppFieldsHelper.VersionMajorWindows10Or11 &&
                           ((Environment.OSVersion.Version.Build is >= AppFieldsHelper.VersionBuildMinWindows10Elevation
                                 and < AppFieldsHelper.VersionBuildMinWindows11 &&
                             versionRevisionWindows >= AppFieldsHelper.VersionRevisionMinWindows10Elevation) ||
                            (Environment.OSVersion.Version.Build >= AppFieldsHelper.VersionBuildMinWindows11 &&
                             versionRevisionWindows >=
                             AppFieldsHelper
                                 .VersionRevisionMinWindows11Elevation)); // Currently, the way to distinguish Windows 10 and 11 is by build version. Reference: https://stackoverflow.com/a/69922526

                return false;
            }
            catch (Exception exception)
            {
                Log.Error("Failed to get the Windows build version.");
                LogException(exception);
                return false;
            } // end try...catch
        } // end method DetermineAppRestart

        /// <summary>
        /// Generate the app version from the package version.
        /// </summary>
        private void GenerateAppVersion()
        {
            var packageVersion = Package.Current.Id.Version; // Get the package version first.
            var appVersionBase = $"{packageVersion.Major}.{packageVersion.Minor}";
            var suffix = packageVersion.Build switch
            {
                < AppFieldsHelper.VersionBuildBetaMin => $"{AppFieldsHelper.BuildAlpha}.{packageVersion.Build + 1}",
                < AppFieldsHelper.VersionBuildStableMin =>
                    $"{AppFieldsHelper.BuildBeta}.{packageVersion.Build - AppFieldsHelper.VersionBuildBetaMin + 1}",
                _ => $"{AppFieldsHelper.BuildStable}.{packageVersion.Build - AppFieldsHelper.VersionBuildStableMin + 1}"
            };

            AppVersion = $"{appVersionBase}-{suffix}";
            AppVersionTag = $"{appVersionBase}.{packageVersion.Build}-{Architecture}";
        } // end method GetAppVersion

        /// <summary>
        /// Log an exception.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public static void LogException(Exception exception)
        {
            Log.Error($"  - Message: {exception.Message}");
            Log.Error($"  - Inner exception: {exception.InnerException}");
            Log.Error($"  - Stack trace: {exception.StackTrace}");
            Log.Error($"  - HRESULT: {exception.HResult}");
            Log.Error($"  - Help link: {exception.HelpLink}");
        } // end method LogException

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// Other entry points will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            WindowsH =
                new WindowsHelper(); // Need to initialise the windows helper at first for showing a deployment failure window when necessary.

            if (DeploymentManager.GetStatus().Status is not DeploymentStatus.Ok)
            {
                Log.Warning("The Windows App SDK runtime not in a good deployment status.");

                var initialiseTask = Task.Run(() =>
                    DeploymentManager.Initialize(new DeploymentInitializeOptions
                        { ForceDeployment = true, OnErrorShowUI = true }));

                initialiseTask.Wait();

                if (initialiseTask.Result.Status is not DeploymentStatus.Ok)
                {
                    Log.Error("Failed to ensure a good deployment status of the Windows App SDK runtime.");
                    LogException(initialiseTask.Result.ExtendedError);
                    _ = new Window(); // Exiting the app takes no effect if no window instances (Reference: https://github.com/microsoft/microsoft-ui-xaml/issues/5931). Put it here to avoid an access violation exception.

                    if (initialiseTask.Result.ExtendedError.HResult == AppFieldsHelper.HResultAccessDenied &&
                        DetermineElevatedAppRestart())
                    {
                        Log.Information("Restarting the app elevated to try fixing the deployment failure.");
                        AppInstance.GetCurrent().UnregisterKey();

                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                Arguments = AppFieldsHelper.TaskIdElevatedAppRestart,
                                FileName = $"shell:appsFolder\\{Package.Current.Id.FamilyName}!App",
                                UseShellExecute = true,
                                Verb = "runas"
                            });
                            Exit();
                            return;
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Failed to restart the app elevated.");
                            LogException(exception);
                        } // end try...catch
                    } // end if

                    Log.Warning("The app cannot resolve the deployment failure.");
                    _ = WindowsH.GetExistingDeploymentFailureWindow();
                    return;
                } // end if
            } // end if

            SettingsH = new SettingsHelper(); // Need to initialise the settings helper as early as possible.
            HttpClientH =
                new HttpClientHelper(); // Need to initialise the HTTP client helper before any other parts requiring the HTTP client.
            AccountsH = new AccountsHelper();
            UrlGitHubRepoRelease = $"{AppFieldsHelper.UrlBaseGitHubRepoRelease}{AppVersionTag}";
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
        /// The architecture.
        /// </summary>
        public static string Architecture => Package.Current.Id.Architecture.ToString().ToLower();

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