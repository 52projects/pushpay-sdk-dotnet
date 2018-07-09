using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushPay.Models {
    public class PushPayCollection<T> where T : new() {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

        public int TotalPages { get; set; }

        public List<T> Items { get; set; }

        public bool HasMore {
            get {
                return TotalPages > Page + 1;
            }
        }
    }
}
