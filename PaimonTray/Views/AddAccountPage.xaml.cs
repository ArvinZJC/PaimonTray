using Microsoft.UI.Windowing;
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
        private WebView2 _webView2Login;

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
            var uriLoginMiHoYo = new Uri(ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                ? AppConstantsHelper.UrlLoginMiHoYo
                : AppConstantsHelper.UrlLoginHoYoLab);

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
                _webView2Login.Source = uriLoginMiHoYo;
                PageAddAccount.Height = pageMaxHeight < AppConstantsHelper.PageAddAccountLoginWebPageSideLength
                    ? pageMaxHeight
                    : AppConstantsHelper.PageAddAccountLoginWebPageSideLength;
                PageAddAccount.Width = pageMaxWidth < AppConstantsHelper.PageAddAccountLoginWebPageSideLength
                    ? pageMaxWidth
                    : AppConstantsHelper.PageAddAccountLoginWebPageSideLength;
            }
            else
            {
                HyperlinkLoginPlace.NavigateUri = uriLoginMiHoYo;
                PageAddAccount.Height = pageMaxHeight < AppConstantsHelper.PageAddAccountLoginAlternativeSideLength
                    ? pageMaxHeight
                    : AppConstantsHelper.PageAddAccountLoginAlternativeSideLength;
                PageAddAccount.Width = pageMaxWidth < AppConstantsHelper.PageAddAccountLoginAlternativeSideLength
                    ? pageMaxWidth
                    : AppConstantsHelper.PageAddAccountLoginAlternativeSideLength;
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
                _webView2Login = new WebView2();
                await _webView2Login.EnsureCoreWebView2Async();
                _webView2Login.CoreWebView2.SourceChanged += CoreWebView2Login_OnSourceChanged;

                GridLogin.Children.Add(_webView2Login);
                Grid.SetRow(_webView2Login, 1);
                TextBlockLogin.Text = _resourceLoader.GetString("LoginWebPage");

                ApplyServerSelection();
            }
            catch (Exception e)
            {
                Log.Error("Failed to detect WebView2 Runtime.");
                Log.Error(e.ToString());
                _isWebView2Available = false;

                ButtonLogin.Content = _resourceLoader.GetString("Login");
                GridCookies.Visibility = Visibility.Visible;
                GridLoginHintAlternative.Visibility = Visibility.Visible;
                HyperlinkButtonDownloadWebView2Runtime.Content = _resourceLoader.GetString("DownloadWebView2Runtime");
                HyperlinkButtonHowToGetCookies.Visibility = Visibility.Visible;
                RichTextBlockLoginPlace.Visibility = Visibility.Visible;
                TextBlockLogin.Text = _resourceLoader.GetString("Cookies");
                TextBlockLoginHintAlternative.Text = _resourceLoader.GetString("LoginHintAlternative");
                ToolTipService.SetToolTip(HyperlinkButtonHowToGetCookies, _resourceLoader.GetString("HowToGetCookies"));
            } // end try...catch
        } // end method ConfigWebView2LoginAsync

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
            if (_isWebView2Available) _webView2Login.Close();
        } // end method AddAccountPage_OnUnloaded

        // Handle the login button's click event.
        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        } // end method ButtonLogin_OnClick

        // Handle the server combo box's selection changed event.
        private void ComboBoxServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyServerSelection();
        } // end method ComboBoxServer_OnSelectionChanged

        // Handle the login CoreWebView2's source changed event.
        private async void CoreWebView2Login_OnSourceChanged(CoreWebView2 sender,
            CoreWebView2SourceChangedEventArgs args)
        {
            if (!_webView2Login.Source.ToString()
                    .Contains(ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                        ? AppConstantsHelper.UrlLoginEndMiHoYo
                        : AppConstantsHelper.UrlLoginEndHoYoLab)) return;

            var cookies = new StringBuilder();

            foreach (var cookie in await _webView2Login.CoreWebView2.CookieManager.GetCookiesAsync(
                         ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                             ? AppConstantsHelper.UrlCookiesMiHoYo
                             : AppConstantsHelper.UrlCookiesHoYoLab))
                cookies.Append($"{cookie.Name}={cookie.Value};");

            _webView2Login.Close();
            Log.Information(cookies.ToString());
        } // end method CoreWebView2Login_OnSourceChanged

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views