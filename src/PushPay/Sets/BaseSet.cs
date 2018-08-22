using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using PushPay.QueryObjects;
using System.Web;

namespace PushPay.Sets {
    public class BaseSet<T> where T : new() {
        private readonly PushPayOptions _options;
        private readonly OAuthToken _token;

        public BaseSet(PushPayOptions options, OAuthToken token) {
            _options = options;
            _token = token;
        }

        internal async Task<IPushPayResponse<T>> GetAsync(string url) {
            using (var http = CreateClient()) {
                var response = await http.GetAsync(url);
                return await ConvertResponseAsync<T>(response);
            }
        }

        internal async Task<IPushPayResponse<PushPayCollection<T>>> FindAsync(string url) {
            using (var http = CreateClient()) {
                var response = await http.GetAsync(url);
                return await ConvertResponseAsync<PushPayCollection<T>>(response);
            }
        }

        internal async Task<IPushPayResponse<PushPayCollection<T>>> FindAsync(string url, BaseQO qo) {
            using (var http = CreateClient()) {
                var response = await http.GetAsync(BuildURLParametersString(url, qo));
                return await ConvertResponseAsync<PushPayCollection<T>>(response);
            }
        }

        private HttpClient CreateClient() {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_options.IsStaging ? "https://sandbox-api.pushpay.io" : "https://api.pushpay.com");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token.access_token);
            return httpClient;
        }

        private async Task<IPushPayResponse<S>> ConvertResponseAsync<S>(HttpResponseMessage response) where S : new() {
            var pushPayResponse = new PushPayResponse<S> {
                StatusCode = response.StatusCode,
                JsonResponse = await response.Content.ReadAsStringAsync()
            };

            if (!string.IsNullOrEmpty(pushPayResponse.JsonResponse) && (int)response.StatusCode > 300) {
                var responseError = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(pushPayResponse.JsonResponse);
                pushPayResponse.ErrorMessage = responseError.error_message;
            }
            else {
                pushPayResponse.Data = Newtonsoft.Json.JsonConvert.DeserializeObject<S>(pushPayResponse.JsonResponse);
            }
            return pushPayResponse;
        }

        private string BuildURLParametersString(string uri, BaseQO qo) {
            return $"{ uri}?{qo.ToQueryString()}";
        }
    }
}
