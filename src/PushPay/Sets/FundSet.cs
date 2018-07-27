using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Models;
using PushPay.QueryObjects;

namespace PushPay.Sets {
    public class FundSet : BaseSet<Fund> {
        public FundSet(PushPayOptions options, OAuthToken token) : base(options, token) {

        }

        /// <summary>
        /// Get all the organizations that the admin has scope for
        /// </summary>
        /// <param name="organizationKey">The organization</param>
        /// <returns>A collection of organizations that are in scope</returns>
        public async Task<IPushPayResponse<PushPayCollection<Fund>>> FindOpenAsync(string organizationKey) {
            return await FindAsync($"/v1/organization/{organizationKey}/funds");
        }

        /// <summary>
        /// Get all the organizations that the admin has scope for
        /// </summary>
        /// <param name="organizationKey">The organization</param>
        /// <returns>A collection of organizations that are in scope</returns>
        public async new Task<IPushPayResponse<PushPayCollection<Fund>>> FindAsync(string merchantKey) {
            return await FindAsync($"/v1/merchant/{merchantKey}/funds");
        }
    }
}