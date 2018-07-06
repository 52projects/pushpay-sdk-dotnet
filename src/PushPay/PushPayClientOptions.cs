﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay {
    public class PushPayClientOptions {
        public PushPayClientOptions() {
            IsStaging = false;
        }

        public bool IsStaging { get; set; }

        public string ClientID { get; set; }

        public string ClientSecret { get; set; }
    }
}
