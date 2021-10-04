using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using IdentityModel.AspNetCore.AccessTokenManagement;
namespace TM.Web.Controllers
{
    [Authorize]
    public class WeatherController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public WeatherController(
                IHttpClientFactory httpClientFactory,
                IConfiguration configuration
            )
        {
            _clientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            HttpClient client =
                _clientFactory.CreateClient("Api");
            var token = await HttpContext.GetUserAccessTokenAsync();
            client.SetBearerToken(token);
            var response = await client.GetAsync("/WeatherForecast");
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                return Content(jsonResponse, "application/json");
            }
            return StatusCode((int)response.StatusCode, null);
        }
    }
}
