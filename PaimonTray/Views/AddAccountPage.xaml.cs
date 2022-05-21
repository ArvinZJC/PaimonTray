using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
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
        private readonly MainWindow _mainWindow;
        private readonly ResourceLoader _resourceLoader;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the page for adding an account.
        /// </summary>
        public AddAccountPage()
        {
            _app = Application.Current as App;
            _mainWindow = WindowsHelper.ShowMainWindow();
            _resourceLoader = ResourceLoader.GetForViewIndependentUse();
            InitializeComponent();
            ChooseLoginMethodAsync();
            UpdateUiText();
        } // end constructor AddAccountPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Add or update an account.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="cookies">The cookies.</param>
        /// <returns>A <see cref="Task"/> object just to indicate that any later operation needs to wait.</returns>
        private async Task AddOrUpdateAccountAsync(string accountId, string cookies)
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

            TextBlockBusyIndicator.Text =
                _resourceLoader.GetString(shouldUpdateAccount ? "AccountUpdating" : "AccountAdding");

            var propertySetAccount = applicationDataContainerAccounts
                .CreateContainer(containerKeyAccount, ApplicationDataCreateDisposition.Always)
                .Values; // Need to declare after the flag indicating if the account should be updated or added.

            propertySetAccount[AccountsHelper.KeyCookies] = cookies;
            propertySetAccount[AccountsHelper.KeyId] = accountId;
            propertySetAccount[AccountsHelper.KeyServer] = server;
            propertySetAccount[AccountsHelper.KeyStatus] = shouldUpdateAccount
                ? AccountsHelper.TagStatusUpdating
                : AccountsHelper.TagStatusAdding;

            var characters = await _app.AccHelper.GetCharactersFromApiAsync(containerKeyAccount)!;

            InitialiseLogin();

            if (characters == null)
            {
                Log.Warning(
                    $"Failed to add the account due to null characters (account container key: {containerKeyAccount}).");
                ShowInfoBarLogin(_resourceLoader.GetString("LoginFail"), InfoBarSeverity.Error);

                if (!shouldUpdateAccount) applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);

                return;
            } // end if

            if (shouldUpdateAccount)
            {
                _app.AccHelper.StoreCharacters(characters, containerKeyAccount);
                ShowInfoBarLogin(_resourceLoader.GetString("AccountUpdated"));
                return;
            } // end if

            if (characters.Count == 0)
            {
                _contentDialogue = new ContentDialog
                {
                    Content = _resourceLoader.GetString("AccountAddNoCharacter"),
                    CloseButtonText = _resourceLoader.GetString("No"),
                    DefaultButton = ContentDialogButton.Close,
                    PrimaryButtonText = _resourceLoader.GetString("Yes"),
                    RequestedTheme = SettingsHelper.GetTheme(),
                    Title = _resourceLoader.GetString("AccountAddConfirmation"),
                    XamlRoot = XamlRoot // It is essential to set the XAML root here to avoid any possible exception.
                };

                var contentDialogResult = await _contentDialogue.ShowAsync();

                _contentDialogue = null;

                if (contentDialogResult != ContentDialogResult.Primary)
                {
                    applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);
                    return;
                } // end if

                ShowInfoBarLogin(_resourceLoader.GetString("AccountAddNoCharacterSuccess"), InfoBarSeverity.Success);
            } // end if

            _app.AccHelper.StoreCharacters(characters, containerKeyAccount);
        } // end method AddOrUpdateAccountAsync

        /// <summary>
        /// Apply the server selection.
        /// </summary>
        private void ApplyServerSelection()
        {
            var comboBoxServerSelectedItem = ComboBoxServer.SelectedItem as ComboBoxItem;

            if (comboBoxServerSelectedItem == null) return;

            var uriLoginMiHoYo = GetLoginWebPageUri();
            var workArea = DisplayArea.GetFromWindowId(_mainWindow.WinId, DisplayAreaFallback.Primary).WorkArea;

            // The work area's height/width minus the specific constant is primarily for reserving space for the navigation pane.
            var pageMaxHeight = workArea.Height - 6 * AppConstantsHelper.MainWindowPositionOffset;
            var pageMaxWidth = workArea.Width - 6 * AppConstantsHelper.MainWindowPositionOffset;

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
                ButtonLoginCompleteConfirm.IsEnabled = false;
                ButtonLoginCompleteConfirm.Visibility = isServerCn ? Visibility.Collapsed : Visibility.Visible;
                Height = pageMaxHeight < pageSuggestedHeight ? pageMaxHeight : pageSuggestedHeight;
                Width = pageMaxWidth < pageSuggestedWidth ? pageMaxWidth : pageSuggestedWidth;
            }
            else
            {
                Height = pageMaxHeight < AppConstantsHelper.AddAccountPageLoginAlternativeHeight
                    ? pageMaxHeight
                    : AppConstantsHelper.AddAccountPageLoginAlternativeHeight;
                HyperlinkLoginHeaderPlace.NavigateUri = uriLoginMiHoYo;
                RunLoginHeaderPlace.Text = _resourceLoader.GetString(isServerCn ? "MiHoYo" : "HoYoLab");
                Width = pageMaxWidth < AppConstantsHelper.AddAccountPageLoginAlternativeWidth
                    ? pageMaxWidth
                    : AppConstantsHelper.AddAccountPageLoginAlternativeWidth;
            } // end if...else
        } // end method ApplyServerSelection

        /// <summary>
        /// Check if the number of the accounts added has already reached the limit.
        /// </summary>
        /// <returns>A flag indicating if the number of the accounts added has already reached the limit.</returns>
        private bool CheckAccountsCount()
        {
            var hasReachedLimit = _app.AccHelper.CountAccounts() >= AccountsHelper.CountAccountsMax;

            if (hasReachedLimit)
                ShowInfoBarLogin(_resourceLoader.GetString("AccountAddReachLimit"), InfoBarSeverity.Error);
            else InfoBarLogin.IsOpen = false;

            return hasReachedLimit;
        } // end method CheckAccountsCount

        /// <summary>
        /// Choose the login method automatically.
        /// </summary>
        private async void ChooseLoginMethodAsync()
        {
            var propertySetSettings = ApplicationData.Current.LocalSettings
                .Containers[SettingsHelper.ContainerKeySettings].Values;

            if ((bool)propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways]) UseAlternativeLoginMethod(true);
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

                    ButtonLoginAssist.IsEnabled = false;
                    FontIconLoginAssist.Glyph = AppConstantsHelper.GlyphUpdateRestore;
                    GridLogin.Children.Add(_webView2LoginWebPage);
                    Grid.SetRow(_webView2LoginWebPage, 1);
                    TextBlockLoginHeaderWebPage.Text = _resourceLoader.GetString("LoginHeaderWebPage");
                    ToolTipService.SetToolTip(ButtonLoginAssist, _resourceLoader.GetString("LoginWebPageReturn"));
                    ToolTipService.SetToolTip(ButtonLoginCompleteConfirm,
                        _resourceLoader.GetString("LoginCompleteConfirm"));
                }
                catch (Exception exception)
                {
                    Log.Error("Failed to detect WebView2 Runtime.");
                    Log.Error(exception.ToString());
                    UseAlternativeLoginMethod();
                } // end try...catch

            ComboBoxServer.SelectedItem = propertySetSettings[SettingsHelper.KeyServerDefault] switch
            {
                AccountsHelper.TagServerCn => ComboBoxItemServerCn,
                AccountsHelper.TagServerGlobal => ComboBoxItemServerGlobal,
                _ => null
            };
        } // end method ChooseLoginMethodAsync

        /// <summary>
        /// Get the login web page URI.
        /// </summary>
        /// <returns>The login web page URI.</returns>
        private Uri GetLoginWebPageUri()
        {
            return new Uri(ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                ? AccountsHelper.UrlLoginMiHoYo
                : AccountsHelper.UrlLoginHoYoLab);
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
            TextBoxLoginAlternative.ClearUndoRedoHistory();
        } // end method InitialiseLogin

        /// <summary>
        /// Log in.
        /// </summary>
        private async void LogInAsync()
        {
            ShowGridBusyIndicator();

            if (CheckAccountsCount()) InitialiseLogin();
            else
            {
                string accountId;
                string cookies;

                TextBlockBusyIndicator.Text = _resourceLoader.GetString("CookiesProcessing");

                if (_isWebView2Available)
                {
                    var rawCookies = (await _webView2LoginWebPage.CoreWebView2.CookieManager.GetCookiesAsync(
                        ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                            ? AccountsHelper.UrlCookiesMiHoYo
                            : AccountsHelper.UrlCookiesHoYoLab)).ToImmutableList();

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
                    cookies.Contains(AccountsHelper.CookieKeyToken)) await AddOrUpdateAccountAsync(accountId, cookies);
                else
                {
                    Log.Warning((_isWebView2Available ? "Web page" : "Alternative") +
                                $" login failed due to invalid cookies ({cookies}).");
                    InitialiseLogin();
                    ShowInfoBarLogin(_resourceLoader.GetString("LoginFail"), InfoBarSeverity.Error);
                } // end if...else
            } // end if

            _mainWindow.NavigationViewItemBodyRealTimeNotes.IsEnabled = true;
            GridBusyIndicator.Visibility = Visibility.Collapsed;
            TextBlockBusyIndicator.Text = _resourceLoader.GetString("Initialising");
        } // end method LogInAsync

        /// <summary>
        /// Invoked immediately after the <see cref="AddAccountPage"/> is unloaded and is no longer the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="args">Details about the navigation that has unloaded the current <see cref="AddAccountPage"/>.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            if (_isWebView2Available) _webView2LoginWebPage.Close();

            ButtonLoginCompleteConfirm.RemoveHandler(PointerPressedEvent,
                (PointerEventHandler)ButtonLoginCompleteConfirm_OnPointerPressed);
            ButtonLoginCompleteConfirm.RemoveHandler(PointerReleasedEvent,
                (PointerEventHandler)ButtonLoginCompleteConfirm_OnPointerReleased);
            base.OnNavigatedFrom(args);
        } // end method OnNavigatedFrom

        /// <summary>
        /// Invoked when the <see cref="AddAccountPage"/> is loaded and becomes the current source of a parent <see cref="Frame"/>.
        /// </summary>
        /// <param name="args">Details about the pending navigation that will load the current <see cref="AddAccountPage"/>.</param>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            ButtonLoginCompleteConfirm.AddHandler(PointerPressedEvent,
                new PointerEventHandler(ButtonLoginCompleteConfirm_OnPointerPressed), true);
            ButtonLoginCompleteConfirm.AddHandler(PointerReleasedEvent,
                new PointerEventHandler(ButtonLoginCompleteConfirm_OnPointerReleased), true);
            base.OnNavigatedTo(args);
        } // end method OnNavigatedTo

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
        /// Show the busy indicator grid.
        /// </summary>
        private void ShowGridBusyIndicator()
        {
            _mainWindow.NavigationViewItemBodyRealTimeNotes.IsEnabled = false;
            GridBusyIndicator.Visibility = Visibility.Visible;
        } // end method ShowGridBusyIndicator

        /// <summary>
        /// Show the login info bar.
        /// </summary>
        /// <param name="message">The login message.</param>
        /// <param name="infoBarSeverity">The info bar's severity.</param>
        private void ShowInfoBarLogin(string message, InfoBarSeverity infoBarSeverity = InfoBarSeverity.Informational)
        {
            InfoBarLogin.Margin = new Thickness(0, 0, 0, 8);
            InfoBarLogin.Message = message;
            InfoBarLogin.Severity = infoBarSeverity;
            InfoBarLogin.IsOpen = true; // Show the info bar when ready.
        } // end method ShowInfoBarLogin

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            ComboBoxItemServerCn.Content = _resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = _resourceLoader.GetString("ServerGlobal");
            TextBlockBusyIndicator.Text = _resourceLoader.GetString("Initialising");
            TextBlockServer.Text = _resourceLoader.GetString("Server");
            TextBlockServerExplanation.Text = _resourceLoader.GetString("ServerExplanation");
            TextBlockTitle.Text = _resourceLoader.GetString("AccountAdd");
        } // end method UpdateUiText

        /// <summary>
        /// Use the alternative login method.
        /// </summary>
        /// <param name="isAlways">A flag indicating if the alternative login method is always used.</param>
        private void UseAlternativeLoginMethod(bool isAlways = false)
        {
            _isWebView2Available = false;

            if (!isAlways)
            {
                HyperlinkButtonWebView2RuntimeDownload.Content = _resourceLoader.GetString("WebView2RuntimeDownload");
                InfoBarLoginAlternativeAutomatically.Margin = new Thickness(0, 0, 0, 8);
                InfoBarLoginAlternativeAutomatically.Message =
                    _resourceLoader.GetString("LoginAlternativeAutomatically");
                InfoBarLoginAlternativeAutomatically.IsOpen = true; // Show the info bar when ready.
            } // end if

            ButtonLoginAlternative.Content = _resourceLoader.GetString("Login");
            ButtonLoginAlternativeClear.Content = _resourceLoader.GetString("Clear");
            FontIconLoginAssist.Glyph = AppConstantsHelper.GlyphHelp;
            GridLoginAlternative.Visibility = Visibility.Visible;
            RunLoginHeaderCookies.Text = _resourceLoader.GetString("Cookies");
            RunLoginHeaderEnter.Text = _resourceLoader.GetString("Enter");
            TextBlockLoginHeaderAlternative.Visibility = Visibility.Visible;
            TextBlockLoginHeaderWebPage.Visibility = Visibility.Collapsed;
            ToolTipService.SetToolTip(ButtonLoginAssist, _resourceLoader.GetString("CookiesHowToGet"));
        } // end method UseAlternativeLoginMethod

        #endregion Methods

        #region Event Handlers

        // Handle the actual theme changed event of the page for adding an account.
        private void AddAccountPage_OnActualThemeChanged(FrameworkElement sender, object args)
        {
            if (_contentDialogue != null) _contentDialogue.RequestedTheme = SettingsHelper.GetTheme();
        } // end method AddAccountPage_OnActualThemeChanged

        // Handle the alternative login button's click event.
        private void ButtonLoginAlternative_OnClick(object sender, RoutedEventArgs e)
        {
            LogInAsync();
        } // end method ButtonLoginAlternative_OnClick

        // Handle the click event of the button for clearing cookies for the alternative login.
        private void ButtonLoginAlternativeClear_OnClick(object sender, RoutedEventArgs e)
        {
            TextBoxLoginAlternative.Text = string.Empty;
        } // end method ButtonLoginAlternativeClear_OnClick

        // Handle the click event of the button for assisting login.
        private void ButtonLoginAssist_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isWebView2Available) _webView2LoginWebPage.Source = GetLoginWebPageUri();
            else new CommandsViewModel().OpenLinkInDefaultCommand.Execute(AppConstantsHelper.UrlCookiesHowToGet);
        } // end method ButtonLoginAssist_OnClick

        // Handle the click event of the button for confirming completing login.
        private void ButtonLoginCompleteConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            LogInAsync();
        } // end method ButtonLoginCompleteConfirm_OnClick

        // Handle the pointer pressed event of the button for confirming completing login.
        private void ButtonLoginCompleteConfirm_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(AnimatedIconLoginCompleteConfirm, "NormalOff"); // NormalOnToNormalOff
        } // end method ButtonLoginCompleteConfirm_OnPointerPressed

        // Handle the pointer released event of the button for confirming completing login.
        private void ButtonLoginCompleteConfirm_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            AnimatedIcon.SetState(AnimatedIconLoginCompleteConfirm, "NormalOn"); // NormalOffToNormalOn
        } // end method ButtonLoginCompleteConfirm_OnPointerReleased

        // Handle the server combo box item's loaded event.
        private void ComboBoxItemServer_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemServerActualWidth = ((ComboBoxItem)sender).ActualWidth;

            if (ComboBoxServer.MinWidth < comboBoxItemServerActualWidth)
                ComboBoxServer.MinWidth = comboBoxItemServerActualWidth;
        } // end method ComboBoxItemServer_OnLoaded

        // Handle the server combo box's selection changed event.
        private void ComboBoxServer_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyServerSelection();
        } // end method ComboBoxServer_OnSelectionChanged

        // Handle the web page login CoreWebView2's source changed event.
        private void CoreWebView2LoginWebPage_OnSourceChanged(CoreWebView2 sender,
            CoreWebView2SourceChangedEventArgs args)
        {
            var isServerCn = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn;
            var webView2LoginWebPageSource = _webView2LoginWebPage.Source.ToString();

            if (isServerCn && webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginEndMiHoYo))
                ShowGridBusyIndicator();

            ButtonLoginAssist.IsEnabled =
                !((isServerCn && (webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginMiHoYo) ||
                                  webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginRedirectMiHoYo))) ||
                  (!isServerCn && webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginHoYoLab)));
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

#pragma warning disable CA1822 // Mark members as static
        // Handle the info bar's closing event.
        private void InfoBar_OnClosing(InfoBar sender, InfoBarClosingEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            sender.Margin = new Thickness(0);
        } // end method InfoBar_OnClosing

        // Handle the alternative login text box's text changed event.
        private void TextBoxLoginAlternative_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ButtonLoginAlternative.IsEnabled = TextBoxLoginAlternative.Text.Trim() != string.Empty;
            ButtonLoginAlternativeClear.IsEnabled = TextBoxLoginAlternative.Text != string.Empty;
        } // end method TextBoxLoginAlternative_OnTextChanged

        // Handle the web page login WebView2's navigation completed event.
        private void WebView2LoginWebPage_OnNavigationCompleted(WebView2 sender,
            CoreWebView2NavigationCompletedEventArgs args)
        {
            switch (ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn)
            {
                // Although the CoreWebView2's source changed event uses the same condition, this event is to ensure cookies.
                case true when _webView2LoginWebPage.Source.ToString().Contains(AccountsHelper.UrlLoginEndMiHoYo):
                    LogInAsync();
                    break;

                case false:
                    ButtonLoginCompleteConfirm.IsEnabled = true;
                    break;
            } // end switch-case
        } // end method WebView2LoginWebPage_OnNavigationCompleted

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views