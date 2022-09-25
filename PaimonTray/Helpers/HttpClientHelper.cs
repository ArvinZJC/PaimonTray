using Serilog;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PaimonTray.Helpers
{
    /// <summary>
    /// The HTTP client helper.
    /// </summary>
    public class HttpClientHelper
    {
        #region Constants

        /// <summary>
        /// The CN server's dynamic secret salt.
        /// </summary>
        private const string DynamicSecretSaltServerCn = "xV8v4Qu54lUKrEYFZkJhB8cuOh9Asafs";

        /// <summary>
        /// The global server's dynamic secret salt.
        /// </summary>
        private const string DynamicSecretSaltServerGlobal = "okr4obncj8bw5a65hbnn5oo6ixjc3l9w";

        /// <summary>
        /// The app version header name.
        /// </summary>
        private const string HeaderNameAppVersion = "x-rpc-app_version";

        /// <summary>
        /// The client type header name.
        /// </summary>
        private const string HeaderNameClientType = "x-rpc-client_type";

        /// <summary>
        /// The cookie header name.
        /// </summary>
        private const string HeaderNameCookie = "Cookie";

        /// <summary>
        /// The dynamic secret header name.
        /// </summary>
        private const string HeaderNameDynamicSecret = "DS";

        /// <summary>
        /// The app version header value for the CN server.
        /// </summary>
        private const string HeaderValueAppVersionServerCn = "2.37.1";

        /// <summary>
        /// The app version header value for the global server.
        /// </summary>
        private const string HeaderValueAppVersionServerGlobal = "2.19.0";

        /// <summary>
        /// The client type header value for the CN server.
        /// </summary>
        private const string HeaderValueClientTypeServerCn = "5";

        /// <summary>
        /// The client type header value for the global server.
        /// </summary>
        private const string HeaderValueClientTypeServerGlobal = "2";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initialise the HTTP client helper.
        /// NOTE: Must be done before any other parts requiring the HTTP client.
        /// </summary>
        public HttpClientHelper()
        {
            _lazyHttpClient = new Lazy<HttpClient>(() => new HttpClient(new HttpClientHandler { UseCookies = false }));
            _random = new Random();
        } // end constructor HttpClientHelper

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Ensure disposing.
        /// </summary>
        ~HttpClientHelper()
        {
            _lazyHttpClient.Value.Dispose();
            _lazyHttpClient = null;
            _random = null;
        } // end destructor

        #endregion Destructor

        #region Fields

        /// <summary>
        /// The HTTP client's lazy initialisation.
        /// NOTE: System.Net.Http is used rather than the recommended Windows.Web.Http because of disabling automatic cookies handling, which the latter seems to perform badly.
        /// </summary>
        private Lazy<HttpClient> _lazyHttpClient;

        /// <summary>
        /// The pseudo-random number generator.
        /// </summary>
        private Random _random;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Generate a dynamic secret.
        /// </summary>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="query">The query.</param>
        /// <returns>The dynamic secret.</returns>
        private string GenerateDynamicSecret(bool isServerCn, string query)
        {
            var dynamicSecretSalt = isServerCn ? DynamicSecretSaltServerCn : DynamicSecretSaltServerGlobal;
            using var md5 = MD5.Create();
            var randomInt = _random.Next(100000, 200000);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (string.IsNullOrWhiteSpace(query) || !query.Contains('=')) Log.Warning($"Invalid query ({query}).");

            return
                $"{timestamp},{randomInt},{Convert.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes($"salt={dynamicSecretSalt}&t={timestamp}&r={randomInt}&b=&q={query}"))).ToLowerInvariant()}";
        } // end method GenerateDynamicSecret

        /// <summary>
        /// Send an HTTP GET request.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="url">The URL.</param>
        /// <returns>The HTTP response message content, or <c>null</c> if the operation fails.</returns>
        public async Task<string> SendGetRequestAsync(string cookies, bool isServerCn, string url)
        {
            return await SendGetRequestAsync(cookies, isServerCn, false, null, url);
        } // end method SendGetRequestAsync(String, Boolean, String)

        /// <summary>
        /// Send an HTTP GET request.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="needDynamicSecret">A flag indicating if the request needs the dynamic secret.</param>
        /// <param name="query">The query.</param>
        /// <param name="url">The URL.</param>
        /// <returns>The HTTP response message content, or <c>null</c> if the operation fails.</returns>
        public async Task<string> SendGetRequestAsync(string cookies, bool isServerCn, bool needDynamicSecret,
            string query, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http"))
            {
                Log.Warning($"Invalid URL ({url}).");
                return null;
            } // end if

            var httpClient = _lazyHttpClient.Value;
            var httpClientHeaders = httpClient.DefaultRequestHeaders;

            httpClientHeaders.Clear(); // Clear first.

            if (!httpClientHeaders.TryAddWithoutValidation(HeaderNameCookie, cookies))
            {
                Log.Warning($"Failed to add the cookie header (cookies: {cookies}).");
                return null;
            } // end if

            if (needDynamicSecret)
            {
                var headerValueAppVersion =
                    isServerCn ? HeaderValueAppVersionServerCn : HeaderValueAppVersionServerGlobal;

                if (!httpClientHeaders.TryAddWithoutValidation(HeaderNameAppVersion, headerValueAppVersion))
                {
                    Log.Warning($"Failed to add the app version header (app version: {headerValueAppVersion}).");
                    return null;
                } // end if

                var headerValueClientType =
                    isServerCn ? HeaderValueClientTypeServerCn : HeaderValueClientTypeServerGlobal;

                if (!httpClientHeaders.TryAddWithoutValidation(HeaderNameClientType, headerValueClientType))
                {
                    Log.Warning($"Failed to add the client type header (client type: {headerValueClientType}).");
                    return null;
                } // end if

                var headerValueDynamicSecret = GenerateDynamicSecret(isServerCn, query);

                if (!httpClientHeaders.TryAddWithoutValidation(HeaderNameDynamicSecret, headerValueDynamicSecret))
                {
                    Log.Warning(
                        $"Failed to add the dynamic secret header (dynamic secret: {headerValueDynamicSecret}).");
                    return null;
                } // end if
            } // end if

            HttpResponseMessage httpResponseMessage;

            try
            {
                httpResponseMessage = await httpClient.GetAsync(new Uri(url));
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception exception)
            {
                Log.Error($"The HTTP GET request was unsuccessful (URL: {url}).");
                Log.Error(exception.ToString());
                return null;
            } // end try...catch

            return await httpResponseMessage.Content.ReadAsStringAsync();
        } // end method SendGetRequestAsync(String, Boolean, Boolean, String, String)

        #endregion Methods
    } // end class HttpClientHelper
} // end namespace PaimonTray.Helpers