using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay.Models {
    public class ExternalLink {
        public Application Application { get; set; }

        public string Relationship { get; set; }

        public string Value { get; set; }
    }
}
