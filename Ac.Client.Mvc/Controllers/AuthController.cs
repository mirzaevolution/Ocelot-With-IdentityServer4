using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;

namespace Ac.Client.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _generalHttpClient;
        private readonly IConfiguration _configuration;
        public AuthController(
                IHttpClientFactory generalHttpClient,
                IConfiguration configuration
            )
        {
            _generalHttpClient = generalHttpClient.CreateClient("General");
            _configuration = configuration;
        }
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                AllowRefresh = true,
                RedirectUri = Url.Action("Index","Home",null,HttpContext.Request.Scheme)
            });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
           var endpointResponse = await _generalHttpClient.GetDiscoveryDocumentAsync(_configuration["Resources:IdentityServer:BaseUrl"]);
            if (!endpointResponse.IsError)
            {
                var revokeResponse = await _generalHttpClient.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = endpointResponse.RevocationEndpoint,
                    ClientId = _configuration["Resources:IdentityServer:ClientId"],
                    ClientSecret = _configuration["Resources:IdentityServer:ClientSecret"],
                    Token = await HttpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken)
                });

            }
           return SignOut(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home", null, HttpContext.Request.Scheme)
            }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> Claims()
        {
            Dictionary<string, string> claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);
            claims.Add("id_token", await HttpContext.GetTokenAsync("id_token"));
            claims.Add("access_token", await HttpContext.GetTokenAsync("access_token"));
            claims.Add("expires_at", await HttpContext.GetTokenAsync("expires_at"));

            return Ok(claims);
        }
    }
}
