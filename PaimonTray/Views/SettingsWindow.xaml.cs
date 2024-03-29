﻿using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using PaimonTray.Helpers;
using PaimonTray.Models;
using Serilog;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics;

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
            _app = Application.Current as App;
            InitializeComponent();
            _ = CustomiseWindowAsync();
            UpdateUiText();
        } // end constructor SettingsWindow

        #endregion Constructors

        #region Event Handlers

        // Handle the root grid's actual theme changed event.
        private void GridRoot_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            CustomiseTitleBar();
            SetRootGridBackground();
        } // end method GridRoot_OnActualThemeChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            _existingWindow = _app.WindowsH.GetExistingSettingsWindow();
            SetRootGridBackground();
            Activate(); // Activate when ready.
        } // end method GridRoot_OnLoaded

        // Handle the body navigation view's display mode changed event.
        private void NavigationViewBody_OnDisplayModeChanged(NavigationView sender,
            NavigationViewDisplayModeChangedEventArgs args)
        {
            NavigationViewBody.IsPaneToggleButtonVisible =
                NavigationViewBody.DisplayMode is not NavigationViewDisplayMode.Expanded;
            ScrollViewerBody.Padding = NavigationViewBody.DisplayMode is NavigationViewDisplayMode.Minimal
                ? new Thickness(18, 18, 18, 0)
                : new Thickness(56, 18, 56, 0);
        } // end method NavigationViewBody_OnDisplayModeChanged

        // Handle the body navigation view's selection changed event.
        private void NavigationViewBody_OnSelectionChanged(NavigationView sender,
            NavigationViewSelectionChangedEventArgs args)
        {
            var navigationViewBodySelectedItem = NavigationViewBody.SelectedItem as NavigationViewItem;
            Type pageType;

            if (navigationViewBodySelectedItem == NavigationViewItemBodyAbout) pageType = typeof(AboutAppPage);
            else if (navigationViewBodySelectedItem == NavigationViewItemBodyAccounts)
                pageType = typeof(AccountsSettingsPage);
            else pageType = typeof(GeneralSettingsPage);

            FrameBody.Navigate(pageType, null, new EntranceNavigationTransitionInfo());
            NavigationViewBody.Header = navigationViewBodySelectedItem?.Content;
        } // end method NavigationViewBody_OnSelectionChanged

        // Handle the settings window's activated event.
        private void SettingsWindow_OnActivated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState is WindowActivationState.Deactivated)
            {
                GridTitleBar.Resources.TryGetValue("TitleBarCaptionForegroundDisabled",
                    out var titleBarCaptionForegroundDisabled);
                TextBlockWindowTitle.Foreground = titleBarCaptionForegroundDisabled as SolidColorBrush;
            }
            else
            {
                GridTitleBar.Resources.TryGetValue("TitleBarCaptionForeground", out var titleBarCaptionForeground);
                TextBlockWindowTitle.Foreground = titleBarCaptionForeground as SolidColorBrush;
            } // end if...else
        } // end method SettingsWindow_OnActivated

        // Handle the settings window's closed event.
        private void SettingsWindow_OnClosed(object sender, WindowEventArgs args)
        {
            _app = null;
            _appWindow = null;
            _existingWindow = null;
        } // end method SettingsWindow_OnClosed

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// The app window.
        /// </summary>
        private AppWindow _appWindow;

        /// <summary>
        /// The existing window.
        /// </summary>
        private ExistingWindow _existingWindow;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Customise the title bar.
        /// </summary>
        private void CustomiseTitleBar()
        {
            GridTitleBar.Resources.TryGetValue("TitleBarCaptionForeground", out var titleBarCaptionForegroundObject);
            GridTitleBar.Resources.TryGetValue("TitleBarButtonBackgroundHover", out var titleBarButtonBackgroundHover);
            GridTitleBar.Resources.TryGetValue("TitleBarButtonBackgroundPressed",
                out var titleBarButtonBackgroundPressed);
            GridTitleBar.Resources.TryGetValue("TitleBarCaptionForegroundDisabled",
                out var titleBarCaptionForegroundDisabled);

            var titleBar = _appWindow.TitleBar;
            var titleBarCaptionForeground = titleBarCaptionForegroundObject as SolidColorBrush;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = titleBarCaptionForeground?.Color;
            titleBar.ButtonHoverBackgroundColor = (titleBarButtonBackgroundHover as SolidColorBrush)?.Color;
            titleBar.ButtonHoverForegroundColor = titleBarCaptionForeground?.Color;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = GridTitleBar.ActualTheme switch
            {
                ElementTheme.Dark => Colors.DimGray,
                ElementTheme.Default => null,
                ElementTheme.Light => Colors.Silver,
                _ => null
            }; // Must set the colour explicitly rather than using the resource for the title bar caption's disabled foreground to make it take effect.
            titleBar.ButtonPressedBackgroundColor = (titleBarButtonBackgroundPressed as SolidColorBrush)?.Color;
            titleBar.ButtonPressedForegroundColor = (titleBarCaptionForegroundDisabled as SolidColorBrush)?.Color;
            titleBar.ExtendsContentIntoTitleBar =
                true; // Need to extend the content into the title bar before getting the title bar's height.

            GridColumnTitleBarLeftPadding.Width = new GridLength(titleBar.LeftInset);
            GridColumnTitleBarRightPadding.Width = new GridLength(titleBar.RightInset);
            GridTitleBar.Height = titleBar.Height;
        } // end method CustomiseTitleBar

        /// <summary>
        /// Customise the window.
        /// </summary>
        /// <returns>Void.</returns>
        private async Task CustomiseWindowAsync()
        {
            var windowId = WindowsHelper.GetWindowId(this);

            _appWindow = await _app.WindowsH.GetAppWindowWithIconAsync(windowId);

            if (_appWindow is null)
            {
                Log.Warning("The settings window's app window is null.");
                return;
            } // end if

            CustomiseTitleBar();

            var workArea = WindowsHelper.GetWorkArea(windowId);

            _appWindow.Move(new PointInt32((workArea.Width - _appWindow.Size.Width) / 2,
                (workArea.Height - _appWindow.Size.Height) / 2));
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
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            NavigationViewItemBodyAbout.Content = resourceLoader?.GetString("About");
            NavigationViewItemBodyAccounts.Content = resourceLoader?.GetString("Accounts");
            NavigationViewItemBodyGeneral.Content = resourceLoader?.GetString("General");
            TextBlockWindowTitle.Text = resourceLoader?.GetString("Settings");
            Title = $"{resourceLoader?.GetString("Settings")} - {Package.Current.DisplayName}";
        } // end method UpdateUiText

        #endregion Methods
    } // end class SettingsWindow
} // end namespace PaimonTray.Views