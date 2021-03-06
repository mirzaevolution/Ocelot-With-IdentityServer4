using Bs.Ac.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bs.Ac.WebApp.Controllers
{
    [Authorize]
    public class ApiAccessController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClientV1, _httpClientV2;
        public ApiAccessController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClientV1 = _httpClientFactory.CreateClient("ApiAccessV1");
            _httpClientV2 = _httpClientFactory.CreateClient("ApiAccessV2");

        }
        public async Task<IActionResult> ReadAccess()
        {
            ApiAccessResponse apiResponse = new ApiAccessResponse();
            try
            {
                var response = await _httpClientV1.GetAsync("Api/Test/ReadAccess");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiAccessResponse>(
                            jsonResponse
                        );
                }
                else
                {
                    apiResponse.Message = $"{response.StatusCode}";
                }
            }
            catch(Exception ex)
            {
                apiResponse.Message = ex.Message;    
            }
            return View(apiResponse);
        }
        public async Task<IActionResult> WriteAccess()
        {
            ApiAccessResponse apiResponse = new ApiAccessResponse();
            try
            {
                var response = await _httpClientV1.GetAsync("Api/Test/WriteAccess");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiAccessResponse>(
                            jsonResponse
                        );
                }
                else
                {
                    apiResponse.Message = $"{response.StatusCode}";
                }
            }
            catch(Exception ex)
            {
                apiResponse.Message = ex.Message;
            }
            return View(apiResponse);
        }
        public async Task<IActionResult> ExportAccess()
        {
            ApiAccessResponse apiResponse = new ApiAccessResponse();
            try
            {
                var response = await _httpClientV2.GetAsync("Api/Test/ExportAccess");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    apiResponse = JsonConvert.DeserializeObject<ApiAccessResponse>(
                            jsonResponse
                        );
                }
                else
                {
                    apiResponse.Message = $"{response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                apiResponse.Message = ex.Message;
            }
            return View(apiResponse);
        }
    }
}
