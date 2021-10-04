using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace TM.Api.Extensions
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool hasAuthorizeAttribute =
                context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ||
                context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

            if (hasAuthorizeAttribute)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = GlobalConstants.SwaggerSecurityDefinitionKey
                                }
                            },
                            new string[] 
                            { 
                                "openid",
                                "profile",
                                "email",
                                "Roles",
                                "TM.Api:read"
                            }
                        }
                    }
                };
            }

        }
    }
}
