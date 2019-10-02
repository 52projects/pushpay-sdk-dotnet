using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PushPay.Models {
    public class Payment {
        public string Status { get; set; }

        public long TransactionID { get; set; }

        public string PaymentToken { get; set; }

        public AmountLookup Amount { get; set; }

        public Payment RefundedBy { get; set; }

        public Payment RefundFor { get; set; }

        public Payer Payer { get; set; }

        public CreditCard Card { get; set; }

        public Fund Fund { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public dynamic Data { get; set; }

        public string PaymentMethodType { get; set; }

        public string Source { get; set; }

        public string SourceReference { get; set; }

        public string IpAddress { get; set; }

        public Campus Campus { get; set; }

        public List<ExternalLink> ExternalLinks { get; set; }

        [JsonProperty(PropertyName = "_links")]
        public PaymentLinks Links { get; set; }

        [JsonIgnore]
        public int? PersonID {
            get {
                var value = GetAttributeValue("person_id");
                int i = 0;

                if (int.TryParse(value, out i)) {
                    return i;
                }

                return null;
            }
        }

        [JsonIgnore]
        public int? ExternalTransactionID {
            get {
                var value = GetAttributeValue("transaction_id");
                int i = 0;

                if (int.TryParse(value, out i)) {
                    return i;
                }

                return null;
            }
        }

        [JsonIgnore]
        public int? FundID {
            get {
                var value = GetAttributeValue("fund_id");
                int i = 0;

                if (int.TryParse(value, out i)) {
                    return i;
                }

                return null;
            }
        }

        private string GetAttributeValue(string relationship) {
            if (ExternalLinks != null && ExternalLinks.Any(x => x.Relationship == relationship)) {
                return ExternalLinks.First(x => x.Relationship == relationship).Value;
            }
            return null;
        }
    }

    public class PaymentLinks {
        public Link Self { get; set; }

        public Link MerchantViewPayment { get; set; }
    }
}
