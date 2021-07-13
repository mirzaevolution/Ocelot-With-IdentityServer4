using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bs.Ac.Api.v2.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet(nameof(ExportAccess))]
        [Authorize(Roles = "owner, export")]
        public IActionResult ExportAccess()
        {
            return Ok(new
            {
                message = "Export-Access!"
            });
        }
    }
}
