using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaimonTray.Helpers;
using Serilog;
using System;
using Windows.ApplicationModel;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The settings window.
    /// </summary>
    public sealed partial class SettingsWindow
    {
        #region Properties

        /// <summary>
        /// The settings window's <see cref="AppWindow"/>.
        /// </summary>
        public AppWindow AppWin { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initialise the settings window.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            CustomiseWindowAsync();

            NavigationViewItemRootAbout.Content = $"About {Package.Current.DisplayName}";
        } // end constructor SettingsWindow

        #endregion Constructors

        #region Methods

        // Customise the window.
        private async void CustomiseWindowAsync()
        {
            AppWin = WindowManagementHelper.GetAppWindow(this);
            Title = $"Settings - {Package.Current.DisplayName}";

            if (AppWin == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

            AppWin.Closing += AppWin_OnClosing;
            AppWin.Destroying += AppWin_OnDestroying;
            AppWin.SetIcon((await StorageFile.GetFileFromApplicationUriAsync(
                new Uri("ms-appx:///Assets/AppIcon/AppIcon.ico"))).Path);

            // Customise the title bar.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                AppWin.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                AppWin.TitleBar.ButtonHoverBackgroundColor =
                    ((SolidColorBrush)Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"]).Color;
                AppWin.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                AppWin.TitleBar.ExtendsContentIntoTitleBar = true;
                GridColumnTitleBarLeftPadding.Width = new GridLength(AppWin.TitleBar.LeftInset);
                GridColumnTitleBarRightPadding.Width = new GridLength(AppWin.TitleBar.RightInset);
                GridTitleBar.Height = AppWin.TitleBar.Height;
            }
            else
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(GridTitleBar);
            } // end if...else
        } // end method CustomiseWindowAsync

        #endregion Methods

        #region Event Handlers

        // Handle the AppWindow's closing event.
        private static void AppWin_OnClosing(AppWindow sender, AppWindowClosingEventArgs e)
        {
            ((App)Application.Current).SettingsWin = null;
        } // end method AppWin_OnClosing

        // Handle the AppWindow's destroying event.
        private static void AppWin_OnDestroying(object sender, object e)
        {
            ((App)Application.Current).SettingsWin = null;
        } // end method AppWin_OnDestroying

        // Handle the root navigation view's display mode changed event.
        private void NavigationViewRoot_OnDisplayModeChanged(NavigationView sender,
            NavigationViewDisplayModeChangedEventArgs args)
        {
            NavigationViewRoot.IsPaneToggleButtonVisible = args.DisplayMode != NavigationViewDisplayMode.Expanded;
        } // end method NavigationViewRoot_OnDisplayModeChanged

        // Handle the window's activated event.
        private void SettingsWindow_OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                TextBlockWindowTitle.Foreground =
                    (SolidColorBrush)Application.Current.Resources["WindowCaptionForegroundDisabled"];
            else
                TextBlockWindowTitle.Foreground =
                    (SolidColorBrush)Application.Current.Resources["WindowCaptionForeground"];
        } // end method SettingsWindow_OnActivated

        #endregion Event Handlers
    } // end class SettingsWindow
} // end namespace PaimonTray.Views