using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CommonLib.Services
{
    /// <summary>
    /// Wrapper on Http context to obtains user's info
    /// </summary>
    public class IdentityService : IIdentityService
    {
        /// <summary>
        /// Http context accessor
        /// </summary>
        private readonly IHttpContextAccessor context;

        public IdentityService(IHttpContextAccessor context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Returns User Id
        /// </summary>
        /// <returns>User id (UUID)</returns>
        public string GetUserIdentity()
        {
            return context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
    }
}
