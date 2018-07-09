using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Attributes;

namespace PushPay.QueryObjects {
    public class OrganizationQO : BaseQO {

        /// <summary>
        /// Filter the organizations based on a partial of full name
        /// </summary>
        [QO("name")]
        public string Name { get; set; }

        /// <summary>
        /// Filter organizations by Active or Closed. Not putting a value will return both
        /// </summary>
        [QO("status")]
        public string Status { get; set; }
    }
}
