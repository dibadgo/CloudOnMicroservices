using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4WebApp.Configure
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserManager<IdentityUser> userManager { get; set; }
        public ResourceOwnerPasswordValidator(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            IdentityUser user = await userManager.FindByNameAsync(context.UserName);
            if (user != null && await userManager.CheckPasswordAsync(user, context.Password))
                context.Result = new GrantValidationResult(user.Id, "custom", await userManager.GetClaimsAsync(user));
            else
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
        }
    }
}
