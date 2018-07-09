using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using PushPay.Models;
using PushPay.Sets;

namespace PushPay {
    public class PushPayClient {
        private readonly OrganizationSet _organizations;

        public PushPayClient(PushPayOptions options, OAuthToken token) {
            _organizations = new OrganizationSet(options, token);
        }

        public OrganizationSet Organizations {
            get {
                return _organizations;
            }
        }

        /// <summary>
        /// Gets the url that should be sent via browser to authorize the push pay user and get consent
        /// </summary>
        /// <param name="options">Options to set things like client id and secret</param>
        /// <param name="returnUrl">The url to return to when consent is chosen</param>
        /// <param name="scopes">The scopes for the authorization to identify the rights of subsequent calls. NOTE: list should be space separated</param>
        /// <returns>A url to be sent via browser for the user to give consent to application</returns>
        public static Uri GetAuthorizationUrl(PushPayOptions options, string returnUrl, string scopes) {
            System.Text.StringBuilder loginUrl = new System.Text.StringBuilder();
            loginUrl.Append(options.IsStaging ? "https://auth.pushpay.com/pushpay-sandbox/oauth/authorize" : "https://auth.pushpay.com/pushpay/oauth/authorize");
            loginUrl.Append($"?client_id={options.ClientID}&response_type=code&redirect_uri={returnUrl}&scope={scopes}");
            return new Uri(loginUrl.ToString());
        }

        /// <summary>
        /// Request an access token from PushPay after going through the authorization
        /// </summary>
        /// <param name="options">Options to set things like client id and secret</param>
        /// <param name="returnUrl">This url should be the same as the one passed into GetAuthorizationUrl()</param>
        /// <param name="code">The authorization code received from PushPay after authorization</param>
        /// <returns>An OAuth Token object to use for subsequent requests</returns>
        public static async Task<IPushPayResponse<OAuthToken>> RequestAccessTokenAsync(PushPayOptions options, string returnUrl, string code) {
            using (var httpClient = new HttpClient()) {
                var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes($"{options.ClientID}:{options.ClientSecret}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", returnUrl)
                });

                var url = new Uri(options.IsStaging ? "https://auth.pushpay.com/pushpay-sandbox" : "https://auth.pushpay.com/pushpay");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(toEncodeAsBytes, 0, toEncodeAsBytes.Length));
                var response = await httpClient.PostAsync($"{url}/oauth/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var pushPayResponse = new PushPayResponse<OAuthToken> {
                    StatusCode = response.StatusCode,
                    RequestValue = Newtonsoft.Json.JsonConvert.SerializeObject(content)
                };

                if (!string.IsNullOrEmpty(responseContent) && responseContent.Contains("error")) {
                    var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                    pushPayResponse.ErrorMessage = responseError.error_message;
                }
                else {
                    pushPayResponse.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthToken>(responseContent);
                }

                return pushPayResponse; 
            }
        }

        public static async Task<IPushPayResponse<OAuthToken>> RefreshAccessTokenAsync(PushPayOptions options, string refreshToken) {
            using (var httpClient = new HttpClient()) {
                var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes($"{options.ClientID}:{options.ClientSecret}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                });

                var url = new Uri(options.IsStaging ? "https://auth.pushpay.com/pushpay-sandbox" : "https://auth.pushpay.com/pushpay");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(toEncodeAsBytes, 0, toEncodeAsBytes.Length));
                var response = await httpClient.PostAsync($"{url}/oauth/token", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var pushPayResponse = new PushPayResponse<OAuthToken> {
                    StatusCode = response.StatusCode,
                    RequestValue = Newtonsoft.Json.JsonConvert.SerializeObject(content)
                };

                if (!string.IsNullOrEmpty(responseContent) && responseContent.Contains("error")) {
                    var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
                    pushPayResponse.ErrorMessage = responseError.error_message;
                }
                else {
                    pushPayResponse.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthToken>(responseContent);
                }

                return pushPayResponse;
            }
        }
    }
}
