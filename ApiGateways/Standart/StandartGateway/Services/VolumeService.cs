using Disks.gRPC.Service;
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
    public class VolumeService : IVolumeSevice
    {
        private readonly HttpClient httpClient;
        private readonly UrlsConfig urls;
        private readonly ILogger<VolumeService> logger;
        private readonly GrpcCallerService grpcCallerService;

        public VolumeService(HttpClient httpClient, ILogger<VolumeService> logger, IOptions<UrlsConfig> config, GrpcCallerService grpcCallerService)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.urls = config.Value;
            this.grpcCallerService = grpcCallerService;
        }

        public async Task<VolumeReply> CreateVolume()
        {
            return await grpcCallerService.CallService(urls.Volumes, async channel =>
            {
                var client = new Volume.VolumeClient(channel);
                logger.LogDebug("grpc client created, request = {@id}", "");

                var response = await client.GetAsync(new GetVolume() { Id = "vol-00000"});
                logger.LogDebug("grpc response {@response}", response);

                return response;
            });
        }
    }
}
