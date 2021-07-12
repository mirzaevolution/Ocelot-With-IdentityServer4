using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Bs.Ac.WebApp.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties
            {
               RedirectUri = Url.Action("Index","Home",null,Request.Scheme)
            });
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return SignOut(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
        }
        [AllowAnonymous]
        [Route("/AccessDenied")]
        public IActionResult AccessDenied() => View();
    }
}
