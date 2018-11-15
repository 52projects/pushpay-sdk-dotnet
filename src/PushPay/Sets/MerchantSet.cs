using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Models;
using PushPay.QueryObjects;

namespace PushPay.Sets {
    public class MerchantSet : BaseSet<Merchant> {
        public MerchantSet(PushPayOptions options, OAuthToken token) : base(options, token) {

        }

        /// <summary>
        /// Get all the merchants that the admin has scope for
        /// </summary>
        /// <returns>A collection of merchants that are in scope</returns>
        public async Task<IPushPayResponse<PushPayCollection<Merchant>>> FindInScopeAsync() {
            return await FindAsync($"/v1/merchants/in-scope");
        }

        public async Task<IPushPayResponse<PushPayCollection<Merchant>>> FindAsync(string organizationKey, MerchantQO qo) {
            return await base.FindAsync($"/v1/organization/{organizationKey}/merchantlistings", qo);
        }

        /// <summary>
        /// Get a specific merchant by key
        /// </summary>
        /// <returns>A merchant</returns>
        public new async Task<IPushPayResponse<Merchant>> GetAsync(string key) {
            return await base.GetAsync($"/v1/merchant/{key}");
        }
    }
}
