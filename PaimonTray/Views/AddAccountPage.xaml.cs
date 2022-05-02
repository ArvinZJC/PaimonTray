﻿using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PaimonTray.Helpers;
using Serilog;
using System;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Resources;

namespace PaimonTray.Views
{
    /// <summary>
    /// The page for adding an account.
    /// </summary>
    public sealed partial class AddAccountPage
    {
        #region Fields

        private bool _isWebView2Available;
        private WebView2 _webView2LoginWebPage;

        private readonly ResourceLoader _resourceLoader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the page for adding an account.
        /// </summary>
        public AddAccountPage()
        {
            InitializeComponent();
            _resourceLoader = ResourceLoader.GetForViewIndependentUse();
            ChooseLoginMethodAsync();
            UpdateUiText();
        } // end constructor AddAccountPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Apply the server selection.
        /// </summary>
        private void ApplyServerSelection()
        {
            if (ComboBoxServer.SelectedItem == null) return;

            var pageMaxHeight = 0;
            var pageMaxWidth = 0;
            var uriLoginMiHoYo = GetLoginWebPageUri();

            foreach (var existingWindow in WindowsHelper.ExistingWindowList.Where(existingWindow =>
                         existingWindow is MainWindow))
            {
                var workArea = DisplayArea
                    .GetFromWindowId(((MainWindow)existingWindow).WinId, DisplayAreaFallback.Primary).WorkArea;

                pageMaxHeight = workArea.Height - 2 * AppConstantsHelper.WindowMainPositionOffset;
                pageMaxWidth = workArea.Width - 2 * AppConstantsHelper.WindowMainPositionOffset;
                break;
            } // end foreach

            if (_isWebView2Available)
            {
                var pageSuggestedHeight = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                    ? AppConstantsHelper.PageAddAccountLoginWebPageCnHeight
                    : AppConstantsHelper.PageAddAccountLoginWebPageGlobalHeight;
                var pageSuggestedWidth = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                    ? AppConstantsHelper.PageAddAccountLoginWebPageCnWidth
                    : AppConstantsHelper.PageAddAccountLoginWebPageGlobalWidth;

                _webView2LoginWebPage.Source = uriLoginMiHoYo;
                ButtonLoginWebPage.Visibility = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                PageAddAccount.Height = pageMaxHeight < pageSuggestedHeight ? pageMaxHeight : pageSuggestedHeight;
                PageAddAccount.Width = pageMaxWidth < pageSuggestedWidth ? pageMaxWidth : pageSuggestedWidth;
            }
            else
            {
                HyperlinkLoginPlace.NavigateUri = uriLoginMiHoYo;
                PageAddAccount.Height = pageMaxHeight < AppConstantsHelper.PageAddAccountLoginAlternativeHeight
                    ? pageMaxHeight
                    : AppConstantsHelper.PageAddAccountLoginAlternativeHeight;
                PageAddAccount.Width = pageMaxWidth < AppConstantsHelper.PageAddAccountLoginAlternativeWidth
                    ? pageMaxWidth
                    : AppConstantsHelper.PageAddAccountLoginAlternativeWidth;
                RunLoginPlace.Text = _resourceLoader.GetString(
                    ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn ? "MiHoYo" : "HoYoLab");
            } // end if...else
        } // end method ApplyServerSelection

        /// <summary>
        /// Choose the login method automatically.
        /// </summary>
        private async void ChooseLoginMethodAsync()
        {
            try
            {
                Log.Information(
                    $"WebView2 Runtime V{CoreWebView2Environment.GetAvailableBrowserVersionString()} detected.");
                _isWebView2Available = true;
                _webView2LoginWebPage = new WebView2();
                await _webView2LoginWebPage.EnsureCoreWebView2Async();
                _webView2LoginWebPage.CoreWebView2.CookieManager.DeleteAllCookies();
                _webView2LoginWebPage.CoreWebView2.SourceChanged += CoreWebView2LoginWebPage_OnSourceChanged;

                ButtonLoginWebPage.Content = _resourceLoader.GetString("LoginComplete");
                GridLoginWebPage.Children.Add(_webView2LoginWebPage);
                Grid.SetRow(_webView2LoginWebPage, 0);
                TextBlockLogin.Text = _resourceLoader.GetString("LoginWebPage");
                ToolTipService.SetToolTip(ButtonLoginWebPage, _resourceLoader.GetString("LoginHintComplete"));
                ToolTipService.SetToolTip(ButtonLoginWebPageReload, _resourceLoader.GetString("ReloadLoginWebPage"));

                ApplyServerSelection();
            }
            catch (Exception e)
            {
                Log.Error("Failed to detect WebView2 Runtime.");
                Log.Error(e.ToString());
                _isWebView2Available = false;

                ButtonLoginAlternative.Content = _resourceLoader.GetString("Login");
                ButtonLoginWebPageReload.Visibility = Visibility.Collapsed;
                GridLoginAlternative.Visibility = Visibility.Visible;
                GridLoginHintAlternative.Visibility = Visibility.Visible;
                HyperlinkButtonDownloadWebView2Runtime.Content = _resourceLoader.GetString("DownloadWebView2Runtime");
                HyperlinkButtonHowToGetCookies.Visibility = Visibility.Visible;
                RichTextBlockLoginPlace.Visibility = Visibility.Visible;
                TextBlockLogin.Text = _resourceLoader.GetString("Cookies");
                TextBlockLoginHintAlternative.Text = _resourceLoader.GetString("LoginHintAlternative");
                ToolTipService.SetToolTip(HyperlinkButtonHowToGetCookies, _resourceLoader.GetString("HowToGetCookies"));
            } // end try...catch
        } // end method ConfigWebView2LoginAsync

        private async void GetCookiesAsync()
        {
            var stringBuilderCookies = new StringBuilder();

            foreach (var cookie in await _webView2LoginWebPage.CoreWebView2.CookieManager.GetCookiesAsync(
                         ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                             ? AppConstantsHelper.UrlCookiesMiHoYo
                             : AppConstantsHelper.UrlCookiesHoYoLab))
                stringBuilderCookies.Append($"{cookie.Name}={cookie.Value};");

            var cookies = stringBuilderCookies.ToString();

            if (cookies.Contains(AppConstantsHelper.CookieNameEssential))
            {
                _webView2LoginWebPage.Close();
                Log.Information(cookies);
                return;
            } // end if

            Log.Warning("Web page login failed. User is expected to try again.");
            await new ContentDialog
            {
                Content = _resourceLoader.GetString("LoginFail"),
                CloseButtonText = _resourceLoader.GetString("Ok"),
                XamlRoot = Content.XamlRoot
            }.ShowAsync();
        } // end method GetCookiesAsync

        /// <summary>
        /// Get the login web page URI.
        /// </summary>
        /// <returns>The login web page URI.</returns>
        private Uri GetLoginWebPageUri()
        {
            return new Uri(ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                ? AppConstantsHelper.UrlLoginMiHoYo
                : AppConstantsHelper.UrlLoginHoYoLab);
        } // end method GetLoginWebPageUri

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            ComboBoxItemServerCn.Content = _resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = _resourceLoader.GetString("ServerGlobal");
            TextBlockServer.Text = _resourceLoader.GetString("Server");
            TextBlockServerExplanation.Text = _resourceLoader.GetString("ServerExplanation");
            TextBlockTitle.Text = _resourceLoader.GetString("AddAccount");
        } // end method UpdateUiText

        #endregion Methods

        #region Event Handlers

        // Handle the unloaded event of the page for adding an account.
        private void AddAccountPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isWebView2Available) _webView2LoginWebPage.Close();
        } // end method AddAccountPage_OnUnloaded

        // Handle the alternative login button's click event.
        private void ButtonLoginAlternative_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        } // end method ButtonLoginAlternative_OnClick

        // Handle the web page login button's click event.
        private void ButtonLoginWebPage_OnClick(object sender, RoutedEventArgs e)
        {
            GetCookiesAsync();
        } // end method ButtonLoginWebPage_OnClick

        // Handle the click event of the button for reloading the login web page.
        private void ButtonLoginWebPageReload_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isWebView2Available) _webView2LoginWebPage.Source = GetLoginWebPageUri();
        } // end method ButtonLoginWebPageReload_OnClick

        // Handle the server combo box's selection changed event.
        private void ComboBoxServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyServerSelection();
        } // end method ComboBoxServer_OnSelectionChanged

        // Handle the web page login CoreWebView2's source changed event.
        private void CoreWebView2LoginWebPage_OnSourceChanged(CoreWebView2 sender,
            CoreWebView2SourceChangedEventArgs args)
        {
            if ((ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn &&
                 !_webView2LoginWebPage.Source.ToString().Contains(AppConstantsHelper.UrlLoginEndMiHoYo)) ||
                ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerGlobal) return;

            GetCookiesAsync();
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views