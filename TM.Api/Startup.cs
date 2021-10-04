using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TM.Api.Extensions;

namespace TM.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["IdentityServer:Authority"];
                    options.Audience = Configuration["IdentityServer:Audience"];
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TM.Api", Version = "v1" });
                c.AddSecurityDefinition(GlobalConstants.SwaggerSecurityDefinitionKey, new OpenApiSecurityScheme
                {
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{Configuration["IdentityServer:Authority"]}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration["IdentityServer:Authority"]}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "TM.Api:read","TM.Api Read Access" },
                                { "openid", "OpenId Connect Scope" },
                                { "profile", "Profile Scope" },
                                { "email", "Email Scope" },
                                { "Roles", "Roles Scope" }
                            }
                        }
                    },
                    Type = SecuritySchemeType.OAuth2
                });
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
            services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy.WithOrigins("https://localhost:44342")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TM.Api v1");
                c.OAuthClientId(Configuration["IdentityServer:ClientId"]);
                c.OAuthClientSecret(Configuration["IdentityServer:ClientSecret"]);
                c.OAuthAppName("TM.Api");
                c.OAuthUsePkce();
            });

            app.UseRouting();
            app.UseCors("Default");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
