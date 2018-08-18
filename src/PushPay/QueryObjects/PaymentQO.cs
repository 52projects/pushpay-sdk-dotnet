using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Attributes;

namespace PushPay.QueryObjects {
    public class PaymentQO : BaseQO {
        [QOIgnore()]
        public string OrganizationKey { get; set; }

        /// <summary>
        /// Only include payments after a date/time(UTC)
        /// </summary>
        [QO("from")]
        public DateTime? FromDate { get; set; }

        [QO("updatedFrom")]
        public DateTime? UpdatedFrom { get; set; }
    }
}
