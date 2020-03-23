using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using static IdentityServer4.IdentityServerConstants;


namespace IdentityServer4WebApp.Configure
{
        public static class Config
        {

            public static IEnumerable<IdentityResource> Ids =>
                new List<IdentityResource>
                {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile()
                };

            public static IEnumerable<Scope> Scopes =>
                new List<Scope> { 
                    new Scope {
                        Name = "api1",
                        DisplayName = "API 1",
                        Description = "API 1 features and data"                    
                    } 
                };
       
            public static IEnumerable<ApiResource> Apis =>
                new List<ApiResource>
                {
                new ApiResource("api1", "My API")
                };

            public static IEnumerable<Client> Clients =>
                new List<Client>
                {
                new Client
                {
                    ClientId = "client",
                    ClientName = "Resource owner Client",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RequireClientSecret = false,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    
                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                }   
                };
        }
    }
