using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using PaimonTray.Helpers;
using Serilog;
using System;
using Windows.ApplicationModel;
using Windows.Graphics;
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
        /// Initialise the settings window. No need to activate it for the 1st time.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            CustomiseWindowAsync();

            NavigationViewItemRootAboutApp.Content = $"About {Package.Current.DisplayName}";
        } // end constructor SettingsWindow

        #endregion Constructors

        #region Methods

        // Customise the window.
        private async void CustomiseWindowAsync()
        {
            var windowId = WindowManagementHelper.GetWindowId(this);

            AppWin = WindowManagementHelper.GetAppWindow(windowId);
            Title = $"Settings - {Package.Current.DisplayName}";

            if (AppWin == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

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

            var workArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary).WorkArea;
            var windowHeight = workArea.Height * 2 / 3;
            var windowWidth = workArea.Width * 2 / 3;

            AppWin.MoveAndResize(new RectInt32
            {
                Height = windowHeight, Width = windowWidth,
                X = (workArea.Width - windowWidth) / 2,
                Y = (workArea.Height - windowHeight) / 2
            });
        } // end method CustomiseWindowAsync

        #endregion Methods

        #region Event Handlers

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

        // Handle the root navigation view's loaded event.
        private void NavigationViewRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            Activate();
        } // end method NavigationViewRoot_OnLoaded

        // Handle the root navigation view's selection changed event.
        private void NavigationViewRoot_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            Type pageType;

            switch (((NavigationViewItem)args.SelectedItem).Tag)
            {
                case "AboutApp":
                    pageType = typeof(AboutAppPage);
                    break;

                case "AccountsSettings":
                    pageType = typeof(AccountsSettingsPage);
                    break;

                case "GeneralSettings":
                    pageType = typeof(GeneralSettingsPage);
                    break;

                default:
                    pageType = typeof(GeneralSettingsPage);
                    Log.Warning(
                        $"Failed to identify the selected item in the settings window's root navigation view by tag. (Content: {((NavigationViewItem)args.SelectedItem).Content}, tag: {((NavigationViewItem)args.SelectedItem).Tag})");
                    break;
            } // end switch-case

            FrameRoot.Navigate(pageType);
            NavigationViewRoot.Header = ((NavigationViewItem)args.SelectedItem).Content;
        } // end method NavigationViewRoot_OnSelectionChanged

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