using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaimonTray.Helpers;
using PaimonTray.Models;
using Serilog;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Graphics;
using Windows.System;
using DispatcherQueueTimer = Microsoft.UI.Dispatching.DispatcherQueueTimer;

namespace PaimonTray.Views
{
    /// <summary>
    /// The deployment failure window.
    /// </summary>
    public sealed partial class DeploymentFailureWindow
    {
        #region Constructors

        /// <summary>
        /// Initialise the deployment failure window. No need to activate it manually for the 1st time.
        /// </summary>
        public DeploymentFailureWindow()
        {
            InitializeComponent();
            _ = CustomiseWindowAsync();
            UpdateUiText();
        } // end constructor DeploymentFailureWindow

        #endregion Constructors

        #region Event Handlers

        // Handle the deployment failure window's closed event.
        private void DeploymentFailureWindow_OnClosed(object sender, WindowEventArgs args)
        {
            _appWindow = null;
            _dispatcherQueueTimer.Stop();
            _dispatcherQueueTimer.Tick -= DispatcherQueueTimer_OnTick;
            _existingWindow = null;
        } // end method DeploymentFailureWindow_OnClosed

        // Handle the dispatcher queue timer's tick event.
        private void DispatcherQueueTimer_OnTick(object sender, object e)
        {
            var workArea = WindowsHelper.GetWorkArea(_windowId);

            if (workArea.Height == _workAreaHeightLast && workArea.Width == _workAreaWidthLast) return;

            SynchroniseContentDialogueWindowSize();
        } // end method DispatcherQueueTimer_OnTick

        // Handle the root grid's actual theme changed event.
        private void GridRoot_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetRootGridBackground();
        } // end method GridRoot_OnActualThemeChanged

        // Handle the root grid's loaded event.
        private async void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            _dispatcherQueueTimer = DispatcherQueue.CreateTimer();
            _dispatcherQueueTimer.Interval = TimeSpan.FromSeconds(1);
            _dispatcherQueueTimer.Tick += DispatcherQueueTimer_OnTick;
            SetRootGridBackground();
            SynchroniseContentDialogueWindowSize();
            _dispatcherQueueTimer.Start(); // Start the dispatcher queue timer when ready.
            Activate(); // Activate when ready.

            var contentDialogueResultDeploymentFailure = await ContentDialogueDeploymentFailure.ShowAsync();

            if (contentDialogueResultDeploymentFailure is ContentDialogResult.Primary)
            {
                var urlWindowsAppSdkRuntimeDownload = $"{AppFieldsHelper.UrlBaseWindowsAppSdkRuntimeDownload}" +
                                                      $"{AppFieldsHelper.VersionMajorNuGetWindowsAppSdk}.{AppFieldsHelper.VersionMinorNuGetWindowsAppSdk}/" +
                                                      $"{AppFieldsHelper.VersionMajorNuGetWindowsAppSdk}.{AppFieldsHelper.VersionMinorNuGetWindowsAppSdk}.{AppFieldsHelper.VersionBuildNuGetWindowsAppSdk}.{AppFieldsHelper.VersionRevisionNuGetWindowsAppSdk}/" +
                                                      $"windowsappruntimeinstall-{App.Architecture}.exe";

                _appWindow.Hide(); // Hide the window first to improve usability.
                Log.Information($"Open the Windows App SDK download link: {urlWindowsAppSdkRuntimeDownload}");
                await Launcher.LaunchUriAsync(
                    new Uri(
                        urlWindowsAppSdkRuntimeDownload)); // Need to wait for finishing the operation before closing the window, so the static property in the commands view model cannot be used.
            } // end if

            Close();
        } // end method GridRoot_OnLoaded

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app window.
        /// </summary>
        private AppWindow _appWindow;

        /// <summary>
        /// The dispatcher queue timer.
        /// </summary>
        private DispatcherQueueTimer _dispatcherQueueTimer;

        /// <summary>
        /// The existing window.
        /// </summary>
        private ExistingWindow _existingWindow;

        /// <summary>
        /// The window ID.
        /// </summary>
        private WindowId _windowId;

        /// <summary>
        /// The work area's last height.
        /// </summary>
        private int _workAreaHeightLast;

        /// <summary>
        /// The work area's last width.
        /// </summary>
        private int _workAreaWidthLast;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Customise the window.
        /// </summary>
        /// <returns>Void.</returns>
        private async Task CustomiseWindowAsync()
        {
            _windowId = WindowsHelper.GetWindowId(this); // Get the window ID first.
            _appWindow = await (Application.Current as App)?.WindowsH.GetAppWindowWithIconAsync(_windowId)!;

            if (_appWindow is null)
            {
                Log.Warning("The deployment failure window's app window is null.");
                return;
            } // end if

            if (_appWindow.Presenter is not OverlappedPresenter appWindowOverlappedPresenter)
            {
                appWindowOverlappedPresenter = OverlappedPresenter.Create();
                _appWindow.SetPresenter(appWindowOverlappedPresenter);
            } // end if

            appWindowOverlappedPresenter.IsMaximizable = false;
            appWindowOverlappedPresenter.IsMinimizable = false;
            appWindowOverlappedPresenter.IsResizable = false;
            appWindowOverlappedPresenter.SetBorderAndTitleBar(true, false);
        } // end method CustomiseWindowAsync

        /// <summary>
        /// Set the root grid's background.
        /// </summary>
        private void SetRootGridBackground()
        {
            if (_existingWindow?.MicaC is null)
            {
                GridRoot.Resources.TryGetValue(
                    _existingWindow?.DesktopAcrylicC is not null
                        ? "RootGridAcrylicBackground"
                        : "RootGridFallbackBackground", out var gridRootBackground);

                if (gridRootBackground is SolidColorBrush solidColourBrushGridRootBackground)
                {
                    GridRoot.Background =
                        new SolidColorBrush(solidColourBrushGridRootBackground
                            .Color); // Code in this way to make the brush transition work properly.
                    return;
                } // end if
            } // end if

            GridRoot.Background = null;
        } // end method SetRootGridBackground

        /// <summary>
        /// Synchronise the size of the content dialogue and the window.
        /// </summary>
        private void SynchroniseContentDialogueWindowSize()
        {
            GridRoot.Resources.TryGetValue("ContentDialogMaxHeight", out var contentDialogueHeightMaxObject);
            GridRoot.Resources.TryGetValue("ContentDialogMaxWidth", out var contentDialogueWidthMaxObject);
            GridRoot.Resources.TryGetValue("ContentDialogMinHeight", out var contentDialogueHeightMinObject);
            GridRoot.Resources.TryGetValue("ContentDialogMinWidth", out var contentDialogueWidthMinObject);
            GridRoot.Resources.TryGetValue("ContentDialogPadding", out var contentDialoguePaddingObject);

            try
            {
                var contentDialogueHeightMax = Convert.ToDouble(contentDialogueHeightMaxObject);
                var contentDialogueHeightMin = Convert.ToDouble(contentDialogueHeightMinObject);
                var contentDialogueHeightSuggested = (int)Math.Floor(
                    (AppFieldsHelper.ContentDialogueDeploymentFailureHeightExpected < contentDialogueHeightMin
                        ? contentDialogueHeightMin
                        : AppFieldsHelper.ContentDialogueDeploymentFailureHeightExpected > contentDialogueHeightMax
                            ? contentDialogueHeightMax
                            : AppFieldsHelper.ContentDialogueDeploymentFailureHeightExpected) *
                    GridRoot.XamlRoot.RasterizationScale);
                var contentDialoguePadding = contentDialoguePaddingObject as Thickness?;
                var contentDialogueWidthMax = Convert.ToDouble(contentDialogueWidthMaxObject);
                var contentDialogueWidthMin = Convert.ToDouble(contentDialogueWidthMinObject);
                var contentDialogueWidthSuggested = (int)Math.Floor(
                    (AppFieldsHelper.ContentDialogueDeploymentFailureWidthExpected < contentDialogueWidthMin
                        ? contentDialogueWidthMin
                        : AppFieldsHelper.ContentDialogueDeploymentFailureWidthExpected > contentDialogueWidthMax
                            ? contentDialogueWidthMax
                            : AppFieldsHelper.ContentDialogueDeploymentFailureWidthExpected) *
                    GridRoot.XamlRoot.RasterizationScale);
                var workArea = WindowsHelper.GetWorkArea(_windowId);

                _appWindow.MoveAndResize(new RectInt32
                {
                    Height = contentDialogueHeightSuggested, Width = contentDialogueWidthSuggested,
                    X = (workArea.Width - contentDialogueWidthSuggested) / 2,
                    Y = (workArea.Height - contentDialogueHeightSuggested) / 2
                });
                _workAreaHeightLast = workArea.Height;
                _workAreaWidthLast = workArea.Width;
                ScrollViewerDeploymentFailure.Width =
                    contentDialogueWidthSuggested / GridRoot.XamlRoot.RasterizationScale -
                    Convert.ToDouble(contentDialoguePadding?.Left + contentDialoguePadding?.Right);
            }
            catch (Exception exception)
            {
                Log.Error(
                    "Failed to synchronise the size of the content dialogue for the deployment failure and the deployment failure window.");
                App.LogException(exception);
            } // end try...catch
        } // end method SynchroniseContentDialogueWindowSize

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            ContentDialogueDeploymentFailure.CloseButtonText = resourceLoader.GetString("No");
            ContentDialogueDeploymentFailure.PrimaryButtonText = resourceLoader.GetString("Yes");
            ContentDialogueDeploymentFailure.Title =
                resourceLoader.GetString("WindowsAppSdkRuntimeDownloadConfirmation");
            TextBlockDeploymentFailure.Text = resourceLoader.GetString("WindowsAppSdkRuntimeDownloadExplanation");
        } // end method UpdateUiText

        #endregion Methods
    } // end class DeploymentFailureWindow
} // end namespace PaimonTray.Views