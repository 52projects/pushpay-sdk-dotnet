using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Models;
using PushPay.QueryObjects;

namespace PushPay.Sets {
    public class PaymentSet : BaseSet<Payment> {
        public PaymentSet(PushPayOptions options, OAuthToken token) : base(options, token) {
        }

        /// <summary>
        /// Get the payment based on a merchant Key and Payment Token
        /// </summary>
        /// <param name="merchantKey">The merchant the payment will be associated to</param>
        /// <param name="token">The payment token to find</param>
        /// <returns>A payment from Push Pay</returns>
        public new async Task<IPushPayResponse<Payment>> GetAsync(string merchantKey, string token) {
            return await base.GetAsync($"/v1/merchant/{merchantKey}/payment/{token}");
        }
    }
}
