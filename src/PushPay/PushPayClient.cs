using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace PushPay {
    public class PushPayClient {
        /// <summary>
        /// Gets the url that should be sent via browser to authorize the push pay user and get consent
        /// </summary>
        /// <param name="options">Options to set things like client id and secret</param>
        /// <param name="returnUrl">The url to return to when consent is chosen</param>
        /// <param name="scopes">The scopes for the authorization to identify the rights of subsequent calls. NOTE: list should be space separated</param>
        /// <returns>A url to be sent via browser for the user to give consent to application</returns>
        public static Uri GetAuthorizationUrl(PushPayClientOptions options, string returnUrl, string scopes) {
            System.Text.StringBuilder loginUrl = new System.Text.StringBuilder();
            loginUrl.Append(options.IsStaging ? "https://auth.pushpay.com/pushpay-sandbox/oauth/authorize" : "https://auth.pushpay.com/pushpay/oauth/authorize");
            loginUrl.Append($"?client_id={options.ClientID}&response_type=code&redirect_uri={returnUrl}&scope={scopes}");
            return new Uri(loginUrl.ToString());
        }

        public static async Task<string> GetAccessTokenAsync(PushPayClientOptions options, string returnUrl, string code) {
            using (var httpClient = new HttpClient()) {
                httpClient.BaseAddress = new Uri(options.IsStaging ? "https://auth.pushpay.com/pushpay-sandbox" : "https://auth.pushpay.com/pushpay");
                var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes($"{options.ClientID}:{options.ClientSecret}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_url", returnUrl)
                });

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(toEncodeAsBytes, 0, toEncodeAsBytes.Length));
                var response = await httpClient.PostAsync("oauth/token", content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
