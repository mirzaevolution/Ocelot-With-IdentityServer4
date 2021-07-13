// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace Bs.Ac.Idp
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles","User role(s)", new string[]
                {
                    "role"
                })
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("bs.ac.api:read","Bs.Ac.Api [Read Acess]"),
                new ApiScope("bs.ac.api:write","Bs.Ac.Api [Write Access]"),
                new ApiScope("bs.ac.api:export","Bs.Ac.Api v2 [Export Access]")
            };
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("bs.ac.api","Basic Auth Code Api v1 Endpoint")
                {
                    Scopes =
                    {
                        "bs.ac.api:read",
                        "bs.ac.api:write"
                    },
                    UserClaims =
                    {
                        "name",
                        "email",
                        "role"
                    }
                },
                new ApiResource("bs.ac.api.v2","Basic Auth Code Api v2 Endpoint")
                {
                    Scopes =
                    {
                        "bs.ac.api:export"
                    },
                    UserClaims =
                    {
                        "name",
                        "email",
                        "role"
                    }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
               
                new Client
                {
                    ClientId = "bs.ac.webapp",
                    ClientSecrets =
                    {
                        new Secret("default".Sha256())
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = (int)TimeSpan.FromMinutes(2).TotalSeconds, //for testing only
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        "bs.ac.api:read",
                        "bs.ac.api:write",
                        "bs.ac.api:export"
                    },
                    RedirectUris =
                    {
                        "https://localhost:44314/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:44314/signout-callback-oidc"
                    },
                    RequireConsent = true
                }
            };
    }
}