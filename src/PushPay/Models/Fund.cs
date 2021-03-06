﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay.Models {
    public class Fund {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public bool TaxDeductible { get; set; }

        public string Notes { get; set; }

        public string Status { get; set; }
    }
}
