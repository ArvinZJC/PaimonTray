using Microsoft.UI.Xaml;
using Microsoft.Web.WebView2.Core;
using System;
using Serilog;

namespace PaimonTray.Views
{
    /// <summary>
    /// The page for adding an account.
    /// </summary>
    public sealed partial class AddAccountPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the page for adding an account.
        /// </summary>
        public AddAccountPage()
        {
            InitializeComponent();
            ConfigWebView2LoginAsync();
        } // end constructor AddAccountPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Configure the login WebView2.
        /// </summary>
        private async void ConfigWebView2LoginAsync()
        {
            await WebView2Login.EnsureCoreWebView2Async();
            WebView2Login.CoreWebView2.SourceChanged += CoreWebView2Login_OnSourceChanged;
        } // end method ConfigWebView2LoginAsync

        #endregion Methods

        #region Event Handlers

        // Handle the unloaded event of the page for adding an account.
        private void AddAccountPage_OnUnloaded(object sender, RoutedEventArgs e)
        {
            WebView2Login.Close();
        } // end method AddAccountPage_OnUnloaded

        // Handle the login CoreWebView2's source changed event.
        private void CoreWebView2Login_OnSourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
        {
            if (WebView2Login.Source.ToString().Contains("https://user.mihoyo.com/#/account")) return;
        } // end method CoreWebView2Login_OnSourceChanged

        #endregion Event Handlers
    } // end class AddAccountPage
} // end namespace PaimonTray.Views