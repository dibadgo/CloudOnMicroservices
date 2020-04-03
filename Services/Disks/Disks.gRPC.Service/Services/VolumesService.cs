using Disks.gRPC.Service.Repos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Disks.gRPC.Service.Services
{
    public class VolumesService : Volume.VolumeBase
    {
        private readonly IVolumeDataSource volumeDataSource;

        private readonly ILogger<VolumesService> logger;

        public VolumesService(IVolumeDataSource volumeDataSource, ILogger<VolumesService> logger)
        {
            this.volumeDataSource = volumeDataSource;
            this.logger = logger;
        }

        public override async Task<VolumeReply> Create(CreateVolumeRequest request, ServerCallContext context)
        {
            try
            {
                return await volumeDataSource.Create(request);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Redis service error");
                throw new Exception("Something went wrong");
            }
        }

        public override async Task<VolumeReply> Get(GetVolume request, ServerCallContext context)
        {
            try
            {
                return await volumeDataSource.Get(request.Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Redis service error");
                throw new Exception("Something went wrong");
            }           
        }

        public override async Task List(CreateVolumeRequest request, IServerStreamWriter<VolumeReply> responseStream, ServerCallContext context)
        {
            IEnumerable<VolumeReply> list = await volumeDataSource.List();
            
            foreach (VolumeReply reply in list)
                await responseStream.WriteAsync(reply);
        }
    }
}
