using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bs.Ac.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet(nameof(ReadAccess))]
        [Authorize(Roles = "owner, reader")]
        public IActionResult ReadAccess()
        {
            return Ok(new
            {
                message = "Read-Access!"
            });
        }



        [HttpGet(nameof(WriteAccess))]
        [Authorize(Roles = "owner, writer")]
        public IActionResult WriteAccess()
        {
            return Ok(new
            {
                message = "Write-Access!"
            });
        }
    }
}
