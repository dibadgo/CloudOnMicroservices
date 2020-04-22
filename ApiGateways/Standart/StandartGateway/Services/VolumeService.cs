using Disks.gRPC.Service;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StandartGateway.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StandartGateway.Services
{
    /// <summary>
    /// Volume service
    /// </summary>
    public class VolumeService : IVolumeDataSource
    {
        private readonly HttpClient httpClient;
        private readonly UrlsConfig urls;
        private readonly ILogger<VolumeService> logger;
        private readonly GrpcCallerService grpcCallerService;
        /// <summary>
        /// Constructors
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        /// <param name="grpcCallerService"></param>
        public VolumeService(HttpClient httpClient, ILogger<VolumeService> logger, IOptions<UrlsConfig> config, GrpcCallerService grpcCallerService)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.urls = config.Value;
            this.grpcCallerService = grpcCallerService;
        }
        /// <summary>
        /// Obtains the volume by Id
        /// </summary>
        /// <param name="volumeId">Volume id</param>
        /// <returns></returns>
        public async Task<VolumeReply> GetVolume(string volumeId)
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Volume.VolumeClient(channel);
                logger.LogDebug("grpc client created, request = {@id}", "");

                var response = await client.GetAsync(new GetVolume() { Id = volumeId });
                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }

        /// <summary>
        /// Create a volume from the bind model
        /// </summary>
        /// <param name="bindModel">Bind volume with volume's properties</param>
        /// <returns></returns>
        public async Task<VolumeReply> CreateVolume(VolumeConfigurationBindModel bindModel)
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Volume.VolumeClient(channel);
                logger.LogDebug("grpc client created, request = {@id}", "");

                CreateVolumeRequest volumeRequest = new CreateVolumeRequest()
                {
                    Name = bindModel.Name,
                    SizeGb = bindModel.SizeGb,
                    OsType = bindModel.OsType                    
                };
                volumeRequest.MountPints.AddRange(bindModel.MountPoints);

                var response = await client.CreateAsync(volumeRequest);
                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }

        /// <summary>
        /// Obtain all volumes for user 
        /// </summary>
        /// <returns></returns>
        public async Task<List<VolumeReply>> Volumes()
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Volume.VolumeClient(channel);
                logger.LogDebug("grpc client created, request = {@id}", "");

                using var response = client.List(new Empty());

                List<VolumeReply> volumes = new List<VolumeReply>();
                await foreach (var volume in response.ResponseStream.ReadAllAsync())
                {
                    volumes.Add(volume);
                }

                logger.LogDebug($"Featched {volumes.Count} volumes");

                return volumes;
            });
        }
        /// <summary>
        /// Make a test gRPC call to the Volumes service
        /// </summary>
        /// <returns></returns>
        public async Task<VolumeReply> Test()
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Volume.VolumeClient(channel);

                logger.LogDebug("grpc client created, request = {@id}", "");
                var response = await client.TestAsync(new Empty());
                logger.LogDebug("grpc client replyed");

                return response;
            });
        }
    }
}
