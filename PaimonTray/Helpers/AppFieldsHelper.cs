namespace PaimonTray.Helpers
{
    /// <summary>
    /// The app fields helper.
    /// </summary>
    public static class AppFieldsHelper
    {
        #region Fields

        /// <summary>
        /// The alpha build.
        /// </summary>
        public static readonly string BuildAlpha = "alpha";

        /// <summary>
        /// The beta build.
        /// </summary>
        public static readonly string BuildBeta = "beta";

        /// <summary>
        /// The stable build.
        /// </summary>
        public static readonly string BuildStable = "stable";

        /// <summary>
        /// The code of the glyph Accept.
        /// </summary>
        public static readonly string GlyphAccept = "\xE8FB";

        /// <summary>
        /// The code of the glyph Diagnostic.
        /// </summary>
        public static readonly string GlyphDiagnostic = "\xE9D9";

        /// <summary>
        /// The code of the glyph Dictionary.
        /// </summary>
        public static readonly string GlyphDictionary = "\xE82D";

        /// <summary>
        /// The code of the glyph Help.
        /// </summary>
        public static readonly string GlyphHelp = "\xE897";

        /// <summary>
        /// The code of the glyph Info.
        /// </summary>
        public static readonly string GlyphInfo = "\xE946";

        /// <summary>
        /// The code of the glyph KnowledgeArticle.
        /// </summary>
        public static readonly string GlyphKnowledgeArticle = "\xF000";

        /// <summary>
        /// The code of the glyph Processing.
        /// </summary>
        public static readonly string GlyphProcessing = "\xE9F5";

        /// <summary>
        /// The code of the glyph SwitchUser.
        /// </summary>
        public static readonly string GlyphSwitchUser = "\xE748";

        /// <summary>
        /// The code of the glyph Trackers.
        /// </summary>
        public static readonly string GlyphTrackers = "\xEADF";

        /// <summary>
        /// The code of the glyph UpdateRestore.
        /// </summary>
        public static readonly string GlyphUpdateRestore = "\xE777";

        /// <summary>
        /// The code of the glyph Website.
        /// </summary>
        public static readonly string GlyphWebsite = "\xEB41";

        /// <summary>
        /// The info bar's bottom margin.
        /// </summary>
        public static readonly int InfoBarMarginBottom = 4;

        /// <summary>
        /// The greeting notification tag.
        /// </summary>
        public static readonly string TagNotificationGreeting = "NotificationGreeting";

        /// <summary>
        /// The representation of the unknown value.
        /// </summary>
        public static readonly string Unknown = "-";

        /// <summary>
        /// The app icon file URI.
        /// </summary>
        public static readonly string UriAppIcon = "ms-appx:///Assets/AppIcon/AppIcon.ico";

        /// <summary>
        /// The app icon image URI.
        /// </summary>
        public static readonly string UriImageAppIcon = "ms-appx:///Assets/AppIcon/AppIcon.png";

        /// <summary>
        /// The daily commissions image URI.
        /// </summary>
        public static readonly string UriImageCommissionsDaily = "ms-appx:///Assets/DailyCommissions.png";

        /// <summary>
        /// The Realm Currency image URI.
        /// </summary>
        public static readonly string UriImageCurrencyRealm = "ms-appx:///Assets/RealmCurrency.png";

        /// <summary>
        /// The Trounce Domains image URI.
        /// </summary>
        public static readonly string UriImageDomainsTrounce = "ms-appx:///Assets/TrounceDomains.png";

        /// <summary>
        /// The Original Resin image URI.
        /// </summary>
        public static readonly string UriImageResinOriginal = "ms-appx:///Assets/OriginalResin.png";

        /// <summary>
        /// The Parametric Transformer image URI.
        /// </summary>
        public static readonly string UriImageTransformerParametric = "ms-appx:///Assets/ParametricTransformer.png";

        /// <summary>
        /// The Paimon Surprise image URI.
        /// </summary>
        public static readonly string UriImagePaimonSurprise = "ms-appx:///Assets/PaimonSurprise.png";

        /// <summary>
        /// The URI of the system settings page configuring notifications.
        /// </summary>
        public static readonly string UriSystemSettingsNotifications = "ms-settings:notifications";

        /// <summary>
        /// The URI of the system settings page configuring the startup apps.
        /// </summary>
        public static readonly string UriSystemSettingsStartupApps = "ms-settings:startupapps";

        /// <summary>
        /// The app icon source URL.
        /// </summary>
        public static readonly string UrlAppIconSource = "https://www.pixiv.net/en/artworks/92415888";

        /// <summary>
        /// The URL of the user manual section instructing how to get your cookies.
        /// </summary>
        public static readonly string UrlCookiesHowToGet = "https://paimon.swo.moe/#how-to-get-my-cookie";

        /// <summary>
        /// The app's Gitee repository URL.
        /// </summary>
        public static readonly string UrlGiteeRepo = "https://gitee.com/ArvinZJC/PaimonTray";

        /// <summary>
        /// The app's GitHub repository URL.
        /// </summary>
        public static readonly string UrlGitHubRepo = "https://github.com/ArvinZJC/PaimonTray";

        /// <summary>
        /// The URL of the GitHub repository primarily referred for API uses.
        /// </summary>
        public static readonly string UrlGitHubRepoApiUsesPrimary = "https://github.com/thesadru/genshin.py";

        /// <summary>
        /// The URL of the GitHub repository secondarily referred for API uses.
        /// </summary>
        public static readonly string UrlGitHubRepoApiUsesSecondary = "https://github.com/DGP-Studio";

        /// <summary>
        /// The URL of the GitHub repository inspiring the app.
        /// </summary>
        public static readonly string UrlGitHubRepoInspiration = "https://github.com/spencerwooo/PaimonMenuBar";

        /// <summary>
        /// The app's GitHub repository's issues URL.
        /// </summary>
        public static readonly string UrlGitHubRepoIssues = "https://github.com/ArvinZJC/PaimonTray/issues";

        /// <summary>
        /// The URL of the app licence on GitHub.
        /// </summary>
        public static readonly string UrlGitHubRepoLicence = "https://github.com/ArvinZJC/PaimonTray/blob/main/LICENCE";

        /// <summary>
        /// The app's GitHub repository's releases URL.
        /// </summary>
        public static readonly string UrlGitHubRepoReleases = "https://github.com/ArvinZJC/PaimonTray/releases";

        /// <summary>
        /// The app's GitHub repository's specific release base URL.
        /// </summary>
        public static readonly string
            UrlBaseGitHubRepoRelease = $"{UrlGitHubRepoReleases}/tag/v"; // Need to put after the referred field.

        /// <summary>
        /// The URL for downloading the Microsoft Edge WebView2 Runtime.
        /// </summary>
        public static readonly string UrlWebView2Runtime = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

        /// <summary>
        /// The min beta build version.
        /// </summary>
        public static readonly int VersionBuildBetaMin = 100;

        /// <summary>
        /// The min stable build version.
        /// </summary>
        public static readonly int VersionBuildStableMin = 200;

        #endregion Fields
    } // end class AppFieldsHelper
} // end namespace PaimonTray.Helpers