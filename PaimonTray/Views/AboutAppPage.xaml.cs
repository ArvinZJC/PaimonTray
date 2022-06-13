using Microsoft.UI.Xaml;
using System;
using Windows.ApplicationModel;
using PaimonTray.Helpers;

namespace PaimonTray.Views
{
    /// <summary>
    /// The app introduction page.
    /// </summary>
    public sealed partial class AboutAppPage
    {
        #region Fields

        /// <summary>
        /// The app.
        /// </summary>
        private readonly App _app;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialise the app introduction page.
        /// </summary>
        public AboutAppPage()
        {
            _app = Application.Current as App;
            InitializeComponent();
            UpdateUiText();

            HyperlinkButtonReleaseNotes.NavigateUri =
                new Uri(_app?.UrlGitHubRepoRelease ?? AppConstantsHelper.UrlGitHubRepoReleases);
            TextBlockCopyright.Text = $"© {DateTime.Now.Year} {Package.Current.PublisherDisplayName}";
        } // end constructor AboutAppPage

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Update the UI text during the initialisation process.
        /// </summary>
        private void UpdateUiText()
        {
            var packageVersion = Package.Current.Id.Version;
            var resourceLoader = _app.SettingsH.ResLoader;

            HyperlinkButtonGiteeRepo.Content = resourceLoader?.GetString("GiteeRepo");
            HyperlinkButtonGitHubRepo.Content = resourceLoader?.GetString("GitHubRepo");
            HyperlinkButtonAppSite.Content = $"{Package.Current.DisplayName} {resourceLoader?.GetString("Site")}";
            HyperlinkButtonIssues.Content = resourceLoader?.GetString("Issues");
            HyperlinkButtonLicence.Content = resourceLoader?.GetString("Licence");
            HyperlinkButtonReleaseNotes.Content = resourceLoader?.GetString("ReleaseNotes");
            HyperlinkButtonUserManual.Content = resourceLoader?.GetString("UserManual");
            RunAcknowledgementApiUses.Text = resourceLoader?.GetString("AcknowledgementApiUses");
            RunAcknowledgementApiUsesAnd.Text = resourceLoader?.GetString("And");
            RunAcknowledgementAppIcon.Text = resourceLoader?.GetString("AcknowledgementAppIcon");
            RunAcknowledgementInspiration.Text = resourceLoader?.GetString("AcknowledgementInspiration");
            RunAppIconAuthor.Text = resourceLoader?.GetString("AppIconAuthor");
            RunNameGitHubRepoApiUsesPrimary.Text = resourceLoader?.GetString("NameGitHubRepoApiUsesPrimary");
            RunNameGitHubRepoApiUsesSecondary.Text = resourceLoader?.GetString("NameGitHubRepoApiUsesSecondary");
            RunNameGitHubRepoInspiration.Text = resourceLoader?.GetString("NameGitHubRepoInspiration");
            TextBlockVersion.Text =
                $"{resourceLoader?.GetString("Version")} {_app.AppVersion} ({packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision})";
        } // end method UpdateUiText

        #endregion Methods
    } // end class AboutAppPage
} // end namespace PaimonTray.Views