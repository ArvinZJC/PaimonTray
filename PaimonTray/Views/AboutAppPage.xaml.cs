using Microsoft.UI.Xaml;
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
                $"Version {(Application.Current as App)?.AppVersion} ({Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}.{Package.Current.Id.Version.Revision})";
        } // end constructor AboutAppPage

        #endregion Constructors
    } // end class AboutAppPage
} // end namespace PaimonTray.Views