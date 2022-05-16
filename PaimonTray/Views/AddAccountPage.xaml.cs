using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using PaimonTray.Helpers;
using Serilog;
using System;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
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

        private ContentDialog _contentDialogue;
        private bool _isWebView2Available;
        private WebView2 _webView2LoginWebPage;

        private readonly App _app;
        private readonly ResourceLoader _resourceLoader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the page for adding an account.
        /// </summary>
        public AddAccountPage()
        {
            _app = Application.Current as App;
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
        /// <returns>A <see cref="Task"/> object just to indicate that any later operation needs to wait.</returns>
        private async Task AddAccountAsync(string accountId, string cookies)
        {
            var applicationDataContainerAccounts =
                ApplicationData.Current.LocalSettings.CreateContainer(AccountsHelper.ContainerKeyAccounts,
                    ApplicationDataCreateDisposition.Always);
            var server = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                ? AccountsHelper.TagServerCn
                : AccountsHelper.TagServerGlobal;
            var containerKeyAccount = $"{server}{accountId}";
            var shouldUpdateAccount =
                applicationDataContainerAccounts.Containers
                    .ContainsKey(containerKeyAccount); // A flag indicating if the account should be updated or added.
            var propertySetAccount = applicationDataContainerAccounts
                .CreateContainer(containerKeyAccount, ApplicationDataCreateDisposition.Always)
                .Values; // Need to declare after the flag indicating if the account should be updated or added.

            propertySetAccount[AccountsHelper.KeyCookies] = cookies;
            propertySetAccount[AccountsHelper.KeyId] = accountId;
            propertySetAccount[AccountsHelper.KeyIsEnabled] = false;
            propertySetAccount[AccountsHelper.KeyServer] = server;

            var characters = await _app.AccHelper.GetCharactersFromApiAsync(containerKeyAccount)!;

            InitialiseLogin();

            if (characters == null)
            {
                Log.Warning(
                    $"Failed to add the account due to null characters (account container key: {containerKeyAccount}).");
                ShowLoginMessage(_resourceLoader.GetString("LoginFail"), InfoBarSeverity.Error);

                if (!shouldUpdateAccount) applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);

                return;
            } // end if

            if (shouldUpdateAccount)
            {
                _app.AccHelper.StoreCharacters(characters, containerKeyAccount);
                ShowLoginMessage(_resourceLoader.GetString("MessageUpdateAccount"));
                return;
            } // end if

            if (characters.Count == 0)
            {
                _contentDialogue = new ContentDialog
                {
                    Content = _resourceLoader.GetString("AddAccountNoCharacter"),
                    CloseButtonText = _resourceLoader.GetString("No"),
                    PrimaryButtonText = _resourceLoader.GetString("Yes"),
                    RequestedTheme = SettingsHelper.GetTheme(),
                    Title = _resourceLoader.GetString("AddAccountConfirmation"),
                    XamlRoot = XamlRoot // It is essential to set the XAML root here to avoid any possible exception.
                };

                var contentDialogResult = await _contentDialogue.ShowAsync();

                _contentDialogue = null;

                if (contentDialogResult != ContentDialogResult.Primary)
                {
                    applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);
                    return;
                } // end if

                ShowLoginMessage(_resourceLoader.GetString("AddAccountNoCharacterSuccess"), InfoBarSeverity.Success);
            } // end if

            _app.AccHelper.StoreCharacters(characters, containerKeyAccount);
        } // end method AddAccountAsync

        /// <summary>
        /// Apply the server selection.
        /// </summary>
        private void ApplyServerSelection()
        {
            var comboBoxServerSelectedItem = ComboBoxServer.SelectedItem as ComboBoxItem;

            if (comboBoxServerSelectedItem == null) return;

            var uriLoginMiHoYo = GetLoginWebPageUri();
            var workArea = DisplayArea
                .GetFromWindowId(WindowsHelper.ShowMainWindow().WinId, DisplayAreaFallback.Primary).WorkArea;
            var pageMaxHeight = workArea.Height - 2 * AppConstantsHelper.MainWindowPositionOffset;
            var pageMaxWidth = workArea.Width - 2 * AppConstantsHelper.MainWindowPositionOffset;

            CheckAccountsCount();

            var isServerCn = comboBoxServerSelectedItem == ComboBoxItemServerCn;

            if (_isWebView2Available)
            {
                var pageSuggestedHeight = isServerCn
                    ? AppConstantsHelper.AddAccountPageLoginWebPageServerCnHeight
                    : AppConstantsHelper.AddAccountPageLoginWebPageServerGlobalHeight;
                var pageSuggestedWidth = isServerCn
                    ? AppConstantsHelper.AddAccountPageLoginWebPageServerCnWidth
                    : AppConstantsHelper.AddAccountPageLoginWebPageServerGlobalWidth;

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

        private bool CheckAccountsCount()
        {
            var hasReachedLimit = _app.AccHelper.CountAccounts() >= AccountsHelper.CountAccountsMax;

            if (hasReachedLimit)
                ShowLoginMessage(_resourceLoader.GetString("AddAccountReachLimit"), InfoBarSeverity.Error);
            else
                InfoBarMessageLogin.IsOpen = false;

            return hasReachedLimit;
        } // end method CheckAccountsCount

        /// <summary>
        /// Choose the login method automatically.
        /// </summary>
        private async void ChooseLoginMethodAsync()
        {
            var propertySetSettings = ApplicationData.Current.LocalSettings
                .Containers[SettingsHelper.ContainerKeySettings].Values;

            if ((bool)propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways])
                UseAlternativeLoginMethod(true);
            else
                try
                {
                    Log.Information(
                        $"WebView2 Runtime V{CoreWebView2Environment.GetAvailableBrowserVersionString()} detected.");
                    _isWebView2Available = true;
                    _webView2LoginWebPage = new WebView2();
                    await _webView2LoginWebPage.EnsureCoreWebView2Async();
                    _webView2LoginWebPage.CoreWebView2.CookieManager.DeleteAllCookies();
                    _webView2LoginWebPage.CoreWebView2.SourceChanged += CoreWebView2LoginWebPage_OnSourceChanged;
                    _webView2LoginWebPage.NavigationCompleted += WebView2LoginWebPage_OnNavigationCompleted;

                    ButtonLoginWebPage.Content = _resourceLoader.GetString("LoginComplete");
                    GridLoginWebPage.Children.Add(_webView2LoginWebPage);
                    Grid.SetRow(_webView2LoginWebPage, 0);
                    TextBlockLogin.Text = _resourceLoader.GetString("LoginWebPage");
                    ToolTipService.SetToolTip(ButtonLoginWebPage, _resourceLoader.GetString("LoginCompleteTooltip"));
                    ToolTipService.SetToolTip(ButtonLoginWebPageHome, _resourceLoader.GetString("LoginWebPageHome"));
                }
                catch (Exception exception)
                {
                    Log.Error("Failed to detect WebView2 Runtime.");
                    Log.Error(exception.ToString());
                    UseAlternativeLoginMethod();
                } // end try...catch

            ComboBoxServer.SelectedItem =
                propertySetSettings[SettingsHelper.KeyServerDefault] as string == AccountsHelper.TagServerCn
                    ? ComboBoxItemServerCn
                    : ComboBoxItemServerGlobal;
        } // end method ChooseLoginMethodAsync

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
        /// Initialise the login.
        /// </summary>
        private void InitialiseLogin()
        {
            if (_isWebView2Available)
            {
                _webView2LoginWebPage.CoreWebView2.CookieManager.DeleteAllCookies();
                _webView2LoginWebPage.Source = GetLoginWebPageUri();
                return;
            } // end if

            TextBoxLoginAlternative.Text = string.Empty;
        } // end method InitialiseLogin

        /// <summary>
        /// Log in.
        /// </summary>
        private async void LogInAsync()
        {
            GridAddingAccount.Visibility = Visibility.Visible;

            if (!CheckAccountsCount())
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
                if (accountId != string.Empty && cookies.Contains(AccountsHelper.CookieKeyUserId) &&
                    cookies.Contains(AccountsHelper.CookieKeyToken))
                    await AddAccountAsync(accountId, cookies);
                else
                {
                    Log.Warning((_isWebView2Available ? "Web page" : "Alternative") +
                                $" login failed due to invalid cookies ({cookies}).");
                    InitialiseLogin();
                    ShowLoginMessage(_resourceLoader.GetString("LoginFail"), InfoBarSeverity.Error);
                } // end if...else
            } // end if

            GridAddingAccount.Visibility = Visibility.Collapsed;
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

                if (cookieName is not (AccountsHelper.CookieKeyUserId or AccountsHelper.CookieKeyToken)) continue;

                stringBuilderCookies.Append($"{cookieName}={cookieValue};");
                validCookieNameCount++;

                if (cookieName == AccountsHelper.CookieKeyUserId) accountId = cookieValue;

                if (validCookieNameCount == 2) break;
            } // end foreach

            return (accountId, stringBuilderCookies.ToString());
        } // end generic method ProcessCookies

        /// <summary>
        /// Show the alternative login message.
        /// </summary>
        private void ShowAlternativeLoginMessage()
        {
            HyperlinkButtonDownloadWebView2Runtime.Content = _resourceLoader.GetString("DownloadWebView2Runtime");
            InfoBarMessageLoginAlternative.Margin = new Thickness(0, 0, 0, 8);
            InfoBarMessageLoginAlternative.Message = _resourceLoader.GetString("MessageLoginAlternative");
            InfoBarMessageLoginAlternative.IsOpen = true; // Show the info bar when ready.
        } // end method ShowAlternativeLoginMessage

        /// <summary>
        /// Show the login message.
        /// </summary>
        /// <param name="message">The login message.</param>
        /// <param name="infoBarSeverity">The info bar's severity.</param>
        private void ShowLoginMessage(string message, InfoBarSeverity infoBarSeverity = InfoBarSeverity.Informational)
        {
            InfoBarMessageLogin.Margin = new Thickness(0, 0, 0, 8);
            InfoBarMessageLogin.Message = message;
            InfoBarMessageLogin.Severity = infoBarSeverity;
            InfoBarMessageLogin.IsOpen = true; // Show the info bar when ready.
        } // end method ShowLoginMessage

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            ComboBoxItemServerCn.Content = _resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = _resourceLoader.GetString("ServerGlobal");
            TextBlockAddingAccount.Text = _resourceLoader.GetString("AddingAccount");
            TextBlockServer.Text = _resourceLoader.GetString("Server");
            TextBlockServerExplanation.Text = _resourceLoader.GetString("ServerExplanation");
            TextBlockTitle.Text = _resourceLoader.GetString("AddAccount");
        } // end method UpdateUiText

        /// <summary>
        /// Use the alternative login method.
        /// </summary>
        /// <param name="isAlways">A flag indicating if the alternative login method is always used.</param>
        private void UseAlternativeLoginMethod(bool isAlways = false)
        {
            _isWebView2Available = false;

            if (!isAlways) ShowAlternativeLoginMessage();

            ButtonLoginAlternative.Content = _resourceLoader.GetString("Login");
            ButtonLoginWebPageHome.Visibility = Visibility.Collapsed;
            GridLoginAlternative.Visibility = Visibility.Visible;
            HyperlinkButtonHowToGetCookies.Visibility = Visibility.Visible;
            TextBlockLogin.Text = _resourceLoader.GetString("Cookies");
            TextBlockLoginPlace.Visibility = Visibility.Visible;
            ToolTipService.SetToolTip(HyperlinkButtonHowToGetCookies, _resourceLoader.GetString("HowToGetCookies"));
        } // end method UseAlternativeLoginMethod

        #endregion Methods

        #region Event Handlers

        // Handle the actual theme changed event of the page for adding an account.
        private void AddAccountPage_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (_contentDialogue != null) _contentDialogue.RequestedTheme = SettingsHelper.GetTheme();
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

        // Handle the click event of the button for going to the login web page.
        private void ButtonLoginWebPageHome_OnClick(object sender, RoutedEventArgs e)
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
            if (ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn && _webView2LoginWebPage.Source
                    .ToString().Contains(AppConstantsHelper.UrlLoginEndMiHoYo))
                GridAddingAccount.Visibility = Visibility.Visible;
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

#pragma warning disable CA1822 // Mark members as static
        // Handle the info bar's closing event.
        private void InfoBar_OnClosing(InfoBar sender, InfoBarClosingEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            sender.Margin = new Thickness(0);
        } // end method InfoBar_OnClosing

        // Handle the web page login WebView2's navigation completed event.
        private void WebView2LoginWebPage_OnNavigationCompleted(WebView2 sender,
            CoreWebView2NavigationCompletedEventArgs args)
        {
            switch (ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn)
            {
                // Although the CoreWebView2's source changed event uses the same condition, this event is to ensure cookies.
                case true when _webView2LoginWebPage.Source.ToString().Contains(AppConstantsHelper.UrlLoginEndMiHoYo):
                    LogInAsync();
                    break;

                case false:
                    ButtonLoginWebPage.IsEnabled = true;
                    break;
            } // end switch-case
        } // end method WebView2LoginWebPage_OnNavigationCompleted

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views