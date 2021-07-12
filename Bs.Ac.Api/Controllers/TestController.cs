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
        public IActionResult ReadAccess()
        {
            var isReaderRole = User.IsInRole("reader");
            return Ok(new
            {
                message = "Read-Access!"
            });
        }



        [HttpGet(nameof(WriteAccess))]
        public IActionResult WriteAccess()
        {
            var isWriterRole = User.IsInRole("writer");
            return Ok(new
            {
                message = "Write-Access!"
            });
        }
    }
}
