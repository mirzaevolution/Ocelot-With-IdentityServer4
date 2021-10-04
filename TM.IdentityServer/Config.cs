// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityModel;

namespace TM.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("Roles", new string[]{ "role" })
            };

        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("TM.Api")
            {
                Scopes =
                {
                    "TM.Api:read"
                }
            }
        };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("TM.Api:read","TM Api Read Access")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "TM.App",
                    ClientSecrets = { new Secret("future".Sha256()) },
                    
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = 
                    { 
                        "https://localhost:44333/signin-oidc",
                        "https://localhost:44332/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = { "https://localhost:44333/signout-callback-oidc" },
                    AllowedCorsOrigins =
                    {
                        "https://localhost:44332"
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 120, //for testing purpose
                    AllowedScopes =
                    {
                        OidcConstants.StandardScopes.OpenId,
                        OidcConstants.StandardScopes.Profile,
                        OidcConstants.StandardScopes.Email,
                        OidcConstants.StandardScopes.OfflineAccess,
                        "Roles",
                        "TM.Api:read"
                    }
                },
                new Client
                {
                    ClientId = "TM.SPA",
                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes =
                    {
                        OidcConstants.StandardScopes.OpenId,
                        OidcConstants.StandardScopes.Profile,
                        OidcConstants.StandardScopes.Email,
                        "Roles",
                        "TM.Api:read"
                    },
                    AllowedCorsOrigins =
                    {
                        "https://localhost:44342"
                    },
                    RedirectUris =
                    {
                        "https://localhost:44342/login-callback"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44342/logout-callback"
                    },
                }
            };
    }
}