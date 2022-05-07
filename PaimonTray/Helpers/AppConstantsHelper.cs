namespace PaimonTray.Helpers
{
    /// <summary>
    /// The app constants helper.
    /// </summary>
    public class AppConstantsHelper
    {
        #region Enumerations

        /// <summary>
        /// The game server.
        /// </summary>
        public enum Server
        {
            Cn,
            Global
        } // end enum Server

        #endregion Enumerations

        #region Constants

        /// <summary>
        /// The height of the page for adding an account using the alternative login method.
        /// </summary>
        public const int AddAccountPageLoginAlternativeHeight = 500;

        /// <summary>
        /// The width of the page for adding an account using the alternative login method.
        /// </summary>
        public const int AddAccountPageLoginAlternativeWidth = 500;

        /// <summary>
        /// The height of the page for adding a CN server account using the web page login method.
        /// </summary>
        public const int AddAccountPageLoginWebPageCnHeight = 800;

        /// <summary>
        /// The width of the page for adding a CN server account using the web page login method.
        /// </summary>
        public const int AddAccountPageLoginWebPageCnWidth = 680;

        /// <summary>
        /// The height of the page for adding a global server account using the web page login method.
        /// </summary>
        public const int AddAccountPageLoginWebPageGlobalHeight = 820;

        /// <summary>
        /// The width of the page for adding a global server account using the web page login method.
        /// </summary>
        public const int AddAccountPageLoginWebPageGlobalWidth = 800;

        /// <summary>
        /// The app icon's author.
        /// </summary>
        public const string AppIconAuthor = "Chawong";

        /// <summary>
        /// The ID cookie name.
        /// </summary>
        public const string CookieNameId = "account_id";

        /// <summary>
        /// The token cookie name.
        /// </summary>
        public const string CookieNameToken = "cookie_token";

        /// <summary>
        /// The name of the GitHub repository inspiring the app.
        /// </summary>
        public const string GitHubRepoInspirationName = "PaimonMenuBar";

        /// <summary>
        /// The code of the glyph Dictionary.
        /// </summary>
        public const string GlyphDictionary = "\xE82D";

        /// <summary>
        /// The code of the glyph Info.
        /// </summary>
        public const string GlyphInfo = "\xE946";

        /// <summary>
        /// The code of the glyph Refresh.
        /// </summary>
        public const string GlyphRefresh = "\xE72C";

        /// <summary>
        /// The code of the glyph Repair.
        /// </summary>
        public const string GlyphRepair = "\xE90F";

        /// <summary>
        /// The code of the glyph Status Circle Question Mark.
        /// </summary>
        public const string GlyphStatusCircleQuestionMark = "\xF142";

        /// <summary>
        /// The main window's position offset.
        /// </summary>
        public const int MainWindowPositionOffset = 12;

        /// <summary>
        /// The main window's side length offset.
        /// </summary>
        public const int MainWindowSideLengthOffset = 2;

        /// <summary>
        /// The greeting notification tag.
        /// </summary>
        public const string TagNotificationGreeting = "NotificationGreeting";

        /// <summary>
        /// The app icon (.ico) URI.
        /// </summary>
        public const string UriAppIcon = "ms-appx:///Assets/AppIcon/AppIcon.ico";

        /// <summary>
        /// The app icon image (.png) URI.
        /// </summary>
        public const string UriAppIconImage = "ms-appx:///Assets/AppIcon/AppIcon.png";

        /// <summary>
        /// The URI of the system settings page configuring notifications.
        /// </summary>
        public const string UriSystemSettingsNotifications = "ms-settings:notifications";

        /// <summary>
        /// The URI of the system settings page configuring the startup apps.
        /// </summary>
        public const string UriSystemSettingsStartupApps = "ms-settings:startupapps";

        /// <summary>
        /// The app icon source URL.
        /// </summary>
        public const string UrlAppIconSource = "https://www.pixiv.net/en/artworks/92415888";

        /// <summary>
        /// The HoYoLAB cookies URL.
        /// </summary>
        public const string UrlCookiesHoYoLab = "https://www.hoyolab.com";

        /// <summary>
        /// The miHoYo cookies URL.
        /// </summary>
        public const string UrlCookiesMiHoYo = "https://bbs.mihoyo.com";

        /// <summary>
        /// The app's GitHub repository URL.
        /// </summary>
        public const string UrlGitHubRepo = "https://github.com/ArvinZJC/PaimonTray";

        /// <summary>
        /// The URL of the GitHub repository inspiring the app.
        /// </summary>
        public const string UrlGitHubRepoInspiration = "https://github.com/spencerwooo/PaimonMenuBar";

        /// <summary>
        /// The app's GitHub repository's issues URL.
        /// </summary>
        public const string UrlGitHubRepoIssues = "https://github.com/ArvinZJC/PaimonTray/issues";

        /// <summary>
        /// The URL of the app license on GitHub.
        /// </summary>
        public const string UrlGitHubRepoLicense = "https://github.com/ArvinZJC/PaimonTray/blob/main/LICENSE";

        /// <summary>
        /// The app's GitHub repository's releases URL.
        /// </summary>
        public const string UrlGitHubRepoReleases = "https://github.com/ArvinZJC/PaimonTray/releases";

        /// <summary>
        /// The URL of the user manual section instructing how to get your cookies.
        /// </summary>
        public const string UrlHowToGetCookies = "https://paimon.swo.moe/#how-to-get-my-cookie";

        /// <summary>
        /// The URL for indicating the success of logging into miHoYo.
        /// </summary>
        public const string UrlLoginEndMiHoYo = "https://bbs.mihoyo.com/ys/accountCenter/postList?";

        /// <summary>
        /// The URL for logging into HoYoLAB.
        /// </summary>
        public const string UrlLoginHoYoLab = "https://www.hoyolab.com/home";

        /// <summary>
        /// The URL for logging into miHoYo.
        /// </summary>
        public const string UrlLoginMiHoYo = "https://bbs.mihoyo.com/ys/accountCenter";

        /// <summary>
        /// The URL for downloading the Microsoft Edge WebView2 Runtime.
        /// </summary>
        public const string UrlWebView2Runtime = "https://go.microsoft.com/fwlink/p/?LinkId=2124703";

        /// <summary>
        /// The minimum beta revision version.
        /// </summary>
        public const int VersionRevisionBetaMin = 100;

        /// <summary>
        /// The stable revision version.
        /// </summary>
        public const int VersionRevisionStable = 200;

        #endregion Constants
    } // end class AppConstantsHelper
} // end namespace PaimonTray.Helpers