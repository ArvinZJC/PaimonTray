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
        /// The referer header name.
        /// </summary>
        private const string HeaderNameReferer = "Referer";

        /// <summary>
        /// The app version header value for the CN server.
        /// </summary>
        private const string HeaderValueAppVersionServerCn = "2.42.1";

        /// <summary>
        /// The app version header value for the global server.
        /// </summary>
        private const string HeaderValueAppVersionServerGlobal = "2.24.1";

        /// <summary>
        /// The client type header value for the CN server.
        /// </summary>
        private const string HeaderValueClientTypeServerCn = "5";

        /// <summary>
        /// The client type header value for the global server.
        /// </summary>
        private const string HeaderValueClientTypeServerGlobal = "2";

        /// <summary>
        /// The referer header value for the CN server.
        /// </summary>
        private const string HeaderValueRefererServerCn = "https://webstatic.mihoyo.com/";

        /// <summary>
        /// The referer header value for the global server.
        /// </summary>
        private const string HeaderValueRefererServerGlobal = "https://webstatic-sea.hoyolab.com";

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Initialise the HTTP client helper.
        /// NOTE: Must be done before any other parts requiring the HTTP client.
        /// </summary>
        public HttpClientHelper()
        {
            _lazyHttpClient = new Lazy<HttpClient>(() => new HttpClient(new HttpClientHandler { UseCookies = false }));
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
        } // end destructor

        #endregion Destructor

        #region Fields

        /// <summary>
        /// The HTTP client's lazy initialisation.
        /// NOTE: System.Net.Http is used rather than the recommended Windows.Web.Http because of disabling automatic cookies handling, which the latter seems to perform badly.
        /// </summary>
        private Lazy<HttpClient> _lazyHttpClient;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Generate a dynamic secret.
        /// </summary>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="query">The query.</param>
        /// <returns>The dynamic secret, or <c>null</c> if the operation fails.</returns>
        private static string GenerateDynamicSecret(bool isServerCn, string query)
        {
            var randomInt = Random.Shared.Next(100000, 200000);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (string.IsNullOrWhiteSpace(query) || !query.Contains('=')) Log.Warning($"Invalid query ({query}).");

            try
            {
                return $"{timestamp}," +
                       $"{randomInt}," +
                       $"{Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(
                           $"salt={(isServerCn ? DynamicSecretSaltServerCn : DynamicSecretSaltServerGlobal)}&t={timestamp}&r={randomInt}&b=&q={query}"
                       ))).ToLowerInvariant()}";
            }
            catch (Exception exception)
            {
                Log.Error("Failed to generate a dynamic secret.");
                App.LogException(exception);
                return null;
            } // end try...catch
        } // end method GenerateDynamicSecret

        /// <summary>
        /// Send a GET request.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="url">The full URL.</param>
        /// <param name="needDynamicSecret">A flag indicating if the request needs the dynamic secret. Default: <c>false</c>.</param>
        /// <param name="query">The query for generating a dynamic secret. Default: <c>null</c>.</param>
        /// <returns>The HTTP response message content, or <c>null</c> if the operation fails.</returns>
        public async Task<string> GetAsync(string cookies, bool isServerCn, string url,
            bool needDynamicSecret = false, string query = null)
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

            var headerValueReferer = isServerCn ? HeaderValueRefererServerCn : HeaderValueRefererServerGlobal;

            if (!httpClientHeaders.TryAddWithoutValidation(HeaderNameReferer, headerValueReferer))
            {
                Log.Warning($"Failed to add the referer header (referer: {headerValueReferer}).");
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

            try
            {
                var httpResponseMessage = await httpClient.GetAsync(new Uri(url));

                httpResponseMessage.EnsureSuccessStatusCode();
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                Log.Error($"The GET request was unsuccessful (URL: {url}).");
                App.LogException(exception);
                return null;
            } // end try...catch
        } // end method GetAsync

        #endregion Methods
    } // end class HttpClientHelper
} // end namespace PaimonTray.Helpers