using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PushPay.Models {
    public class AmountLookup {
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        [JsonProperty("details")]
        public AmountDetails Details { get; set; }
    }
}
