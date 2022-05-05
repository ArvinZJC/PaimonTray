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

        private ContentDialog _contentDialogueLoginFail;
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
            _resourceLoader = ResourceLoader.GetForViewIndependentUse();
            InitializeComponent();
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
            var comboBoxServerSelectedItem = ComboBoxServer.SelectedItem as ComboBoxItem;

            if (comboBoxServerSelectedItem == null) return;

            var pageMaxHeight = 0;
            var pageMaxWidth = 0;
            var uriLoginMiHoYo = GetLoginWebPageUri();

            foreach (var existingWindow in WindowsHelper.ExistingWindowList.Where(existingWindow =>
                         existingWindow is MainWindow))
            {
                var workArea = DisplayArea
                    .GetFromWindowId(((MainWindow)existingWindow).WinId, DisplayAreaFallback.Primary).WorkArea;

                pageMaxHeight = workArea.Height - 2 * AppConstantsHelper.MainWindowPositionOffset;
                pageMaxWidth = workArea.Width - 2 * AppConstantsHelper.MainWindowPositionOffset;
                break;
            } // end foreach

            if (_isWebView2Available)
            {
                var pageSuggestedHeight = comboBoxServerSelectedItem == ComboBoxItemServerCn
                    ? AppConstantsHelper.AddAccountPageLoginWebPageCnHeight
                    : AppConstantsHelper.AddAccountPageLoginWebPageGlobalHeight;
                var pageSuggestedWidth = comboBoxServerSelectedItem == ComboBoxItemServerCn
                    ? AppConstantsHelper.AddAccountPageLoginWebPageCnWidth
                    : AppConstantsHelper.AddAccountPageLoginWebPageGlobalWidth;

                _webView2LoginWebPage.Source = uriLoginMiHoYo;
                ButtonLoginWebPage.Visibility = comboBoxServerSelectedItem == ComboBoxItemServerCn
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                PageAddAccount.Height = pageMaxHeight < pageSuggestedHeight ? pageMaxHeight : pageSuggestedHeight;
                PageAddAccount.Width = pageMaxWidth < pageSuggestedWidth ? pageMaxWidth : pageSuggestedWidth;
            }
            else
            {
                HyperlinkLoginPlace.NavigateUri = uriLoginMiHoYo;
                PageAddAccount.Height = pageMaxHeight < AppConstantsHelper.AddAccountPageLoginAlternativeHeight
                    ? pageMaxHeight
                    : AppConstantsHelper.AddAccountPageLoginAlternativeHeight;
                PageAddAccount.Width = pageMaxWidth < AppConstantsHelper.AddAccountPageLoginAlternativeWidth
                    ? pageMaxWidth
                    : AppConstantsHelper.AddAccountPageLoginAlternativeWidth;
                RunLoginPlace.Text =
                    _resourceLoader.GetString(comboBoxServerSelectedItem == ComboBoxItemServerCn
                        ? "MiHoYo"
                        : "HoYoLab");
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

        // Get the login cookies.
        private async void GetCookiesAsync()
        {
            string cookies;

            if (_isWebView2Available)
            {
                var stringBuilderCookies = new StringBuilder(string.Empty);

                foreach (var cookie in await _webView2LoginWebPage.CoreWebView2.CookieManager.GetCookiesAsync(
                             ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                                 ? AppConstantsHelper.UrlCookiesMiHoYo
                                 : AppConstantsHelper.UrlCookiesHoYoLab))
                    if (cookie.Name is AppConstantsHelper.CookieNameId or AppConstantsHelper.CookieNameToken)
                        stringBuilderCookies.Append($"{cookie.Name}={cookie.Value};");

                cookies = stringBuilderCookies.ToString();
            }
            else
                cookies = TextBoxLoginAlternative.Text;


            if (cookies.Contains(AppConstantsHelper.CookieNameId) &&
                cookies.Contains(AppConstantsHelper.CookieNameToken))
            {
                if (_isWebView2Available) _webView2LoginWebPage.Close();

                Log.Information(cookies); // TODO
                return;
            } // end if

            Log.Warning((_isWebView2Available ? "Web page" : "Alternative") +
                        $" login failed (cookies: {cookies}). User is expected to try again.");

            _contentDialogueLoginFail = new ContentDialog
            {
                Content = _resourceLoader.GetString("LoginFail"),
                CloseButtonText = _resourceLoader.GetString("Ok"),
                RequestedTheme = SettingsHelper.GetTheme(),
                XamlRoot = Content.XamlRoot
            };

            await _contentDialogueLoginFail.ShowAsync();
            _contentDialogueLoginFail = null;

            if (!_isWebView2Available) return;

            _webView2LoginWebPage.CoreWebView2.CookieManager.DeleteAllCookies();
            _webView2LoginWebPage.Source = GetLoginWebPageUri();
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

        // Handle the actual theme changed event of the page for adding an account.
        private void AddAccountPage_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (_contentDialogueLoginFail == null) return;

            _contentDialogueLoginFail.RequestedTheme = SettingsHelper.GetTheme();
        } // end method AddAccountPage_OnActualThemeChanged

        // Handle the unloaded event of the page for adding an account.
        private void AddAccountPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isWebView2Available) _webView2LoginWebPage.Close();
        } // end method AddAccountPage_OnUnloaded

        // Handle the alternative login button's click event.
        private void ButtonLoginAlternative_OnClick(object sender, RoutedEventArgs e)
        {
            GetCookiesAsync();
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
            var comboBoxServerSelectedItem = ComboBoxServer.SelectedItem as ComboBoxItem;

            if ((comboBoxServerSelectedItem == ComboBoxItemServerCn && !_webView2LoginWebPage.Source.ToString()
                    .Contains(AppConstantsHelper.UrlLoginEndMiHoYo)) ||
                comboBoxServerSelectedItem == ComboBoxItemServerGlobal) return;

            GetCookiesAsync();
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views