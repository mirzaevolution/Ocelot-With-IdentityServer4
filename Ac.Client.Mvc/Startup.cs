using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using IdentityModel;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Http.Extensions;

using System.Net.Http;
using System.Net.Http.Headers;
using Ac.Client.Mvc.Extensions;

namespace Ac.Client.Mvc
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
                options.DefaultScheme  = options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/auth/login");
                    options.LogoutPath = new PathString("/auth/logout");
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = Configuration["Resources:IdentityServer:BaseUrl"];
                    options.ClientId = Configuration["Resources:IdentityServer:ClientId"];
                    options.ClientSecret = Configuration["Resources:IdentityServer:ClientSecret"];
                    options.SaveTokens = true;
                    options.ResponseType = OidcConstants.ResponseTypes.Code;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    foreach(string scope in Configuration["Resources:IdentityServer:Scopes"]
                    .Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        options.Scope.Add(scope);
                    }
                    options.ClaimActions.MapAll();
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.TokenValidationParameters.RoleClaimType = "role";
                });
            services.AddSingleton<BearerTokenHttpHandler>();
            services.AddHttpContextAccessor();
            services.AddHttpClient("General");
            services.AddHttpClient("Api", o =>
            {
                o.BaseAddress = new Uri(Configuration["Resources:BaseApiUrl"]);
            }).AddHttpMessageHandler<BearerTokenHttpHandler>();
            services.AddControllersWithViews();
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
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
