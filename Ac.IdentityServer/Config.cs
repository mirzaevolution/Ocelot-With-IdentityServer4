// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Ac.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", new string[]
                {
                    "role"
                })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("product_service","Scope for accessing Product Service"),
                new ApiScope("order_service", "Scope for accessing Order Service"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("ApiRepo","Api Repository")
                {
                    Scopes =
                    {
                        "product_service",
                        "order_service"
                    }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client_app",
                    ClientSecrets =
                    {
                        new Secret("future".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes =
                    {
                        OidcConstants.StandardScopes.OpenId,
                        OidcConstants.StandardScopes.Profile,
                        OidcConstants.StandardScopes.Email,
                        OidcConstants.StandardScopes.OfflineAccess,
                        "roles",
                        "product_service",
                        "order_service"
                    },
                    AllowOfflineAccess = true,
                    Description = "Web Client for Product and Service Apis",
                    RedirectUris =
                    {
                        "https://localhost:44390/swagger/oauth2-redirect.html",
                        "https://localhost:44391/swagger/oauth2-redirect.html",
                        "https://localhost:44393/signin-oidc"
                    },
                    AllowedCorsOrigins =
                    {
                        "https://localhost:44390",
                        "https://localhost:44391"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44393/signout-callback-oidc"
                    }
                }
            };
    }
}