using Disks.gRPC.Service.Repos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Services
{
    /// <summary>
    /// Service  for manage volumes
    /// </summary>
    [Authorize]
    public class VolumesService : Volume.VolumeBase
    {
        /// <summary>
        /// Volume data source
        /// </summary>
        private readonly IVolumeDataSource volumeDataSource;        
        /// <summary>
        /// Identity service
        /// </summary>
        private readonly IIdentityService identityService;
        /// <summary>
        /// Custom logger
        /// </summary>
        private readonly ILogger<VolumesService> logger;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="volumeDataSource">Volume data source</param>
        /// <param name="logger">Custom logger</param>
        /// <param name="identityService">Identity service</param>
        public VolumesService(IVolumeDataSource volumeDataSource, ILogger<VolumesService> logger, IIdentityService identityService)
        {
            this.volumeDataSource = volumeDataSource;
            this.logger = logger;
            this.identityService = identityService;
        }
        /// <summary>
        /// Creates a volume
        /// </summary>
        /// <param name="request">Volume request</param>
        /// <param name="context">Context</param>
        /// <returns></returns>
        public override async Task<VolumeReply> Create(CreateVolumeRequest request, ServerCallContext context)
        {

            string userId = identityService.GetUserIdentity();
            return await volumeDataSource.Create(request, userId);
        }
        /// <summary>
        /// Get the volume by Id
        /// </summary>
        /// <param name="request">Requst volume</param>
        /// <param name="context">Context</param>
        /// <returns></returns>
        public override async Task<VolumeReply> Get(GetVolume request, ServerCallContext context)
        {
            string userId = identityService.GetUserIdentity();
            return await volumeDataSource.Get(request.Id, userId);
        }

        /// <summary>
        /// List of User's volumes
        /// </summary>
        /// <param name="request">Requst</param>
        /// <param name="responseStream">Responce stream</param>
        /// <param name="context">Context</param>
        /// <returns></returns>
        /// <see cref="https://docs.microsoft.com/ru-ru/aspnet/core/grpc/authn-and-authz?view=aspnetcore-3.1"/>
        public override async Task List(Empty request, IServerStreamWriter<VolumeReply> responseStream, ServerCallContext context)
        {
            string userId = identityService.GetUserIdentity();
            
            foreach (VolumeReply reply in await volumeDataSource.List(userId))
                await responseStream.WriteAsync(reply);
        }
    }
}
