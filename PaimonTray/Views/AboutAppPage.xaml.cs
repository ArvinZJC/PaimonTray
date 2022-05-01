using Microsoft.UI.Xaml;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;

namespace PaimonTray.Views
{
    /// <summary>
    /// The app introduction page.
    /// </summary>
    public sealed partial class AboutAppPage
    {
        #region Constructors

        /// <summary>
        /// Initialise the app introduction page.
        /// </summary>
        public AboutAppPage()
        {
            InitializeComponent();
            UpdateUiText();

            TextBlockCopyright.Text = $"© {DateTime.Now.Year} {Package.Current.PublisherDisplayName}";
        } // end constructor AboutAppPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            HyperlinkButtonGiteeRepo.Content = resourceLoader.GetString("GiteeRepo");
            HyperlinkButtonGitHubRepo.Content = resourceLoader.GetString("GitHubRepo");
            HyperlinkButtonHome.Content = $"{Package.Current.DisplayName} {resourceLoader.GetString("Site")}";
            HyperlinkButtonLicense.Content = resourceLoader.GetString("License");
            HyperlinkButtonReleaseNotes.Content = resourceLoader.GetString("ReleaseNotes");
            HyperlinkButtonUserManual.Content = resourceLoader.GetString("UserManual");
            HyperlinkButtonViewIssues.Content = resourceLoader.GetString("ViewIssues");
            RunAcknowledgementAppIcon.Text = resourceLoader.GetString("AcknowledgementAppIcon");
            RunAcknowledgementInspiration.Text = resourceLoader.GetString("AcknowledgementInspiration");
            TextBlockVersion.Text =
                $"{resourceLoader.GetString("Version")} {(Application.Current as App)?.AppVersion} ({Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision})";
        } // end method UpdateUiText

        #endregion Methods
    } // end class AboutAppPage
} // end namespace PaimonTray.Views