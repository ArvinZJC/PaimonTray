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
        } // end constructor SettingsWindow

        #endregion Constructors

        #region Methods

        // Customise the title bar.
        private void CustomiseTitleBar()
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                _appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                _appWindow.TitleBar.ButtonForegroundColor =
                    (GridTitleBar.Resources["TitleBarCaptionForeground"] as SolidColorBrush)?.Color;
                _appWindow.TitleBar.ButtonHoverBackgroundColor =
                    (GridTitleBar.Resources["TitleBarButtonHoverBackground"] as SolidColorBrush)?.Color;
                _appWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                _appWindow.TitleBar.ButtonInactiveForegroundColor = GridTitleBar.ActualTheme switch
                {
                    ElementTheme.Dark => Colors.DimGray,
                    ElementTheme.Default => null,
                    ElementTheme.Light => Colors.Silver,
                    _ => null
                };
                _appWindow.TitleBar.ButtonPressedBackgroundColor =
                    (GridTitleBar.Resources["TitleBarButtonPressedBackground"] as SolidColorBrush)?.Color;
                _appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                GridColumnTitleBarLeftPadding.Width = new GridLength(_appWindow.TitleBar.LeftInset);
                GridColumnTitleBarRightPadding.Width = new GridLength(_appWindow.TitleBar.RightInset);
                GridTitleBar.Height = _appWindow.TitleBar.Height;
            }
            else
            {
                ((SolidColorBrush)Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"]).Color =
                    ((SolidColorBrush)GridTitleBar.Resources["TitleBarButtonHoverBackground"]).Color;
                ((SolidColorBrush)Application.Current.Resources["WindowCaptionForeground"]).Color =
                    ((SolidColorBrush)GridTitleBar.Resources["TitleBarCaptionForeground"]).Color;
                ((SolidColorBrush)Application.Current.Resources["WindowCaptionForegroundDisabled"]).Color =
                    ((SolidColorBrush)GridTitleBar.Resources["TitleBarCaptionInactiveForeground"]).Color;
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(GridTitleBar);
            } // end if...else
        } // end method CustomiseTitleBar

        // Customise the window.
        private async void CustomiseWindowAsync()
        {
            var windowId = WindowsHelper.GetWindowId(this);

            _appWindow = WindowsHelper.GetAppWindow(windowId);
            Title = $"Settings - {Package.Current.DisplayName}";

            if (_appWindow == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

            _appWindow.SetIcon(
                (await StorageFile.GetFileFromApplicationUriAsync(new Uri(AppConstantsHelper.AppIconPath)))
                .Path);
            CustomiseTitleBar();

            var workArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary).WorkArea;

            _appWindow.Move(new PointInt32((workArea.Width - _appWindow.Size.Width) / 2,
                (workArea.Height - _appWindow.Size.Height) / 2));
        } // end method CustomiseWindowAsync

        #endregion Methods

        #region Event Handlers

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
            ScrollViewerRoot.Padding = args.DisplayMode == NavigationViewDisplayMode.Minimal
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

            switch ((args.SelectedItem as NavigationViewItem)?.Tag)
            {
                case AppConstantsHelper.NavigationViewItemTagAboutApp:
                    pageType = typeof(AboutAppPage);
                    break;

                case AppConstantsHelper.NavigationViewItemTagAccountsSettings:
                    pageType = typeof(AccountsSettingsPage);
                    break;

                case AppConstantsHelper.NavigationViewItemTagGeneralSettings:
                case null:
                    pageType = typeof(GeneralSettingsPage);
                    break;

                default:
                    Log.Warning(
                        $"Failed to identify the selected item in the settings window's root navigation view by tag. (Content: {(args.SelectedItem as NavigationViewItem)?.Content}, tag: {(args.SelectedItem as NavigationViewItem)?.Tag})");
                    NavigationViewItemGeneral.IsSelected = true;
                    return;
            } // end switch-case

            FrameRoot.Navigate(pageType, null, new EntranceNavigationTransitionInfo());
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