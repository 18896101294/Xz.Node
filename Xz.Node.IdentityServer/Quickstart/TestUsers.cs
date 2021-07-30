// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Test;
using Xz.Node.Framework.Common;

namespace Xz.Node.IdentityServer.Quickstart
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "System", Username = Define.SYSTEM_USERNAME, Password = Define.SYSTEM_USERPWD, 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "System"),
                    new Claim(JwtClaimTypes.GivenName, "Xz"),
                    new Claim(JwtClaimTypes.FamilyName, "xiang")
                }
            }
        };
    }
}