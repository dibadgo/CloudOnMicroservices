using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Disks.gRPC.Service;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StandartGateway.Services;

namespace StandartGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VolumesController : ControllerBase
    {
       
        private readonly ILogger<VolumesController> _logger;

        private readonly IVolumeSevice volumeSevice;

        public VolumesController(ILogger<VolumesController> logger, IVolumeSevice volumeSevice)
        {
            _logger = logger;
            this.volumeSevice = volumeSevice;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var resp = await volumeSevice.CreateVolume();

            return Ok(resp);
        }
    }
}
