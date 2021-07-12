using Bs.Ac.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Bs.Ac.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ShowClaims()
        {
            Dictionary<string, string> claims = new Dictionary<string, string>();
            claims.Add("id_token", await HttpContext.GetTokenAsync("id_token"));
            claims.Add("access_token", await HttpContext.GetTokenAsync("access_token"));

            foreach (var claim in User.Claims)
            {
                claims.Add(claim.Type, claim.Value);
            }
            return View(claims);
        }
        [Authorize(Policy = "OwnerAndWriterAccess")]
        public IActionResult Privacy()
        {
            return View();
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
