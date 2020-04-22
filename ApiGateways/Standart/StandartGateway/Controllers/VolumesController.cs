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
    /// <summary>
    /// Volumes controller
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class VolumesController : ControllerBase
    {
        /// <summary>
        /// Custom logger
        /// </summary>
        private readonly ILogger<VolumesController> logger;
        /// <summary>
        /// Volume services
        /// </summary>
        private readonly IVolumeDataSource volumeSevice;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Custom logger</param>
        /// <param name="volumeSevice">Volume service</param>
        public VolumesController(ILogger<VolumesController> logger, IVolumeDataSource volumeSevice)
        {
            this.logger = logger;
            this.volumeSevice = volumeSevice;
        }

        /// <summary>
        /// Volume search by Id
        /// </summary>
        /// <param name="volumeId">Volume id (vol-xxxxxxx)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/{volumeId}")]
        public async Task<IActionResult> Get(string volumeId)
        {
            var resp = await volumeSevice.GetVolume(volumeId);

            return Ok(resp);
        }

        /// <summary>
        /// Obtain the available list of volumes for current user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var resp = await volumeSevice.Volumes();

            return Ok(resp);
        }

        /// <summary>
        /// Create a volume by provided configuration
        /// </summary>
        /// <param name="configuration">The configuration for a new volume</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Create([FromBody] VolumeConfigurationBindModel configuration)
        {
            var resp = await volumeSevice.CreateVolume(configuration);

            return Ok(resp);
        }

        /// <summary>
        /// Make a test call to volume service
        /// </summary>
        /// <param name="configuration">The configuration for a new volume</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Test()
        {
            var resp = await volumeSevice.Test();

            return Ok(resp);
        }
    }
}
