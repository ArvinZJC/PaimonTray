using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PaimonTray.Helpers;
using Serilog;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

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
        /// Add an account.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="cookies">The cookies.</param>
        private async void AddAccountAsync(string accountId, string cookies)
        {
            var server = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                ? AccountsHelper.TagServerCn
                : AccountsHelper.TagServerGlobal;
            var keyAccount = $"{server}{accountId}";
            var propertySetAccounts = ApplicationData.Current.LocalSettings
                .Containers[AccountsHelper.ContainerKeyAccounts].Values;

            Log.Information($"Start to add the account (account key: {keyAccount}).");

            if (propertySetAccounts.ContainsKey(keyAccount))
            {
                Log.Warning("Already added the account before.");
                ShowContentDialogueLoginFailAsync(_resourceLoader.GetString("AddAccountNoNeed"));
                return;
            } // end if

            var applicationDataCompositeValueAccount = new ApplicationDataCompositeValue
            {
                [AccountsHelper.KeyCookies] = cookies
            };

            propertySetAccounts[keyAccount] = applicationDataCompositeValueAccount;

            var roles = await (Application.Current as App)?.AccHelper.GetRolesAsync(accountId, server)!;
            var isNullRoles = roles == null;

            if (isNullRoles || roles.Count == 0)
            {
                Log.Warning("Failed to add the account. " + (isNullRoles ? "Null roles." : "No role linked."));
                ShowContentDialogueLoginFailAsync(
                    _resourceLoader.GetString(isNullRoles ? "LoginFail" : "AddAccountFail"));
                return;
            } // end if

            // TODO: user selects roles if >= 2 roles; add role directly if 1 role
            if (_isWebView2Available) _webView2LoginWebPage.Close();
        } // end method AddAccountAsync

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

            var isServerCn = comboBoxServerSelectedItem == ComboBoxItemServerCn;

            if (_isWebView2Available)
            {
                var pageSuggestedHeight = isServerCn
                    ? AppConstantsHelper.AddAccountPageLoginWebPageCnHeight
                    : AppConstantsHelper.AddAccountPageLoginWebPageGlobalHeight;
                var pageSuggestedWidth = isServerCn
                    ? AppConstantsHelper.AddAccountPageLoginWebPageCnWidth
                    : AppConstantsHelper.AddAccountPageLoginWebPageGlobalWidth;

                _webView2LoginWebPage.Source = uriLoginMiHoYo;
                ButtonLoginWebPage.Visibility = isServerCn ? Visibility.Collapsed : Visibility.Visible;
                Height = pageMaxHeight < pageSuggestedHeight ? pageMaxHeight : pageSuggestedHeight;
                Width = pageMaxWidth < pageSuggestedWidth ? pageMaxWidth : pageSuggestedWidth;
            }
            else
            {
                Height = pageMaxHeight < AppConstantsHelper.AddAccountPageLoginAlternativeHeight
                    ? pageMaxHeight
                    : AppConstantsHelper.AddAccountPageLoginAlternativeHeight;
                HyperlinkLoginPlace.NavigateUri = uriLoginMiHoYo;
                RunLoginPlace.Text = _resourceLoader.GetString(isServerCn ? "MiHoYo" : "HoYoLab");
                Width = pageMaxWidth < AppConstantsHelper.AddAccountPageLoginAlternativeWidth
                    ? pageMaxWidth
                    : AppConstantsHelper.AddAccountPageLoginAlternativeWidth;
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
            catch (Exception exception)
            {
                Log.Error("Failed to detect WebView2 Runtime.");
                Log.Error(exception.ToString());
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
        /// Log in.
        /// </summary>
        private async void LogInAsync()
        {
            string accountId;
            string cookies;

            if (_isWebView2Available)
            {
                var rawCookies = (await _webView2LoginWebPage.CoreWebView2.CookieManager.GetCookiesAsync(
                    ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                        ? AppConstantsHelper.UrlCookiesMiHoYo
                        : AppConstantsHelper.UrlCookiesHoYoLab)).ToImmutableList();

                (accountId, cookies) = ProcessCookies(ref rawCookies);
            }
            else
            {
                var rawCookies = TextBoxLoginAlternative.Text.Trim().Split(';',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToImmutableList();

                (accountId, cookies) = ProcessCookies(ref rawCookies);
            } // end if...else

            // Execute if valid account ID and cookies.
            if (accountId != string.Empty && cookies.Contains(AppConstantsHelper.CookieNameId) &&
                cookies.Contains(AppConstantsHelper.CookieNameToken))
            {
                AddAccountAsync(accountId, cookies);
                return;
            } // end if

            Log.Warning((_isWebView2Available ? "Web page" : "Alternative") +
                        $" login failed due to invalid cookies: {cookies})");
            ShowContentDialogueLoginFailAsync(_resourceLoader.GetString("LoginFail"));
        } // end method LogInAsync

        /// <summary>
        /// Process the raw cookies.
        /// </summary>
        /// <typeparam name="T">Should be a <see cref="string"/> or <see cref="CoreWebView2Cookie"/> type.</typeparam>
        /// <param name="rawCookies">The raw cookies.</param>
        /// <returns>1st item: the account ID; 2nd item: the processed cookies.</returns>
        private static (string, string) ProcessCookies<T>(ref ImmutableList<T> rawCookies)
        {
            var accountId = string.Empty;
            var cookieName = string.Empty;
            var cookieValue = string.Empty;
            var stringBuilderCookies = new StringBuilder(string.Empty);
            var validCookieNameCount = 0;

            foreach (var cookie in rawCookies)
            {
                switch (cookie)
                {
                    case CoreWebView2Cookie coreWebView2Cookie:
                        cookieName = coreWebView2Cookie.Name;
                        cookieValue = coreWebView2Cookie.Value;
                        break;

                    case string stringCookie:
                        var cookieParts = stringCookie.Split('=',
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        if (cookieParts.Length != 2) continue;

                        cookieName = cookieParts[0];
                        cookieValue = cookieParts[1];
                        break;
                } // end switch-case

                if (cookieName is not (AppConstantsHelper.CookieNameId or AppConstantsHelper.CookieNameToken))
                    continue;

                stringBuilderCookies.Append($"{cookieName}={cookieValue};");
                validCookieNameCount++;

                if (cookieName == AppConstantsHelper.CookieNameId) accountId = cookieValue;

                if (validCookieNameCount == 2) break;
            } // end foreach

            return (accountId, stringBuilderCookies.ToString());
        } // end generic method ProcessCookies

        /// <summary>
        /// Show the login fail content dialogue.
        /// </summary>
        /// <param name="content">The content dialogue's content.</param>
        private async void ShowContentDialogueLoginFailAsync(string content)
        {
            _contentDialogueLoginFail = new ContentDialog
            {
                Content = content,
                CloseButtonText = _resourceLoader.GetString("Ok"),
                RequestedTheme = SettingsHelper.GetTheme(),
                XamlRoot = XamlRoot // It is essential to set the XAML root here to avoid any possible exception.
            };

            await _contentDialogueLoginFail.ShowAsync();
            _contentDialogueLoginFail = null;

            if (_isWebView2Available)
            {
                _webView2LoginWebPage.CoreWebView2.CookieManager.DeleteAllCookies();
                _webView2LoginWebPage.Source = GetLoginWebPageUri();
                return;
            } // end if

            TextBoxLoginAlternative.Text = string.Empty;
        } // end method ShowContentDialogueLoginFailAsync

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
            LogInAsync();
        } // end method ButtonLoginAlternative_OnClick

        // Handle the web page login button's click event.
        private void ButtonLoginWebPage_OnClick(object sender, RoutedEventArgs e)
        {
            LogInAsync();
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

            LogInAsync();
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views