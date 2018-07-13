using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay.Models {
    public class Payment {
        public string Status { get; set; }

        public int TrasnactionID { get; set; }

        public string PaymentToken { get; set; }

        public AmountLookup Amount { get; set; }

        public Payer Payer { get; set; }

        public Fund Fund { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string PaymentMethodType { get; set; }

        public string Source { get; set; }

        public string IpAddress { get; set; }

        public Campus Campus { get; set; }
    }
}
