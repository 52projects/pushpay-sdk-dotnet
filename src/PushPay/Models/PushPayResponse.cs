using System.Net;

namespace PushPay.Models {
    public interface IPushPayResponse {
        string RequestValue { get; set; }

        string JsonResponse { get; set; }

        HttpStatusCode StatusCode { get; set; }

        string ErrorMessage { get; set; }

        bool IsSuccessful { get; }
    }
    public interface IPushPayResponse<T> : IPushPayResponse {
        T Data { get; set; }
    }

    public class PushPayResponse : IPushPayResponse {
        public string RequestValue { get; set; }

        public string JsonResponse { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsSuccessful {
            get {
                return (int)StatusCode >= 200 && (int)StatusCode < 300;
            }
        }
    }

    public class PushPayResponse<T> : PushPayResponse, IPushPayResponse<T> where T : new() {
        public T Data { get; set; }
    }
}
