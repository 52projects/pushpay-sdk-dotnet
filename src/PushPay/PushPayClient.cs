using PushPay.Models;
using PushPay.Sets;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

namespace PushPay {
    public class PushPayClient {
        private readonly OrganizationSet _organizations;
        private readonly FundSet _funds;
        private readonly MerchantSet _merchants;
        private readonly PaymentSet _payments;

        public PushPayClient(PushPayOptions options, OAuthToken token) {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
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
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
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
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
            using (var httpClient = new HttpClient()) {
                try {
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
                catch (Exception e) {
                    return null;
                }
            }
        }

        public static string CreatePushPayUrl(
            PushPayOptions options, 
            string merchantHandle, 
            string sourceReference, 
            string returnUrl, 
            string payerMobilePhone = null, 
            string fundKey = null, 
            decimal? amount = null, 
            string notes = null, 
            string additionalData = null, 
            string recurringFrequency = null,
            string fundVisibility = "show", 
            bool amountLocked = false, 
            bool recurringSelectorVisible = true) {
            var baseUrl = options.IsStaging ? "https://sandbox.pushpay.io" : "https://pushpay.com";
            var sb = new StringBuilder();
            sb.Append($"{baseUrl}/g/{merchantHandle}?sr={sourceReference}&fndv={fundVisibility.ToLower()}&rcv={recurringSelectorVisible.ToString().ToLower()}&ru={returnUrl}&al={amountLocked.ToString().ToLower()}");

            if (string.IsNullOrEmpty(recurringFrequency)) {
                sb.Append("&r=no");
            }
            else {
                sb.Append($"&r={recurringFrequency}");
            }

            if (!string.IsNullOrEmpty(payerMobilePhone)) {
                sb.Append($"&up={payerMobilePhone}");
            }

            if (!string.IsNullOrEmpty(fundKey)) {
                sb.Append($"&fnd={fundKey}");
            }

            if (amount.HasValue) {
                sb.Append($"&a={amount}");
            }

            if (!string.IsNullOrEmpty(notes)) {
                sb.Append($"&nt={notes}");
            }

            if (!string.IsNullOrEmpty(additionalData)) {
                sb.Append($"&ad={additionalData}");
            }

            return sb.ToString();
        }
    }
}
