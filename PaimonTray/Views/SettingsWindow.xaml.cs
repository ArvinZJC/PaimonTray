using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using PaimonTray.Helpers;
using Serilog;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Graphics;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The settings window.
    /// </summary>
    public sealed partial class SettingsWindow
    {
        #region Fields

        private AppWindow _appWindow;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the settings window. No need to activate it for the 1st time.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
            CustomiseWindowAsync();
            UpdateUiText();
        } // end constructor SettingsWindow

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Customise the title bar.
        /// </summary>
        private void CustomiseTitleBar()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = _appWindow.TitleBar;

                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor =
                    (GridTitleBar.Resources["TitleBarCaptionForeground"] as SolidColorBrush)?.Color;
                titleBar.ButtonHoverBackgroundColor =
                    (GridTitleBar.Resources["TitleBarButtonHoverBackground"] as SolidColorBrush)?.Color;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = GridTitleBar.ActualTheme switch
                {
                    ElementTheme.Dark => Colors.DimGray,
                    ElementTheme.Default => null,
                    ElementTheme.Light => Colors.Silver,
                    _ => null
                };
                titleBar.ButtonPressedBackgroundColor =
                    (GridTitleBar.Resources["TitleBarButtonPressedBackground"] as SolidColorBrush)?.Color;
                titleBar.ExtendsContentIntoTitleBar = true;

                GridColumnTitleBarLeftPadding.Width = new GridLength(titleBar.LeftInset);
                GridColumnTitleBarRightPadding.Width = new GridLength(titleBar.RightInset);
                GridTitleBar.Height = titleBar.Height;
            }
            else
            {
                var app = Application.Current;

                // It is necessary to set the colour rather than the solid colour brush to avoid strange performance.
                ((SolidColorBrush)app.Resources["WindowCaptionButtonBackgroundPointerOver"]).Color =
                    ((SolidColorBrush)GridTitleBar.Resources["TitleBarButtonHoverBackground"]).Color;
                ((SolidColorBrush)app.Resources["WindowCaptionForeground"]).Color =
                    ((SolidColorBrush)GridTitleBar.Resources["TitleBarCaptionForeground"]).Color;
                ((SolidColorBrush)app.Resources["WindowCaptionForegroundDisabled"]).Color =
                    ((SolidColorBrush)GridTitleBar.Resources["TitleBarCaptionInactiveForeground"]).Color;

                ExtendsContentIntoTitleBar = true;
                SetTitleBar(GridTitleBar);
            } // end if...else
        } // end method CustomiseTitleBar

        /// <summary>
        /// Customise the window.
        /// </summary>
        private async void CustomiseWindowAsync()
        {
            var windowId = WindowsHelper.GetWindowId(this);

            _appWindow = WindowsHelper.GetAppWindow(windowId);

            if (_appWindow == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

            _appWindow.SetIcon(
                (await StorageFile.GetFileFromApplicationUriAsync(new Uri(AppConstantsHelper.UriAppIcon))).Path);
            CustomiseTitleBar();

            var workArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary).WorkArea;

            _appWindow.Move(new PointInt32((workArea.Width - _appWindow.Size.Width) / 2,
                (workArea.Height - _appWindow.Size.Height) / 2));
        } // end method CustomiseWindowAsync

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            NavigationViewItemBodyAbout.Content = resourceLoader.GetString("About");
            NavigationViewItemBodyAccounts.Content = resourceLoader.GetString("Accounts");
            NavigationViewItemBodyGeneral.Content = resourceLoader.GetString("General");
            TextBlockWindowTitle.Text = resourceLoader.GetString("Settings");
            Title = $"{resourceLoader.GetString("Settings")} - {Package.Current.DisplayName}";
        } // end method UpdateUiText

        #endregion Methods

        #region Events

        // Handle the title bar grid's actual theme changed event.
        private void GridTitleBar_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            CustomiseTitleBar();
        } // end method GridTitleBar_OnActualThemeChanged

        // Handle the body navigation view's display mode changed event.
        private void NavigationViewBody_OnDisplayModeChanged(NavigationView sender,
            NavigationViewDisplayModeChangedEventArgs args)
        {
            NavigationViewBody.IsPaneToggleButtonVisible = args.DisplayMode != NavigationViewDisplayMode.Expanded;
            ScrollViewerBody.Padding = args.DisplayMode == NavigationViewDisplayMode.Minimal
                ? new Thickness(18, 18, 18, 0)
                : new Thickness(56, 18, 56, 0);
        } // end method NavigationViewBody_OnDisplayModeChanged

        // Handle the body navigation view's loaded event.
        private void NavigationViewBody_OnLoaded(object sender, RoutedEventArgs e)
        {
            Activate();
        } // end method NavigationViewBody_OnLoaded

        // Handle the body navigation view's selection changed event.
        private void NavigationViewBody_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            Type pageType;

            if (args.SelectedItem as NavigationViewItem == NavigationViewItemBodyAbout)
                pageType = typeof(AboutAppPage);
            else if (args.SelectedItem as NavigationViewItem == NavigationViewItemBodyAccounts)
                pageType = typeof(AccountsSettingsPage);
            else
                pageType = typeof(GeneralSettingsPage);

            FrameBody.Navigate(pageType, null, new EntranceNavigationTransitionInfo());
            NavigationViewBody.Header = (args.SelectedItem as NavigationViewItem)?.Content;
        } // end method NavigationViewBody_OnSelectionChanged

        // Handle the settings window's activated event.
        private void SettingsWindow_OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                TextBlockWindowTitle.Foreground =
                    GridTitleBar.Resources["TitleBarCaptionInactiveForeground"] as SolidColorBrush;
            else
                TextBlockWindowTitle.Foreground =
                    GridTitleBar.Resources["TitleBarCaptionForeground"] as SolidColorBrush;
        } // end method SettingsWindow_OnActivated

        #endregion Event Handlers
    } // end class SettingsWindow
} // end namespace PaimonTray.Views