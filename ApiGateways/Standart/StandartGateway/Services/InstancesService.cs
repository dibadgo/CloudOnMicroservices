using Instances.API;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StandartGateway.BindModels;
using StandartGateway.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
//using Instances.API;

namespace StandartGateway.Services
{
    public class InstancesService : IInstanceDataSource
    {
        private readonly HttpClient httpClient;
        private readonly UrlsConfig urls;
        private readonly ILogger<InstancesService> logger;
        private readonly GrpcCallerService grpcCallerService;
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="grpcCallerService"></param>
        public InstancesService(HttpClient httpClient, ILogger<InstancesService> logger, IOptions<UrlsConfig> config, GrpcCallerService grpcCallerService)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.urls = config.Value;
            this.grpcCallerService = grpcCallerService;
        }

        /// <summary>
        /// Launch a new instance by the provided confiduration
        /// </summary>
        /// <param name="configuration">Configurations</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> LaunchAsync(LaunchInstanceConfiguration configuration)
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Instances.API.Instances.InstancesClient(channel);
                //new Instances.InstanceClient()

                logger.LogDebug("grpc client created, request = {@id}", "");

                LaunchInstanceRequest launchInstanceRequest = new LaunchInstanceRequest()
                {
                    Name = configuration.Name,
                    InstanceType = configuration.InstanceType,
                    SystemVolume = configuration.SystemVolumeId
                };
                launchInstanceRequest.DataVolumes.AddRange(configuration.DataVolumeIds);

                var response = await client.LaunchAsync(launchInstanceRequest);
                
                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }

        /// <summary>
        /// Start the instance
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> StartAsync(string instanceId)
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Instances.API.Instances.InstancesClient(channel);

                logger.LogDebug("grpc client created, request = {@id}", "");

                InstanceId instanceIdRequest = new InstanceId() { Id = instanceId };

                var response = await client.StartAsync(instanceIdRequest);

                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }

        /// <summary>
        /// Stop the instance
        /// </summary>
        /// <param name="instanceId">The instance id</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> StopAsync(string instanceId)
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Instances.API.Instances.InstancesClient(channel);

                logger.LogDebug("grpc client created, request = {@id}", "");

                InstanceId instanceIdRequest = new InstanceId() { Id = instanceId };

                var response = await client.StopAsync(instanceIdRequest);

                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }

        /// <summary>
        /// Terminate the instance
        /// </summary>
        /// <param name="instanceId">Instance id</param>
        /// <returns></returns>
        public async Task<InstanceGrpc> TerminateAsync(string instanceId)
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Instances.API.Instances.InstancesClient(channel);

                logger.LogDebug("grpc client created, request = {@id}", "");

                InstanceId instanceIdRequest = new InstanceId() { Id = instanceId };

                var response = await client.StopAsync(instanceIdRequest);

                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }
    }
}
