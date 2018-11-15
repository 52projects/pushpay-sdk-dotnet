using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Attributes;

namespace PushPay.QueryObjects {
    public class MerchantQO : BaseQO {

        /// <summary>
        /// Include a specific status in merchant results, options are Hidden / Active
        /// </summary>
        [QO("visibility")]
        public string Visibility { get; set; }
    }
}
