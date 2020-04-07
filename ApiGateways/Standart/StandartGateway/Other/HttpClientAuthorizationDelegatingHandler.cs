using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace StandartGateway.Other
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor httpContextAccesor;
        private readonly ILogger<HttpClientAuthorizationDelegatingHandler> logger;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor, ILogger<HttpClientAuthorizationDelegatingHandler> logger)
        {
            this.httpContextAccesor = httpContextAccesor;
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version = new Version(2, 0);
            
            var authorizationHeader = httpContextAccesor.HttpContext
                .Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                logger.LogDebug("Add Authorization header to request");
                request.Headers.Add("Authorization", new List<string>() { authorizationHeader });
            }
            else
                logger.LogDebug("The Authorization header wasn now found in the source requst");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
