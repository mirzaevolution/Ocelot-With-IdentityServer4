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
                        SubjectId = "12345",
                        Username = "admin",
                        Password = "admin",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Administrator"),
                            new Claim(JwtClaimTypes.Email, "admin@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.Role, "owner")

                        }
                    },
                    new TestUser
                    {
                        SubjectId = "31211",
                        Username = "mirza",
                        Password = "future",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Mirza Ghulam Rasyid"),
                            new Claim(JwtClaimTypes.Email, "ghulamcyber@hotmail.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.Role, "reader", ClaimValueTypes.String),
                            new Claim(JwtClaimTypes.Role, "writer", ClaimValueTypes.String),
                            new Claim(JwtClaimTypes.Role, "export", ClaimValueTypes.String)


                        }
                    },
                    new TestUser
                    {
                        SubjectId = "51214",
                        Username = "rara",
                        Password = "future",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Rara Anjani"),
                            new Claim(JwtClaimTypes.Email, "raraanjani@hotmail.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.Role, "reader"),
                            new Claim(JwtClaimTypes.Role, "export", ClaimValueTypes.String)
                        }
                    }
                };
            }
        }
    }
}