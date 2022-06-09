using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The page for adding/updating an account.
    /// </summary>
    public sealed partial class AddUpdateAccountPage
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        /// <summary>
        /// A flag indicating if the WebView2 is available.
        /// </summary>
        private bool _isWebView2Available;

        /// <summary>
        /// The main window.
        /// </summary>
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// The resource loader.
        /// </summary>
        private readonly ResourceLoader _resourceLoader;

        /// <summary>
        /// The login web page WebView2.
        /// </summary>
        private WebView2 _webView2LoginWebPage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the page for adding/updating an account.
        /// </summary>
        public AddUpdateAccountPage()
        {
            _app = Application.Current as App;
            _mainWindow = _app?.WindowsH.GetExistingMainWindow()?.Win as MainWindow;
            _resourceLoader = _app?.SettingsH.ResLoader;
            InitializeComponent();
            ChooseLoginMethodAsync();
            ToggleStatusVisibility();
            UpdateUiText();
        } // end constructor AddUpdateAccountPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Add/Update an account.
        /// </summary>
        /// <param name="aUid">The account's UID.</param>
        /// <param name="cookies">The cookies.</param>
        /// <returns>A task just to indicate that any later operation needs to wait.</returns>
        private async Task AddUpdateAccountAsync(string aUid, string cookies)
        {
            var applicationDataContainerAccounts = _app.AccountsH.ApplicationDataContainerAccounts;
            var server = ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                ? AccountsHelper.TagServerCn
                : AccountsHelper.TagServerGlobal;
            var containerKeyAccount = $"{server}{aUid}";
            var shouldUpdateAccount =
                applicationDataContainerAccounts.Containers
                    .ContainsKey(
                        containerKeyAccount); // A flag indicating whether the account should be updated or added.
            var propertySetAccount = applicationDataContainerAccounts
                .CreateContainer(containerKeyAccount, ApplicationDataCreateDisposition.Always)
                .Values; // Need to declare after the flag indicating whether the account should be updated or added.

            TextBlockStatus.Text =
                _resourceLoader.GetString(shouldUpdateAccount ? "StatusAccountUpdating" : "StatusAccountAdding");
            propertySetAccount[AccountsHelper.KeyCookies] = cookies;
            propertySetAccount[AccountsHelper.KeyServer] = server;
            propertySetAccount[AccountsHelper.KeyStatus] = shouldUpdateAccount
                ? AccountsHelper.TagStatusUpdating
                : AccountsHelper.TagStatusAdding;
            propertySetAccount[AccountsHelper.KeyUid] = aUid;
            InitialiseLogin();

            var characters = await _app.AccountsH.GetAccountCharactersFromApiAsync(containerKeyAccount);

            if (characters is null)
            {
                var action = shouldUpdateAccount ? "update" : "add";

                Log.Warning(
                    $"Failed to {action} the account due to null characters (account container key: {containerKeyAccount}).");
                ShowLoginInfoBar(_resourceLoader.GetString("LoginFailExtraInfo"),
                    _resourceLoader.GetString("LoginFail"), InfoBarSeverity.Error);

                if (shouldUpdateAccount) _app.AccountsH.AddUpdateCharacters(null, containerKeyAccount);
                else applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);

                return;
            } // end if

            if (shouldUpdateAccount)
            {
                _app.AccountsH.AddUpdateCharacters(characters, containerKeyAccount);

                if (_app.AccountsH.TrySelectAccountGroupFirstEnabledCharacter(containerKeyAccount))
                    _mainWindow.NavigationViewBody.SelectedItem = _mainWindow.NavigationViewItemBodyRealTimeNotes;
                else
                    ShowLoginInfoBar(_resourceLoader.GetString("AccountUpdatedExtraInfo"),
                        _resourceLoader.GetString("AccountUpdated"));

                return;
            } // end if

            if (characters.Count is 0)
            {
                if (await ContentDialogueAccountAddNoCharacter.ShowAsync() is not ContentDialogResult.Primary)
                {
                    applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);
                    return;
                } // end if

                ShowLoginInfoBar(null, _resourceLoader.GetString("AccountAddNoCharacterSuccess"),
                    InfoBarSeverity.Success);
            } // end if

            _app.AccountsH.AddUpdateCharacters(characters, containerKeyAccount);

            if (_app.AccountsH.TrySelectAccountGroupFirstEnabledCharacter(containerKeyAccount))
                _mainWindow.NavigationViewBody.SelectedItem = _mainWindow.NavigationViewItemBodyRealTimeNotes;
        } // end method AddUpdateAccountAsync

        /// <summary>
        /// Apply the server selection.
        /// </summary>
        private void ApplyServerSelection()
        {
            if (ComboBoxServer.SelectedItem is not ComboBoxItem comboBoxServerSelectedItem) return;

            var isServerCn = comboBoxServerSelectedItem == ComboBoxItemServerCn;
            var uriLoginMiHoYo = GetLoginWebPageUri();

            if (_app.AccountsH.CountAccounts() < AccountsHelper.CountAccountsMax) InfoBarLogin.IsOpen = false;
            else
                ShowLoginInfoBar(_resourceLoader.GetString("AccountAddReachLimitExtraInfo"),
                    _resourceLoader.GetString("AccountAddReachLimit"), InfoBarSeverity.Error);

            if (_isWebView2Available)
            {
                _webView2LoginWebPage.Height = isServerCn ? 616 : 608;
                _webView2LoginWebPage.Source = uriLoginMiHoYo;
                ButtonLoginCompleteConfirm.IsEnabled = false;
                ButtonLoginCompleteConfirm.Visibility = isServerCn ? Visibility.Collapsed : Visibility.Visible;
                GridServer.Width = isServerCn ? 632 : 740;
                StackPanelLogin.Width = GridServer.Width;
            }
            else
            {
                GridServer.Width = 400;
                HyperlinkLoginHeaderPlace.NavigateUri = uriLoginMiHoYo;
                RunLoginHeaderPlace.Text = _resourceLoader.GetString(isServerCn ? "MiHoYo" : "HoYoLab");
                StackPanelLogin.Width = GridServer.Width;
                TextBoxLoginAlternative.Width = GridServer.Width;
            } // end if...else
        } // end method ApplyServerSelection

        /// <summary>
        /// Choose the login method automatically.
        /// </summary>
        private async void ChooseLoginMethodAsync()
        {
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

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
            _app.AccountsH.IsAddingUpdating = true;

            string aUid;
            string cookies;

            TextBlockStatus.Text = _resourceLoader.GetString("StatusCookiesProcessing");

            if (_isWebView2Available)
            {
                var rawCookies = (await _webView2LoginWebPage.CoreWebView2.CookieManager.GetCookiesAsync(
                    ComboBoxServer.SelectedItem as ComboBoxItem == ComboBoxItemServerCn
                        ? AccountsHelper.UrlCookiesMiHoYo
                        : AccountsHelper.UrlCookiesHoYoLab)).ToImmutableList();

                (aUid, cookies) = ProcessCookies(ref rawCookies);
            }
            else
            {
                var rawCookies = TextBoxLoginAlternative.Text.Trim().Split(';',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToImmutableList();

                (aUid, cookies) = ProcessCookies(ref rawCookies);
            } // end if...else

            // Execute if the account's UID and cookies are valid.
            if (aUid != string.Empty && cookies.Contains(AccountsHelper.CookieKeyUid) &&
                cookies.Contains(AccountsHelper.CookieKeyToken)) await AddUpdateAccountAsync(aUid, cookies);
            else
            {
                Log.Warning((_isWebView2Available ? "Web page" : "Alternative") +
                            $" login failed due to invalid cookies ({cookies}).");
                InitialiseLogin();
                ShowLoginInfoBar(_resourceLoader.GetString("LoginFailExtraInfo"),
                    _resourceLoader.GetString("LoginFail"), InfoBarSeverity.Error);
            } // end if...else

            _app.AccountsH.IsAddingUpdating = false;
        } // end method LogInAsync

        /// <summary>
        /// Invoked immediately after the page is unloaded and is no longer the current source of a parent frame.
        /// </summary>
        /// <param name="e">Details about the navigation that has unloaded the current page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (_isWebView2Available)
            {
                _webView2LoginWebPage.CoreWebView2.SourceChanged -= CoreWebView2LoginWebPage_OnSourceChanged;
                _webView2LoginWebPage.NavigationCompleted -= WebView2LoginWebPage_OnNavigationCompleted;
                _webView2LoginWebPage.Close(); // Close at last to avoid the null reference exception.
            } // end if

            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            ButtonLoginCompleteConfirm.RemoveHandler(PointerPressedEvent,
                (PointerEventHandler)ButtonLoginCompleteConfirm_OnPointerPressed);
            ButtonLoginCompleteConfirm.RemoveHandler(PointerReleasedEvent,
                (PointerEventHandler)ButtonLoginCompleteConfirm_OnPointerReleased);
            base.OnNavigatedFrom(e);
        } // end method OnNavigatedFrom

        /// <summary>
        /// Invoked when the page is loaded and becomes the current source of a parent frame.
        /// </summary>
        /// <param name="e">Details about the pending navigation that will load the current page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            ButtonLoginCompleteConfirm.AddHandler(PointerPressedEvent,
                new PointerEventHandler(ButtonLoginCompleteConfirm_OnPointerPressed), true);
            ButtonLoginCompleteConfirm.AddHandler(PointerReleasedEvent,
                new PointerEventHandler(ButtonLoginCompleteConfirm_OnPointerReleased), true);
            base.OnNavigatedTo(e);
        } // end method OnNavigatedTo

        /// <summary>
        /// Process the raw cookies.
        /// </summary>
        /// <typeparam name="T">Should be a <see cref="string"/> or <see cref="CoreWebView2Cookie"/> type.</typeparam>
        /// <param name="rawCookies">The raw cookies.</param>
        /// <returns>1st item: the account's UID; 2nd item: the processed cookies.</returns>
        private static (string, string) ProcessCookies<T>(ref ImmutableList<T> rawCookies)
        {
            var aUid = string.Empty;
            var cookieName = string.Empty;
            var cookies = new List<string>();
            var cookieValue = string.Empty;

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

                        if (cookieParts.Length is not 2) continue; // The key and value.

                        cookieName = cookieParts[0];
                        cookieValue = cookieParts[1];
                        break;
                } // end switch-case

                if (cookieName is not (AccountsHelper.CookieKeyUid or AccountsHelper.CookieKeyToken)) continue;

                cookies.Add($"{cookieName}={cookieValue};");

                if (cookieName is AccountsHelper.CookieKeyUid) aUid = cookieValue;

                if (cookies.Count is 2) break; // The UID and token cookies.
            } // end foreach

            return (aUid, string.Concat(cookies));
        } // end generic method ProcessCookies

        /// <summary>
        /// Set the page size.
        /// </summary>
        private void SetPageSize()
        {
            var pageMaxSize = _app.WindowsH.GetMainWindowPageMaxSize();

            Height = pageMaxSize.Height < GridBody.ActualHeight ? pageMaxSize.Height : GridBody.ActualHeight;
            Width = pageMaxSize.Width < GridBody.ActualWidth ? pageMaxSize.Width : GridBody.ActualWidth;
        } // end method SetPageSize

        /// <summary>
        /// Show the login info bar.
        /// </summary>
        /// <param name="message">The login message.</param>
        /// <param name="title">The login title.</param>
        /// <param name="infoBarSeverity">The info bar's severity.</param>
        private void ShowLoginInfoBar(string message, string title,
            InfoBarSeverity infoBarSeverity = InfoBarSeverity.Informational)
        {
            InfoBarLogin.Margin = new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
            InfoBarLogin.Message = message;
            InfoBarLogin.Severity = infoBarSeverity;
            InfoBarLogin.Title = title;
            InfoBarLogin.IsOpen = true; // Show the info bar when ready.
        } // end method ShowLoginInfoBar

        /// <summary>
        /// Show/Hide the status.
        /// </summary>
        private void ToggleStatusVisibility()
        {
            if (_app.AccountsH.CountAccounts() < AccountsHelper.CountAccountsMax &&
                InfoBarLogin.Title == _resourceLoader.GetString("AccountAddReachLimit")) InfoBarLogin.IsOpen = false;

            TextBlockStatus.Text = _resourceLoader.GetString("StatusLoading");
            GridStatus.Visibility = _app.AccountsH.IsAddingUpdating || _app.AccountsH.IsManaging
                ? Visibility.Visible
                : Visibility.Collapsed; // Show the status grid when ready.
        } // end method ToggleStatusVisibility

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            ComboBoxItemServerCn.Content = _resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = _resourceLoader.GetString("ServerGlobal");
            ContentDialogueAccountAddNoCharacter.CloseButtonText = _resourceLoader.GetString("No");
            ContentDialogueAccountAddNoCharacter.Content =
                _resourceLoader.GetString("AccountAddNoCharacterExplanation");
            ContentDialogueAccountAddNoCharacter.PrimaryButtonText = _resourceLoader.GetString("Yes");
            ContentDialogueAccountAddNoCharacter.Title = _resourceLoader.GetString("AccountAddConfirmation");
            TextBlockServer.Text = _resourceLoader.GetString("Server");
            TextBlockServerExplanation.Text = _resourceLoader.GetString("ServerExplanation");
            TextBlockTitle.Text = _resourceLoader.GetString("AccountAddUpdate");
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
                InfoBarLoginAlternativeAutomatically.Margin =
                    new Thickness(0, 0, 0, AppConstantsHelper.InfoBarMarginBottom);
                InfoBarLoginAlternativeAutomatically.Message =
                    _resourceLoader.GetString("LoginAlternativeAutomaticallyExtraInfo");
                InfoBarLoginAlternativeAutomatically.Title = _resourceLoader.GetString("LoginAlternativeAutomatically");
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

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is AccountsHelper.PropertyNameIsAddingUpdating or AccountsHelper.PropertyNameIsManaging)
                ToggleStatusVisibility();
        } // end method AccountsHelper_OnPropertyChanged

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
            else _app.CommandsVm.OpenLinkInDefaultCommand.Execute(AppConstantsHelper.UrlCookiesHowToGet);
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

            if (isServerCn && webView2LoginWebPageSource.Contains(AccountsHelper.UrlBaseLoginEndMiHoYo))
                _app.AccountsH.IsAddingUpdating = true;

            ButtonLoginAssist.IsEnabled =
                !((isServerCn && (webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginMiHoYo) ||
                                  webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginRedirectMiHoYo))) ||
                  (!isServerCn && webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginHoYoLab)));
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

        // Handle the body grid's size changed event.
        private void GridBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageSize();
        } // end method GridBody_OnSizeChanged

        // Handle the root grid's loaded event.
        private void GridRoot_OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainWinViewModel.PropertyChanged += MainWindowViewModel_OnPropertyChanged;
        } // end method GridRoot_OnLoaded

#pragma warning disable CA1822 // Mark members as static
        // Handle the info bar's closing event.
        private void InfoBar_OnClosing(InfoBar sender, InfoBarClosingEventArgs args)
#pragma warning restore CA1822 // Mark members as static
        {
            sender.Margin = new Thickness(0);
        } // end method InfoBar_OnClosing

        // Handle the main window view model's property changed event.
        private void MainWindowViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is MainWindowViewModel.PropertyNameNavViewPaneDisplayMode) SetPageSize();
        } // end method MainWindowViewModel_OnPropertyChanged

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
                case true when _webView2LoginWebPage.Source.ToString().Contains(AccountsHelper.UrlBaseLoginEndMiHoYo):
                    LogInAsync();
                    break;

                case false:
                    ButtonLoginCompleteConfirm.IsEnabled = true;
                    break;
            } // end switch-case
        } // end method WebView2LoginWebPage_OnNavigationCompleted

        #endregion Event Handlers
    } // end class AddUpdateAccountPage
} // end namespace PaimonTray.Views