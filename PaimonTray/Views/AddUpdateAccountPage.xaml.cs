using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Web.WebView2.Core;
using PaimonTray.Helpers;
using PaimonTray.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace PaimonTray.Views
{
    /// <summary>
    /// The page for adding/updating an account.
    /// </summary>
    public sealed partial class AddUpdateAccountPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the page for adding/updating an account.
        /// </summary>
        public AddUpdateAccountPage()
        {
            _app = Application.Current as App;
            _mainWindow = _app?.WindowsH.GetExistingMainWindow()?.Win as MainWindow;
            InitializeComponent();
            _ = ChooseLoginMethodAsync();
            ToggleStatusVisibility();
            UpdateUiText();
        } // end constructor AddUpdateAccountPage

        #endregion Constructors

        #region Event Handlers

        // Handle the accounts helper's property changed event.
        private void AccountsHelper_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == AccountsHelper.PropertyNameIsAddingUpdating ||
                e.PropertyName == AccountsHelper.PropertyNameIsManaging) ToggleStatusVisibility();
        } // end method AccountsHelper_OnPropertyChanged

        // Handle the loaded event of the page for adding/updating an account.
        private void AddUpdateAccountPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            _app.AccountsH.PropertyChanged += AccountsHelper_OnPropertyChanged;
            _mainWindow.MainWinViewModel.PropertyChanged += MainWindowViewModel_OnPropertyChanged;
            ButtonLoginCompleteConfirm.AddHandler(PointerPressedEvent,
                new PointerEventHandler(ButtonLoginCompleteConfirm_OnPointerPressed), true);
            ButtonLoginCompleteConfirm.AddHandler(PointerReleasedEvent,
                new PointerEventHandler(ButtonLoginCompleteConfirm_OnPointerReleased), true);
        } // end method AddUpdateAccountPage_OnLoaded

        // Handle the unloaded event of the page for adding/updating an account.
        private void AddUpdateAccountPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_isWebView2Available)
            {
                _webView2LoginWebPage.CoreWebView2.SourceChanged -= CoreWebView2LoginWebPage_OnSourceChanged;
                _webView2LoginWebPage.Close(); // Close to ensure ending the WebView2 processes.
                _webView2LoginWebPage = null;
            } // end if

            _app.AccountsH.PropertyChanged -= AccountsHelper_OnPropertyChanged;
            _mainWindow.MainWinViewModel.PropertyChanged -= MainWindowViewModel_OnPropertyChanged;
            ButtonLoginCompleteConfirm.RemoveHandler(PointerPressedEvent,
                (PointerEventHandler)ButtonLoginCompleteConfirm_OnPointerPressed);
            ButtonLoginCompleteConfirm.RemoveHandler(PointerReleasedEvent,
                (PointerEventHandler)ButtonLoginCompleteConfirm_OnPointerReleased);

            _app = null;
            _mainWindow = null;
        } // end method AddUpdateAccountPage_OnUnloaded

        // Handle the alternative login button's click event.
        private void ButtonLoginAlternative_OnClick(object sender, RoutedEventArgs e)
        {
            _ = LogInAsync();
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
            else _app.CommandsVm.OpenLinkInDefaultCommand.Execute(AppFieldsHelper.UrlCookiesHowToGet);
        } // end method ButtonLoginAssist_OnClick

        // Handle the click event of the button for confirming completing login.
        private void ButtonLoginCompleteConfirm_OnClick(object sender, RoutedEventArgs e)
        {
            _ = LogInAsync();
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

#pragma warning disable CA1822 // Mark members as static
        // Handle the combo box's drop-down closed event.
        private void ComboBox_OnDropDownClosed(object sender, object e)
#pragma warning restore CA1822 // Mark members as static
        {
            if (sender is not ComboBox comboBox) return;

            comboBox.MinWidth = 0;
        } // end method ComboBox_OnDropDownClosed

        // Handle the server combo box item's loaded event.
        private void ComboBoxItemServer_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBoxItemServerActualWidth = (sender as ComboBoxItem)?.ActualWidth ?? 0;

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

            ButtonLoginAssist.IsEnabled =
                !((isServerCn && webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginMiHoYo)) ||
                  (!isServerCn && webView2LoginWebPageSource.Contains(AccountsHelper.UrlLoginHoYoLab)));
        } // end method CoreWebView2LoginWebPage_OnSourceChanged

        // Handle the body grid's size changed event.
        private void GridBody_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageSize();
        } // end method GridBody_OnSizeChanged

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
            if (e.PropertyName == MainWindowViewModel.PropertyNameNavViewPaneDisplayMode) SetPageSize();
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
            ButtonLoginCompleteConfirm.IsEnabled = true;
        } // end method WebView2LoginWebPage_OnNavigationCompleted

        #endregion Event Handlers

        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private App _app;

        /// <summary>
        /// A flag indicating if the WebView2 is available.
        /// </summary>
        private bool _isWebView2Available;

        /// <summary>
        /// The main window.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// The login web page WebView2.
        /// </summary>
        private WebView2 _webView2LoginWebPage;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Add/Update an account.
        /// </summary>
        /// <param name="aUid">The account's UID.</param>
        /// <param name="cookies">The cookies.</param>
        /// <returns>Void.</returns>
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
            var resourceLoader = _app.SettingsH.ResLoader;

            TextBlockStatus.Text =
                resourceLoader.GetString(shouldUpdateAccount ? "AccountStatusUpdating" : "AccountStatusAdding");
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
                ShowLoginInfoBar(resourceLoader.GetString("LoginFailExtraInfo"), resourceLoader.GetString("LoginFail"),
                    InfoBarSeverity.Error);

                if (shouldUpdateAccount) await _app.AccountsH.AddUpdateCharactersAsync(null, containerKeyAccount);
                else applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);

                return;
            } // end if

            if (shouldUpdateAccount)
            {
                await _app.AccountsH.AddUpdateCharactersAsync(characters, containerKeyAccount);

                if (_app.AccountsH.TrySelectAccountGroupFirstEnabledCharacter(containerKeyAccount))
                    _mainWindow.NavigationViewBody.SelectedItem = _mainWindow.NavigationViewItemBodyRealTimeNotes;
                else
                    ShowLoginInfoBar(resourceLoader.GetString("AccountUpdatedExtraInfo"),
                        resourceLoader.GetString("AccountUpdated"));

                return;
            } // end if

            if (characters.Count is 0)
            {
                if (await ContentDialogueAccountAddNoCharacter.ShowAsync() is not ContentDialogResult.Primary)
                {
                    applicationDataContainerAccounts.DeleteContainer(containerKeyAccount);
                    return;
                } // end if

                ShowLoginInfoBar(null, resourceLoader.GetString("AccountAddNoCharacterSuccess"),
                    InfoBarSeverity.Success);
            } // end if

            await _app.AccountsH.AddUpdateCharactersAsync(characters, containerKeyAccount);

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
            var resourceLoader = _app.SettingsH.ResLoader;
            var uriLoginMiHoYo = GetLoginWebPageUri();

            if (_app.AccountsH.CountAccounts() < AccountsHelper.CountAccountsMax) InfoBarLogin.IsOpen = false;
            else
                ShowLoginInfoBar(resourceLoader.GetString("AccountAddReachLimitExtraInfo"),
                    resourceLoader.GetString("AccountAddReachLimit"), InfoBarSeverity.Error);

            if (_isWebView2Available)
            {
                _webView2LoginWebPage.Height = isServerCn ? 488 : 608;
                _webView2LoginWebPage.Source = uriLoginMiHoYo;
                GridServer.Width = isServerCn ? 1220 : 740;
            }
            else
            {
                GridServer.Width = 400;
                HyperlinkLoginHeaderPlace.NavigateUri = uriLoginMiHoYo;
                RunLoginHeaderPlace.Text = resourceLoader.GetString(isServerCn ? "MiHoYo" : "HoYoLab");
                TextBoxLoginAlternative.Width = GridServer.Width;
            } // end if...else

            StackPanelLogin.Width = GridServer.Width;
            TextBlockTitle.MaxWidth = GridServer.Width + 12 * 2; // Need consider the server grid's padding.
        } // end method ApplyServerSelection

        /// <summary>
        /// Choose the login method automatically.
        /// </summary>
        /// <returns>Void.</returns>
        private async Task ChooseLoginMethodAsync()
        {
            var propertySetSettings = _app.SettingsH.PropertySetSettings;

            if (propertySetSettings[SettingsHelper.KeyLoginAlternativeAlways] is true) UseAlternativeLoginMethod(true);
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

                    var resourceLoader = _app.SettingsH.ResLoader;

                    ButtonLoginAssist.IsEnabled = false;
                    FontIconLoginAssist.Glyph = AppFieldsHelper.GlyphUpdateRestore;
                    GridLogin.Children.Add(_webView2LoginWebPage);
                    Grid.SetRow(_webView2LoginWebPage, 1);
                    TextBlockLoginHeaderWebPage.Text = resourceLoader.GetString("LoginHeaderWebPage");
                    ToolTipService.SetToolTip(ButtonLoginAssist, resourceLoader.GetString("LoginWebPageReturn"));
                    ToolTipService.SetToolTip(ButtonLoginCompleteConfirm,
                        resourceLoader.GetString("LoginCompleteConfirm"));
                }
                catch (Exception exception)
                {
                    Log.Error("Failed to detect WebView2 Runtime.");
                    Log.Error(exception.ToString());
                    UseAlternativeLoginMethod();
                } // end try...catch

            var serverDefault = propertySetSettings[SettingsHelper.KeyServerDefault] as string;

            if (serverDefault == AccountsHelper.TagServerCn) ComboBoxServer.SelectedItem = ComboBoxItemServerCn;
            else if (serverDefault == AccountsHelper.TagServerGlobal)
                ComboBoxServer.SelectedItem = ComboBoxItemServerGlobal;
            else ComboBoxServer.SelectedItem = null;
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
        /// <returns>Void.</returns>
        private async Task LogInAsync()
        {
            _app.AccountsH.IsAddingUpdating = true;

            string aUid;
            string cookies;
            var resourceLoader = _app.SettingsH.ResLoader;

            TextBlockStatus.Text = resourceLoader.GetString("CookiesStatusProcessing");

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
                ShowLoginInfoBar(resourceLoader.GetString("LoginFailExtraInfo"), resourceLoader.GetString("LoginFail"),
                    InfoBarSeverity.Error);
            } // end if...else

            _app.AccountsH.IsAddingUpdating = false;
        } // end method LogInAsync

        /// <summary>
        /// Process the raw cookies.
        /// </summary>
        /// <typeparam name="T">Should be a <see cref="string"/> or <see cref="CoreWebView2Cookie"/> type.</typeparam>
        /// <param name="rawCookies">The raw cookies.</param>
        /// <returns>A tuple. 1st item: the account's UID; 2nd item: the processed cookies.</returns>
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

                if (cookieName != AccountsHelper.CookieKeyUid && cookieName != AccountsHelper.CookieKeyToken) continue;

                cookies.Add($"{cookieName}={cookieValue};");

                if (cookieName == AccountsHelper.CookieKeyUid) aUid = cookieValue;

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
            InfoBarLogin.Margin = new Thickness(0, 0, 0, AppFieldsHelper.InfoBarMarginBottom);
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
            var resourceLoader = _app.SettingsH.ResLoader;

            if (_app.AccountsH.CountAccounts() < AccountsHelper.CountAccountsMax &&
                InfoBarLogin.Title == resourceLoader.GetString("AccountAddReachLimit")) InfoBarLogin.IsOpen = false;

            TextBlockStatus.Text = resourceLoader.GetString("StatusLoading");
            GridStatus.Visibility = _app.AccountsH.IsAddingUpdating || _app.AccountsH.IsManaging
                ? Visibility.Visible
                : Visibility.Collapsed; // Show the status grid when ready.
        } // end method ToggleStatusVisibility

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = _app.SettingsH.ResLoader;

            ComboBoxItemServerCn.Content = resourceLoader.GetString("ServerCn");
            ComboBoxItemServerGlobal.Content = resourceLoader.GetString("ServerGlobal");
            ContentDialogueAccountAddNoCharacter.CloseButtonText = resourceLoader.GetString("No");
            ContentDialogueAccountAddNoCharacter.Content = resourceLoader.GetString("AccountAddNoCharacterExplanation");
            ContentDialogueAccountAddNoCharacter.PrimaryButtonText = resourceLoader.GetString("Yes");
            ContentDialogueAccountAddNoCharacter.Title = resourceLoader.GetString("AccountAddConfirmation");
            TextBlockServer.Text = resourceLoader.GetString("Server");
            TextBlockServerExplanation.Text = resourceLoader.GetString("ServerExplanation");
            TextBlockTitle.Text = resourceLoader.GetString("AccountAddUpdate");
        } // end method UpdateUiText

        /// <summary>
        /// Use the alternative login method.
        /// </summary>
        /// <param name="isAlways">A flag indicating if the alternative login method is always used.</param>
        private void UseAlternativeLoginMethod(bool isAlways = false)
        {
            _isWebView2Available = false;

            var resourceLoader = _app.SettingsH.ResLoader;

            if (!isAlways)
            {
                HyperlinkButtonWebView2RuntimeDownload.Content = resourceLoader.GetString("WebView2RuntimeDownload");
                InfoBarLoginAlternativeAutomatically.Margin =
                    new Thickness(0, 0, 0, AppFieldsHelper.InfoBarMarginBottom);
                InfoBarLoginAlternativeAutomatically.Message =
                    resourceLoader.GetString("LoginAlternativeAutomaticallyExtraInfo");
                InfoBarLoginAlternativeAutomatically.Title = resourceLoader.GetString("LoginAlternativeAutomatically");
                InfoBarLoginAlternativeAutomatically.IsOpen = true; // Show the info bar when ready.
            } // end if

            ButtonLoginAlternative.Content = resourceLoader.GetString("Login");
            ButtonLoginAlternativeClear.Content = resourceLoader.GetString("Clear");
            FontIconLoginAssist.Glyph = AppFieldsHelper.GlyphHelp;
            GridLoginAlternative.Visibility = Visibility.Visible;
            RunLoginHeaderCookies.Text = resourceLoader.GetString("Cookies");
            RunLoginHeaderEnter.Text = resourceLoader.GetString("Enter");
            TextBlockLoginHeaderAlternative.Visibility = Visibility.Visible;
            TextBlockLoginHeaderWebPage.Visibility = Visibility.Collapsed;
            ToolTipService.SetToolTip(ButtonLoginAssist, resourceLoader.GetString("CookiesHowToGet"));
        } // end method UseAlternativeLoginMethod

        #endregion Methods
    } // end class AddUpdateAccountPage
} // end namespace PaimonTray.Views