using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay.Models {
    public class Payer {
        public string Key { get; set; }

        public string EmailAddress { get; set; }

        public bool EmailAddressVerified { get; set; }

        public string MobileNumber { get; set; }

        public bool MobileNumberVerified { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}
