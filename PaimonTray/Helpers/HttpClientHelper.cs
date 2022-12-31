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
        /// The challenge header name.
        /// </summary>
        private const string HeaderNameChallenge = "x-rpc-challenge";

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
        private const string HeaderValueAppVersionServerCn = "2.43.1";

        /// <summary>
        /// The app version header value for the global server.
        /// </summary>
        private const string HeaderValueAppVersionServerGlobal = "2.25.0";

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
        /// <param name="body">The body. Default: <c>null</c>.</param>
        /// <param name="query">The query. Default: <c>null</c>.</param>
        /// <returns>The dynamic secret, or <c>null</c> if the operation fails.</returns>
        private static async Task<string> GenerateDynamicSecretAsync(bool isServerCn, HttpContent body = null,
            string query = null)
        {
            var randomInt = Random.Shared.Next(100000, 200000);
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (!string.IsNullOrEmpty(query))
            {
                query = query.Trim();

                var indexEqualSign = query.IndexOf('=');

                if (indexEqualSign is -1 or 0 || indexEqualSign == query.Length - 1)
                {
                    Log.Warning($"Invalid query ({query}).");
                    return null;
                } // end if
            } // end if

            try
            {
                return $"{timestamp}," +
                       $"{randomInt}," +
                       $"{Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(
                           $"salt={(isServerCn ? DynamicSecretSaltServerCn : DynamicSecretSaltServerGlobal)}&t={timestamp}&r={randomInt}&b={(body is null ? string.Empty : await body.ReadAsStringAsync())}&q={query}"
                       ))).ToLowerInvariant()}";
            }
            catch (Exception exception)
            {
                Log.Error("Failed to generate a dynamic secret.");
                App.LogException(exception);
                return null;
            } // end try...catch
        } // end method GenerateDynamicSecretAsync

        /// <summary>
        /// Send a GET request.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="url">The full URL.</param>
        /// <param name="challenge">The validated GeeTest challenge.</param>
        /// <param name="needDynamicSecret">A flag indicating if the request needs the dynamic secret. Default: <c>false</c>.</param>
        /// <param name="query">The query for generating a dynamic secret. Default: <c>null</c>.</param>
        /// <returns>The HTTP response body, or <c>null</c> if the operation fails.</returns>
        public async Task<string> GetAsync(string cookies, bool isServerCn, string url, string challenge = null,
            bool needDynamicSecret = false, string query = null)
        {
            return await SendRequestAsync(null, challenge, cookies, true, isServerCn, needDynamicSecret, query, url);
        } // end method GetAsync

        /// <summary>
        /// Send a POST request.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="url">The full URL.</param>
        /// <param name="body">The body.</param>
        /// <param name="needDynamicSecret">A flag indicating if the request needs the dynamic secret. Default: <c>false</c>.</param>
        /// <returns>The HTTP response body, or <c>null</c> if the operation fails.</returns>
        public async Task<string> PostAsync(string cookies, bool isServerCn, string url, HttpContent body = null,
            bool needDynamicSecret = false)
        {
            return await SendRequestAsync(body, null, cookies, false, isServerCn, needDynamicSecret, null, url);
        } // end method PostAsync

        /// <summary>
        /// Send a GET/POST request.
        /// </summary>
        /// <param name="body">The body. Default: <c>null</c>.</param>
        /// <param name="challenge">The validated GeeTest challenge.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="isGet">A flag indicating whether the HTTP request method is GET or POST.</param>
        /// <param name="isServerCn">A flag indicating if an account belongs to the CN server.</param>
        /// <param name="needDynamicSecret">A flag indicating if the request needs the dynamic secret.</param>
        /// <param name="query">The query for generating a dynamic secret. Default: <c>null</c>.</param>
        /// <param name="url">The full URL.</param>
        /// <returns>The HTTP response body, or <c>null</c> if the operation fails.</returns>
        private async Task<string> SendRequestAsync(HttpContent body, string challenge, string cookies, bool isGet,
            bool isServerCn, bool needDynamicSecret, string query, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !url.StartsWith("http"))
            {
                Log.Warning($"Invalid URL ({url}).");
                return null;
            } // end if

            var httpClient = _lazyHttpClient.Value;
            var httpClientHeaders = httpClient.DefaultRequestHeaders;

            httpClientHeaders.Clear(); // Clear first.

            try
            {
                httpClientHeaders.Add(HeaderNameCookie, cookies);
                httpClientHeaders.Add(HeaderNameReferer,
                    isServerCn ? HeaderValueRefererServerCn : HeaderValueRefererServerGlobal);

                if (!string.IsNullOrWhiteSpace(challenge)) httpClientHeaders.Add(HeaderNameChallenge, challenge);

                if (needDynamicSecret)
                {
                    var dynamicSecret = await GenerateDynamicSecretAsync(isServerCn, body, query);

                    if (string.IsNullOrWhiteSpace(dynamicSecret))
                    {
                        Log.Warning($"Invalid dynamic secret ({dynamicSecret}).");
                        return null;
                    } // end if

                    httpClientHeaders.Add(HeaderNameDynamicSecret, dynamicSecret);
                    httpClientHeaders.Add(HeaderNameAppVersion,
                        isServerCn ? HeaderValueAppVersionServerCn : HeaderValueAppVersionServerGlobal);
                    httpClientHeaders.Add(HeaderNameClientType,
                        isServerCn ? HeaderValueClientTypeServerCn : HeaderValueClientTypeServerGlobal);
                } // end if

                var uri = new Uri(url); // Get the URI from the URL first.
                var httpResponseMessage =
                    isGet ? await httpClient.GetAsync(uri) : await httpClient.PostAsync(uri, body);

                httpResponseMessage.EnsureSuccessStatusCode();
                return await httpResponseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                Log.Error($"The {(isGet ? "GET" : "POST")} request was unsuccessful (URL: {url}).");
                App.LogException(exception);
                return null;
            } // end try...catch
        } // end method SendRequestAsync

        #endregion Methods
    } // end class HttpClientHelper
} // end namespace PaimonTray.Helpers