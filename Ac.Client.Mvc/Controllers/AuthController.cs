using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Ac.Client.Mvc.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
                AllowRefresh = true,
                RedirectUri = Url.Action("Index","Home",null,HttpContext.Request.Scheme)
            });
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home", null, HttpContext.Request.Scheme)
            });
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
