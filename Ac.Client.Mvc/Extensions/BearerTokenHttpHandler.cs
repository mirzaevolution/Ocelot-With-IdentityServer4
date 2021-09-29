using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Ac.Client.Mvc.Extensions
{
    public class BearerTokenHttpHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BearerTokenHttpHandler(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        protected override async Task<HttpResponseMessage> SendAsync
            (HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.SetBearerToken(await GetToken());
            return await base.SendAsync(request, cancellationToken);

        }
        private async Task<string> GetToken()
        {
            string token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset utcToken = DateTimeOffset.Parse(
                    await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at") ?? 
                    DateTimeOffset.UtcNow.ToString()
            );
            var httpClient = _httpClientFactory.CreateClient("General");
            if(utcNow >= utcToken.AddMinutes(1))
            {

                var discoveryResponse =  await httpClient.GetDiscoveryDocumentAsync(_configuration["Resources:IdentityServer:BaseUrl"]);
                if(discoveryResponse.IsError)
                    throw new Exception(discoveryResponse.Error);

                var refreshTokenResponse = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    ClientId = _configuration["Resources:IdentityServer:ClientId"],
                    ClientSecret = _configuration["Resources:IdentityServer:ClientSecret"],
                    RefreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token"),
                    Address = discoveryResponse.TokenEndpoint
                });
                if (refreshTokenResponse.IsError)
                {
                    throw new Exception(refreshTokenResponse.Error);
                }

                var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                authenticateResult.Properties.StoreTokens(new List<AuthenticationToken>
                    {
                        new AuthenticationToken { Name = OidcConstants.TokenTypes.IdentityToken, Value = refreshTokenResponse.IdentityToken },
                        new AuthenticationToken { Name = OidcConstants.TokenTypes.AccessToken, Value = refreshTokenResponse.AccessToken },
                        new AuthenticationToken { Name = OidcConstants.TokenTypes.RefreshToken, Value = refreshTokenResponse.RefreshToken },
                        new AuthenticationToken { Name = "expires_at", Value = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(refreshTokenResponse.ExpiresIn)).ToString("O") }
                    });
                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    authenticateResult.Principal,
                    authenticateResult.Properties);

                token = refreshTokenResponse.AccessToken;

            }
            return token;
        }
    }
}
