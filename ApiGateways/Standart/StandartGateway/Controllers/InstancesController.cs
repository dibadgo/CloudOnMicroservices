using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StandartGateway.BindModels;
using StandartGateway.Services;

namespace StandartGateway.Controllers
{
    /// <summary>
    /// The controller for instances managment
    /// </summary>
    [Route("instances")]
    [ApiController]
    public class InstancesController : ControllerBase
    {
        private readonly IInstanceDataSource instanceDataSource;

        private readonly IVolumeDataSource volumeDataSource;

        private readonly ILogger<InstancesController> logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="instanceDataSource"></param>
        /// <param name="volumeDataSource"></param>
        /// <param name="logger"></param>
        public InstancesController(IInstanceDataSource instanceDataSource, IVolumeDataSource volumeDataSource, ILogger<InstancesController> logger)
        {
            this.instanceDataSource = instanceDataSource;
            this.volumeDataSource = volumeDataSource;
            this.logger = logger;
        }

        /// <summary>
        /// Launch a new instance
        /// </summary>
        /// <param name="instanceConfiguration">Instance configuration</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Launch([FromBody] LaunchInstanceConfiguration instanceConfiguration)
        {

            var systemVolume = await volumeDataSource.GetVolume(instanceConfiguration.SystemVolumeId);
            if (systemVolume == null)
                return BadRequest($"Cannot find a system volume {instanceConfiguration.SystemVolumeId}");

            foreach (var volumeId in instanceConfiguration.DataVolumeIds)
            {
                var dataVolume = await volumeDataSource.GetVolume(volumeId);
                if (dataVolume == null)
                    return BadRequest($"Cannot find a data volume {volumeId}");
            }

            var resp = await instanceDataSource.LaunchAsync(instanceConfiguration);

            return Ok(resp);
        }

        /// <summary>
        /// Start the instance
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{instanceId}/start")]
        public async Task<IActionResult> Start(string instanceId)
        {
            var resp = await instanceDataSource.StartAsync(instanceId);

            return Ok(resp);
        }

        /// <summary>
        /// Stop the instance
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{instanceId}/stop")]
        public async Task<IActionResult> Stop(string instanceId)
        {
            var resp = await instanceDataSource.StopAsync(instanceId);

            return Ok(resp);
        }

        /// <summary>
        /// Terminate the instance
        /// </summary>
        /// <param name="instanceId">Instance Id</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{instanceId}/terminate")]
        public async Task<IActionResult> Terminate(string instanceId)
        {
            var resp = await instanceDataSource.TerminateAsync(instanceId);

            return Ok(resp);
        }
    }
}