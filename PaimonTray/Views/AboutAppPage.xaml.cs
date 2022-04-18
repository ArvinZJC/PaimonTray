using PaimonTray.Helpers;
using System;
using Windows.ApplicationModel;

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

            HyperlinkButtonHome.Content = $"{Package.Current.DisplayName} site";
            TextBlockCopyright.Text = $"© {DateTime.Now.Year} {Package.Current.PublisherDisplayName}";
            TextBlockVersion.Text =
                $"Version {GetAppVersion()} ({Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision})";
        } // end constructor AboutAppPage

        #endregion Constructors

        #region Methods

        // Get the app version from the package version.
        private static string GetAppVersion()
        {
            var suffix = Package.Current.Id.Version.Revision switch
            {
                < AppConstantsHelper.RevisionVersionBetaMin => $"-alpha.{Package.Current.Id.Version.Revision + 1}",
                < AppConstantsHelper.RevisionVersionStable =>
                    $"-beta.{Package.Current.Id.Version.Revision - AppConstantsHelper.RevisionVersionBetaMin + 1}",
                _ => string.Empty
            };

            return
                $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}{suffix}";
        } // end method GetAppVersion

        #endregion Methods
    } // end class AboutAppPage
} // end namespace PaimonTray.Views