using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PushPay.Models;
using PushPay.QueryObjects;

namespace PushPay.Sets {
    public class OrganizationSet : BaseSet<Organization> {
        public OrganizationSet(PushPayOptions options, OAuthToken token) : base(options, token) {

        }

        /// <summary>
        /// Get an organization from PushPay based on the organization key
        /// </summary>
        /// <param name="organizationKey">the organiation key to use to get the organization</param>
        /// <returns>A PushPay response with the organization data if it exists</returns>
        public async new Task<IPushPayResponse<Organization>> GetAsync(string organizationKey) {
            return await base.GetAsync($"/v1/organization/{organizationKey}");
        }

        /// <summary>
        /// Get all the organizations that the admin has scope for
        /// </summary>
        /// <param name="qo">A Query object to keep track of filters</param>
        /// <returns>A collection of organizations that are in scope</returns>
        public async Task<IPushPayResponse<PushPayCollection<Organization>>> InScopeAsync(OrganizationQO qo) {
            return await FindAsync($"/v1/organizations/in-scope", qo);
        }

        /// <summary>
        /// Search all organizations
        /// </summary>
        /// <param name="qo">A Query object to keep track of filters</param>
        /// <returns>A collection of organizations</returns>
        public async Task<IPushPayResponse<PushPayCollection<Organization>>> FindAsync(OrganizationQO qo) {
            return await FindAsync($"/v1/organizations", qo);
        }
    }
}
