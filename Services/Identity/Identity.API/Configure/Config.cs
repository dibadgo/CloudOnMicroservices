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
                        Name = "gateway",
                        DisplayName = "API gateway",
                        Description = "Standart gateway"                    
                    },
                    new Scope
                    {
                        Name = "volumes",
                        DisplayName = "API gateway",
                        Description = "Standart gateway"
                    }
                };
       
            public static IEnumerable<ApiResource> Apis =>
                new List<ApiResource>
                {
                    new ApiResource("gateway", "Standart gateway"),
                    new ApiResource("volumes", "Volumes API")
                };

            public static IEnumerable<Client> Clients =>
                new List<Client>
                {
                    new Client
                    {
                        ClientId = "client",
                        ClientName = "Resource owner Client",

                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                    
                        AllowedScopes = { "gateway", "volumes" }
                    }   
                };
        }
}
