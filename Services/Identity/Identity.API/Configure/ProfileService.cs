using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer4WebApp.Configure
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> userManager;

        public ProfileService(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.FindFirst("sub")?.Value;
            if (sub != null)
            {
                var user = await userManager.FindByIdAsync(sub);
                var cp = await getClaims(user);

                var claims = cp.Claims;
                if (context.RequestedClaimTypes != null && context.RequestedClaimTypes.Any())
                {
                    claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToArray().AsEnumerable();
                }

                context.IssuedClaims = claims.ToList();
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }

        private async Task<ClaimsPrincipal> getClaims(IdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = new ClaimsIdentity();
            id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.UserName));

            id.AddClaims(await userManager.GetClaimsAsync(user));

            return new ClaimsPrincipal(id);
        }
    }
}
