using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Bs.Ac.WebApp.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Bs.Ac.WebApp
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options => 
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/auth/login");
                    options.LogoutPath = new PathString("/auth/logout");
                    options.AccessDeniedPath = new PathString("/accessdenied");
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = Configuration["Idp:ClientId"];
                    options.ClientSecret = Configuration["Idp:ClientSecret"];
                    options.Authority = Configuration["Idp:BaseAddress"];
                    options.AccessDeniedPath = new PathString("/accessdenied");
                    options.ResponseType = OidcConstants.ResponseTypes.Code;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SaveTokens = true;
                    options.ClaimActions.MapAll();
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    options.Scope.Add(OidcConstants.StandardScopes.Profile);
                    options.Scope.Add(OidcConstants.StandardScopes.Email);
                    options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                    options.Scope.Add("bs.ac.api:read");
                    options.Scope.Add("bs.ac.api:write");
                    options.Scope.Add("bs.ac.api:export");
                    options.Scope.Add("roles");

                });
            services.AddTransient<IAuthorizationHandler, OwnerAndWriterAccessPolicyHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnerAndWriterAccess", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements((new OwnerAndWriterAccessPolicyRequirement()));
                });
            });
            services.AddHttpContextAccessor();
            services.AddScoped<BearerTokenHandler>();
            services.AddHttpClient("GeneralAccess");
            services.AddHttpClient("ApiAccessV1", options =>
            {
                options.BaseAddress = new Uri(Configuration["Api:v1:BaseAddress"]);
            }).AddHttpMessageHandler<BearerTokenHandler>();
            services.AddHttpClient("ApiAccessV2", options =>
            {
                options.BaseAddress = new Uri(Configuration["Api:v2:BaseAddress"]);
            }).AddHttpMessageHandler<BearerTokenHandler>();
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                .RequireAuthorization();
            });
        }
    }
}
