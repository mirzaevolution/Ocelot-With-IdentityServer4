// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4;

namespace IdentityServerHost.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                
                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "00001",
                        Username = "admin",
                        Password = "future",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Administrator"),
                            new Claim(JwtClaimTypes.Email, "admin@core.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim("role","Administrator")
                        }
                    }
                   
                };
            }
        }
    }
}