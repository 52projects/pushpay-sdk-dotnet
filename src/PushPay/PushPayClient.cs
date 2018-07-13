﻿using PushPay.Models;
using PushPay.Sets;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PushPay {
    public class PushPayClient {
        private readonly OrganizationSet _organizations;
        private readonly FundSet _funds;
        private readonly MerchantSet _merchants;
        private readonly PaymentSet _payments;

        public PushPayClient(PushPayOptions options, OAuthToken token) {
            _organizations = new OrganizationSet(options, token);
            _funds = new FundSet(options, token);
            _merchants = new MerchantSet(options, token);
            _payments = new PaymentSet(options, token);
        }

        public OrganizationSet Organizations {
            get {
                return _organizations;
            }
        }

        public FundSet Funds {
            get {
                return _funds;
            }
        }

        public MerchantSet Merchants {
            get {
                return _merchants;
            }
        }

        public PaymentSet Payments {
            get {
                return _payments;
            }
        }

        /// <summary>
        /// Gets the url that should be sent via browser to authorize the push pay user and get consent
        /// </summary>
        /// <param name="options">Options to set things like client id and secret</param>
        /// <param name="returnUrl">The url to return to when consent is chosen</param>
        /// <param name="scopes">The scopes for the authorization to identify the rights of subsequent calls. NOTE: list should be space separated</param>
        /// <returns>A url to be sent via browser for the user to give consent to application</returns>
        public static Uri GetAuthorizationUrl(PushPayOptions options, string returnUrl, string scopes, string state = null) {
            System.Text.StringBuilder loginUrl = new System.Text.StringBuilder();
            loginUrl.Append(options.IsStaging ? "https://auth.pushpay.com/pushpay-sandbox/oauth/authorize" : "https://auth.pushpay.com/pushpay/oauth/authorize");
            loginUrl.Append($"?client_id={options.ClientID}&response_type=code&redirect_uri={returnUrl}&scope={scopes}");
            if (!string.IsNullOrEmpty(state)) {
                loginUrl.Append($"&state={state}");
            }
            return new Uri(loginUrl.ToString());
        }

        /// <summary>
        /// Request an access token from PushPay after going through the authorization
        /// </summary>
        /// <param name="options">Options to set things like client id and secret</param>
        /// <param name="returnUrl">This url should be the same as the one passed into GetAuthorizationUrl()</param>
        /// <param name="code">The authorization code received from PushPay after authorization</param>
        /// <param name="state">Any specific parameters that need to be sent back from pushpay</param>
        /// <returns>An OAuth Token object to use for subsequent requests</returns>
        public static async Task<IPushPayResponse<OAuthToken>> RequestAccessTokenAsync(PushPayOptions options, string returnUrl, string code, string state = null) {
            using (var httpClient = new HttpClient()) {
                var toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes($"{options.ClientID}:{options.ClientSecret}");

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", returnUrl),
                    new KeyValuePair<string, string>("state", state)
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

        public static string GetGivingBaseUrl(PushPayOptions options, string handle) {
            var baseUrl = options.IsStaging ? "https://sandbox.pushpay.io" : "https://pushpay.io";
            return $"{baseUrl}/g/{handle}";
        }
    }
}
