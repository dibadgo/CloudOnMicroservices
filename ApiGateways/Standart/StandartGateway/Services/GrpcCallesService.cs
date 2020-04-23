using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StandartGateway.Services
{
    public class GrpcCallerService
    {
        private readonly HttpClient httpClient;

        private readonly ILogger<GrpcCallerService> logger;

        public GrpcCallerService(HttpClient httpClient, ILogger<GrpcCallerService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var channel = GrpcChannel.ForAddress(urlGrpc, new GrpcChannelOptions { HttpClient = httpClient });

            logger.LogInformation("Creating grpc client base address urlGrpc = {@urlGrpc}, BaseAddress = {@BaseAddress} ", urlGrpc, channel.Target);

            try
            {
                return await func(channel);
            }
            catch (RpcException e)
            {
                logger.LogError(e, "Error during request to the service");
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }
    }
}
