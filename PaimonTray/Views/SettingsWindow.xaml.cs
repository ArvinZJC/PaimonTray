﻿using Microsoft.UI;
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

        // Customise the window.
        private async void CustomiseWindowAsync()
        {
            var windowId = WindowsHelper.GetWindowId(this);
            var appWin = WindowsHelper.GetAppWindow(windowId);

            Title = $"Settings - {Package.Current.DisplayName}";

            if (appWin == null)
            {
                Log.Warning("The settings window's AppWindow is null.");
                return;
            } // end if

            appWin.SetIcon((await StorageFile.GetFileFromApplicationUriAsync(
                new Uri(AppConstantsHelper.AppIconPath))).Path);

            // Customise the title bar.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                appWin.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                appWin.TitleBar.ButtonHoverBackgroundColor =
                    (Application.Current.Resources["WindowCaptionButtonBackgroundPointerOver"] as SolidColorBrush)
                    ?.Color;
                appWin.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                appWin.TitleBar.ExtendsContentIntoTitleBar = true;
                GridColumnTitleBarLeftPadding.Width = new GridLength(appWin.TitleBar.LeftInset);
                GridColumnTitleBarRightPadding.Width = new GridLength(appWin.TitleBar.RightInset);
                GridTitleBar.Height = appWin.TitleBar.Height;
            }
            else
            {
                ExtendsContentIntoTitleBar = true;
                SetTitleBar(GridTitleBar);
            } // end if...else

            var workArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary).WorkArea;

            appWin.Move(new PointInt32((workArea.Width - appWin.Size.Width) / 2,
                (workArea.Height - appWin.Size.Height) / 2));
        } // end method CustomiseWindowAsync

        #endregion Methods

        #region Event Handlers

        // Handle the root navigation view's display mode changed event.
        private void NavigationViewRoot_OnDisplayModeChanged(NavigationView sender,
            NavigationViewDisplayModeChangedEventArgs args)
        {
            NavigationViewRoot.IsPaneToggleButtonVisible = args.DisplayMode != NavigationViewDisplayMode.Expanded;
            ScrollViewerRoot.Padding = args.DisplayMode == NavigationViewDisplayMode.Minimal
                ? new Thickness(18, 18, 18, 0)
                : new Thickness(56, 18, 56, 0);
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

            switch ((args.SelectedItem as NavigationViewItem)?.Tag)
            {
                case AppConstantsHelper.NavigationViewItemTagAboutApp:
                    pageType = typeof(AboutAppPage);
                    break;

                case AppConstantsHelper.NavigationViewItemTagAccountsSettings:
                    pageType = typeof(AccountsSettingsPage);
                    break;

                case AppConstantsHelper.NavigationViewItemTagGeneralSettings:
                case "":
                    pageType = typeof(GeneralSettingsPage);
                    break;

                default:
                    Log.Warning(
                        $"Failed to identify the selected item in the settings window's root navigation view by tag. (Content: {(args.SelectedItem as NavigationViewItem)?.Content}, tag: {(args.SelectedItem as NavigationViewItem)?.Tag})");
                    NavigationViewItemGeneral.IsSelected = true;
                    return;
            } // end switch-case

            FrameRoot.Navigate(pageType, null, new EntranceNavigationTransitionInfo());
            NavigationViewRoot.Header = (args.SelectedItem as NavigationViewItem)?.Content;
        } // end method NavigationViewRoot_OnSelectionChanged

        // Handle the window's activated event.
        private void SettingsWindow_OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
                TextBlockWindowTitle.Foreground =
                    Application.Current.Resources["WindowCaptionForegroundDisabled"] as SolidColorBrush;
            else
                TextBlockWindowTitle.Foreground =
                    Application.Current.Resources["WindowCaptionForeground"] as SolidColorBrush;
        } // end method SettingsWindow_OnActivated

        #endregion Event Handlers
    } // end class SettingsWindow
} // end namespace PaimonTray.Views