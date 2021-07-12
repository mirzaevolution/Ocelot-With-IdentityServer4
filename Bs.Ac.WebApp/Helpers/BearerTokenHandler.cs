using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using IdentityModel;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Bs.Ac.WebApp.Helpers
{
    public class BearerTokenHandler:DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor, 
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClientFactory.CreateClient("GeneralAccess");
            _configuration = configuration;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           
            request.SetBearerToken(await GetAccessTokenAsync());
            return await base.SendAsync(request, cancellationToken);
        }
        private async Task<string> GetAccessTokenAsync()
        {
            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken);
            string expiresAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");
            DateTimeOffset currentUtcOffset = DateTimeOffset.UtcNow;
            DateTimeOffset expiresAtUtcOffset = DateTimeOffset.Parse(string.IsNullOrEmpty(expiresAt) ? DateTimeOffset.UtcNow.ToString("O") : expiresAt);
            if (currentUtcOffset > expiresAtUtcOffset)
            {
                //do refresh token
                string refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OidcConstants.TokenResponse.RefreshToken);
                var docResponse = await _httpClient.GetDiscoveryDocumentAsync(_configuration["Idp:BaseAddress"]);
                if (docResponse.IsError)
                {
                    throw new Exception(docResponse.Error);
                }
                var refreshTokenResponse = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = docResponse.TokenEndpoint,
                    ClientId = _configuration["Idp:ClientId"],
                    ClientSecret = _configuration["Idp:ClientSecret"],
                    RefreshToken = refreshToken
                });
                if (refreshTokenResponse.IsError)
                {
                    throw new Exception(refreshTokenResponse.Error);

                }
                List<AuthenticationToken> tokens = new List<AuthenticationToken>
                {
                    new AuthenticationToken
                    {
                        Name = OidcConstants.TokenResponse.IdentityToken,
                        Value = refreshTokenResponse.IdentityToken
                    },
                    new AuthenticationToken
                    {
                        Name = OidcConstants.TokenResponse.AccessToken,
                        Value = refreshTokenResponse.AccessToken
                    },
                    new AuthenticationToken
                    {
                        Name = OidcConstants.TokenResponse.RefreshToken,
                        Value = refreshTokenResponse.RefreshToken
                    },
                    new AuthenticationToken
                    {
                        Name = "expires_at",
                        Value = DateTimeOffset.UtcNow.Add(TimeSpan.FromSeconds(refreshTokenResponse.ExpiresIn)).ToString("O")
                    }
                };

                var currentAuthenticationProperties = await _httpContextAccessor.HttpContext.AuthenticateAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme
                    );
                currentAuthenticationProperties.Properties.StoreTokens(tokens);
                await _httpContextAccessor.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        currentAuthenticationProperties.Principal, 
                        currentAuthenticationProperties.Properties
                    );
                accessToken = refreshTokenResponse.AccessToken;
            }
            return accessToken;
        }
    }
}
