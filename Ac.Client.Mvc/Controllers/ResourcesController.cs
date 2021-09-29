using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;

namespace Ac.Client.Mvc.Controllers
{
    [Authorize]
    public class ResourcesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public ResourcesController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OrderService()
        {
            string result = await GetResult(_configuration["Resources:OrderService:GetAll"]);
            return Content(result, "application/json");
        }

        public async Task<IActionResult> ProductService()
        {
            string result = await GetResult(_configuration["Resources:ProductService:GetAll"]);
            return Content(result, "application/json");
        }


        private async Task<string> GetResult(string url)
        {
            var client = _httpClientFactory.CreateClient("Api");
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            return response.StatusCode.ToString();
        }
    }
}
