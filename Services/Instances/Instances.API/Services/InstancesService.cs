using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Instances.API.Repos;
using Microsoft.Extensions.Logging;
using CommonLib.Services;

namespace Instances.API
{
    public class InstancesService : Instances.InstancesBase
    {
        /// <summary>
        /// Instance data source
        /// </summary>
        private readonly IInstanceDataSource instanceDataSource;
        /// <summary>
        /// Custom logger
        /// </summary>
        private readonly ILogger<InstancesService> logger;
        /// <summary>
        /// Identity service
        /// </summary>
        private readonly IIdentityService identityService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="instanceDataSource"></param>
        /// <param name="identityService"></param>
        public InstancesService(ILogger<InstancesService> logger, IInstanceDataSource instanceDataSource, IIdentityService identityService)
        {
            this.logger = logger;
            this.instanceDataSource = instanceDataSource;
            this.identityService = identityService;
        }

        /// <summary>
        /// Launch instance from configuration request
        /// </summary>
        /// <param name="request">Configuration request</param>
        /// <param name="context">ServerCallContext</param>
        /// <returns></returns>
        public override async Task<InstanceGrpc> Launch(LaunchInstanceRequest request, ServerCallContext context)
        {
            string userId = identityService.GetUserIdentity();
            return await instanceDataSource.LaunchAsync(request, userId);
        }
        /// <summary>
        /// Starts the instance
        /// </summary>
        /// <param name="request">Instance Id</param>
        /// <param name="context">ServerCallContext</param>
        /// <returns></returns>
        public override async Task<InstanceGrpc> Start(InstanceId request, ServerCallContext context)
        {
            string userId = identityService.GetUserIdentity();
            return await instanceDataSource.StartAsync(request.Id, userId);
        }
        /// <summary>
        /// Stop instance
        /// </summary>
        /// <param name="request">Instance Id</param>
        /// <param name="context">ServerCallContext</param>
        /// <returns></returns>
        public override async Task<InstanceGrpc> Stop(InstanceId request, ServerCallContext context)
        {
            string userId = identityService.GetUserIdentity();
            return await instanceDataSource.StopAsync(request.Id, userId);
        }
        /// <summary>
        /// Terminate instance
        /// </summary>
        /// <param name="request">Instance Id</param>
        /// <param name="context">ServerCallContext</param>
        /// <returns></returns>
        public override async Task<InstanceGrpc> Terminate(InstanceId request, ServerCallContext context)
        {
            string userId = identityService.GetUserIdentity();
            return await instanceDataSource.TerminateAsync(request.Id, userId);
        }
    }
}
