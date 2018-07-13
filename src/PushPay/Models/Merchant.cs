using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay.Models {
    public class Merchant {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Handle { get; set; }

        public string HomeCountry { get; set; }

        public string Status { get; set; }

        public string Address { get; set; }

        public int Version { get; set; }
    }
}
