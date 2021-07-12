using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Bs.Ac.WebApp.Helpers
{
    public class OwnerAndWriterAccessPolicyRequirement : IAuthorizationRequirement
    {
        public string[] AllowedList => new string[] { "owner", "writer" };
    }
    public class OwnerAndWriterAccessPolicyHandler : AuthorizationHandler<OwnerAndWriterAccessPolicyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerAndWriterAccessPolicyRequirement requirement)
        {
            Claim claim = context.User.Claims.FirstOrDefault(c => c.Type == "role");
            if(claim == null)
            {

                context.Fail();
            }
            else
            {
                List<string> values = new List<string>();
                if (claim.Value.IndexOf(",") > -1)
                {
                    values.AddRange(
                            JsonConvert.DeserializeObject<string[]>(claim.Value)
                        );
                }
                else
                {
                    values.Add(claim.Value);

                }
                bool exists = false;
                foreach(var value in values)
                {
                    if(requirement.AllowedList.Contains(value))
                    {
                        exists = true;
                        break;
                    }
                }
                if(exists)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}
